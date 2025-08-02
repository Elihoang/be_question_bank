using Aspose.Words.Drawing;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Shared.DTOs.CauHoi;
using BEQuestionBank.Shared.DTOs.CauTraLoi;
using BEQuestionBank.Shared.DTOs.File;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using ImageMagick;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO.Compression;
using System.Diagnostics;

namespace BEQuestionBank.Core.Services
{
    public class WordImportService
    {
        private readonly ICauHoiRepository _cauHoiRepository;
        private readonly ILogger<WordImportService> _logger;
        private readonly string _storagePath;
        private const int BatchSize = 50;
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

        public WordImportService(ICauHoiRepository cauHoiRepository, string storagePath, ILogger<WordImportService> logger)
        {
            _cauHoiRepository = cauHoiRepository ?? throw new ArgumentNullException(nameof(cauHoiRepository));
            _storagePath = storagePath ?? throw new ArgumentNullException(nameof(storagePath));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ImportResult> ImportQuestionsAsync(IFormFile wordFile, Guid maPhan, string? mediaFolderPath)
        {
            var result = new ImportResult();

            // Input validation
            if (wordFile == null || wordFile.Length == 0)
            {
                _logger.LogWarning("Invalid Word file: File is null or empty.");
                result.Errors.Add("Word file is required and must not be empty.");
                return result;
            }

            if (wordFile.Length > MaxFileSize)
            {
                _logger.LogWarning("Word file exceeds size limit: {FileSize} bytes.", wordFile.Length);
                result.Errors.Add($"Word file must not exceed {MaxFileSize / (1024 * 1024)}MB.");
                return result;
            }

            if (!wordFile.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid file format: {FileName}. Expected .docx.", wordFile.FileName);
                result.Errors.Add("File must be in .docx format.");
                return result;
            }

            if (!string.IsNullOrEmpty(mediaFolderPath) && !Directory.Exists(mediaFolderPath))
            {
                _logger.LogWarning("Media folder does not exist: {MediaFolderPath}", mediaFolderPath);
                result.Errors.Add("Media folder does not exist.");
                return result;
            }

            try
            {
                var questions = await ParseWordFileAsync(wordFile);
                await SaveQuestionsToDatabaseAsync(questions, maPhan, mediaFolderPath, result);
                _logger.LogInformation("Successfully imported {SuccessCount} questions with {ErrorCount} errors.", result.SuccessCount, result.Errors.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing questions from Word file.");
                result.Errors.Add($"System error occurred during import: {ex.Message}");
            }

            return result;
        }

        private async Task<List<QuestionData>> ParseWordFileAsync(IFormFile wordFile)
        {
            var questions = new List<QuestionData>();
            var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".docx");

            try
            {
                // Save file to temporary location
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                {
                    await wordFile.CopyToAsync(fileStream);
                }

                // Validate DOCX structure
                if (!IsValidDocxFile(tempFilePath))
                {
                    throw new InvalidOperationException("File is not a valid or corrupted DOCX file.");
                }

                return await ParseWordFileFromPath(tempFilePath);
            }
            finally
            {
                // Ensure temp file is deleted
                if (File.Exists(tempFilePath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete temporary file: {TempFilePath}", tempFilePath);
                    }
                }
            }
        }

        private bool IsValidDocxFile(string filePath)
        {
            try
            {
                using (var zip = ZipFile.OpenRead(filePath))
                {
                    return zip.Entries.Any(e => e.FullName == "[Content_Types].xml") &&
                           zip.Entries.Any(e => e.FullName == "_rels/.rels") &&
                           zip.Entries.Any(e => e.FullName == "word/document.xml");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "File validation failed for: {FilePath}", filePath);
                return false;
            }
        }

        private async Task<List<QuestionData>> ParseWordFileFromPath(string filePath)
        {
            var questions = new List<QuestionData>();
            var cloRegex = new Regex(@"\(CLO(\d+)\)");
            var audioRegex = new Regex(@"\[<audio>\](.*?)\[</audio>\]");
            var imageMapping = new Dictionary<string, string>();

            try
            {
                using (var wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    var mainPart = wordDoc.MainDocumentPart;
                    if (mainPart == null)
                    {
                        throw new InvalidOperationException("Document does not have MainDocumentPart.");
                    }

                    // Process images
                    await ProcessImages(mainPart, imageMapping);

                    var body = mainPart.Document.Body;
                    if (body == null)
                    {
                        _logger.LogWarning("Document body is null.");
                        return questions;
                    }

                    var currentQuestion = new QuestionData();

                    foreach (var para in body.Elements<Paragraph>())
                    {
                        var result = ProcessParagraph(para, imageMapping, cloRegex, audioRegex, currentQuestion);

                        if (result.IsNewQuestion && !string.IsNullOrEmpty(currentQuestion.NoiDung))
                        {
                            questions.Add(currentQuestion);
                            currentQuestion = result.Question;
                        }
                        else if (result.Question != null)
                        {
                            currentQuestion = result.Question;
                        }
                    }

                    if (!string.IsNullOrEmpty(currentQuestion.NoiDung))
                    {
                        questions.Add(currentQuestion);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing Word file: {FilePath}", filePath);
                throw;
            }

            _logger.LogInformation("Parsed {Count} questions from Word file.", questions.Count);
            return questions;
        }

        private async Task ProcessImages(MainDocumentPart mainPart, Dictionary<string, string> imageMapping)
        {
            foreach (var imagePart in mainPart.ImageParts)
            {
                var relId = mainPart.GetIdOfPart(imagePart);
                byte[] imageBytes;

                using (var imgStream = imagePart.GetStream())
                using (var ms = new MemoryStream())
                {
                    await imgStream.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                if (imageBytes.Length == 0)
                {
                    _logger.LogWarning("Dữ liệu hình ảnh trống hoặc không hợp lệ với relId: {RelId}", relId);
                    continue;
                }

                string mimeType = imagePart.ContentType;
                _logger.LogInformation("Ảnh relId={RelId} có mimeType: {MimeType}", relId, mimeType);

                try
                {
                    // Convert WMF to PNG using Inkscape
                    if (mimeType.Equals("image/x-wmf", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Chuyển đổi WMF sang PNG bằng Inkscape cho relId: {RelId}", relId);

                        var tempWmf = Path.GetTempFileName() + ".wmf";
                        var tempPng = Path.GetTempFileName() + ".png";

                        try
                        {
                            File.WriteAllBytes(tempWmf, imageBytes);

                            var psi = new ProcessStartInfo
                            {
                                FileName = "inkscape",
                                Arguments = $"\"{tempWmf}\" --export-type=png --export-filename=\"{tempPng}\"",
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };

                            using (var proc = Process.Start(psi))
                            {
                                await proc.WaitForExitAsync();

                                if (proc.ExitCode != 0)
                                {
                                    var error = await proc.StandardError.ReadToEndAsync();
                                    throw new InvalidOperationException($"Inkscape conversion failed: {error}");
                                }
                            }

                            if (!File.Exists(tempPng))
                            {
                                throw new FileNotFoundException("Inkscape không tạo được file PNG.");
                            }

                            // Read converted PNG data
                            imageBytes = await File.ReadAllBytesAsync(tempPng);
                            mimeType = "image/png";

                            _logger.LogInformation("Chuyển đổi WMF thành công cho relId: {RelId}", relId);
                        }
                        finally
                        {
                            // Clean up temp files
                            if (File.Exists(tempWmf)) File.Delete(tempWmf);
                            if (File.Exists(tempPng)) File.Delete(tempPng);
                        }
                    }

                    // Create Base64 image tag for embedding in HTML
                    var base64Image = Convert.ToBase64String(imageBytes);
                    var imgTag = $@"<img src=""data:{mimeType};base64,{base64Image}"" alt=""Question Image"" style=""max-width: 100%; height: auto;"" />";

                    imageMapping[relId] = imgTag;

                    _logger.LogInformation("Đã xử lý thành công ảnh relId: {RelId} với kích thước: {Size} bytes", relId, imageBytes.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Không thể xử lý ảnh relId: {RelId}", relId);
                    imageMapping[relId] = "<img src='data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9IjEwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwIiBoZWlnaHQ9IjEwMCIgZmlsbD0iI2Y0ZjRmNCIvPjx0ZXh0IHg9IjUwIiB5PSI1NSIgZm9udC1zaXplPSIxMiIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZmlsbD0iIzk5OTk5OSI+SW1hZ2UgTG9hZCBFcnJvcjwvdGV4dD48L3N2Zz4=' alt='Failed to load image' style='max-width: 100%; height: auto;' />";
                }
            }
        }

        private (bool IsNewQuestion, QuestionData Question) ProcessParagraph(
            Paragraph para,
            Dictionary<string, string> imageMapping,
            Regex cloRegex,
            Regex audioRegex,
            QuestionData currentQuestion)
        {
            string text = string.Join("", para.Descendants<Text>().Select(t => t.Text)).Trim();
            string htmlText = !string.IsNullOrEmpty(text) ? $"<p>{HttpUtility.HtmlEncode(text)}</p>" : "";

            // Process images in this paragraph (including WMF converted images)
            var imgs = string.Join("",
                para.Descendants<DocumentFormat.OpenXml.Drawing.Blip>()
                    .Select(b => b.Embed?.Value)
                    .Where(relId => !string.IsNullOrEmpty(relId) && imageMapping.ContainsKey(relId))
                    .Select(relId => imageMapping[relId])
                .Concat(
                    para.Descendants<DocumentFormat.OpenXml.Vml.ImageData>() // WMF/VML images
                        .Select(vmlImg => vmlImg.RelationshipId?.Value)
                        .Where(relId => !string.IsNullOrEmpty(relId) && imageMapping.ContainsKey(relId))
                        .Select(relId => imageMapping[relId])
                )
            );


            // Log if images are found
            if (!string.IsNullOrEmpty(imgs))
            {
                _logger.LogInformation("Tìm thấy {Count} ảnh trong paragraph với text: {Text}",
                    para.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().Count(),
                    text.Length > 50 ? text.Substring(0, 50) + "..." : text);
            }

            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(imgs))
                return (false, currentQuestion);

            // Check for CLO pattern (new question)
            var cloMatch = cloRegex.Match(text);
            if (cloMatch.Success && Enum.TryParse<EnumCLO>($"CLO{cloMatch.Groups[1].Value}", out var clo))
            {
                var newQuestion = new QuestionData { CLO = clo };
                var questionText = text.Substring(cloMatch.Length).Trim();

                // Combine question text with images
                newQuestion.NoiDung = !string.IsNullOrEmpty(questionText)
                    ? $"<p>{HttpUtility.HtmlEncode(questionText)}</p>{imgs}"
                    : imgs;

                _logger.LogInformation("Tạo câu hỏi mới với CLO: {CLO}, có {ImageCount} ảnh",
                    clo, string.IsNullOrEmpty(imgs) ? 0 : imgs.Split("<img").Length - 1);

                return (true, newQuestion);
            }

            // Check for answer options (A., B., C., D.)
            if (text.Length >= 2 && "ABCD".Contains(text[0]) && text[1] == '.')
            {
                var answerKey = text[0];
                var answerContent = text.Substring(2).Trim();

                bool isUnderline = para.Descendants<RunProperties>().Any(rp => rp.Underline != null);
                bool isItalic = para.Descendants<RunProperties>().Any(rp => rp.Italic != null);

                var answerHtml = !string.IsNullOrEmpty(answerContent)
                    ? $"<p>{HttpUtility.HtmlEncode(answerContent)}</p>{imgs}"
                    : imgs;

                currentQuestion.Answers.Add(new AnswerData
                {
                    ThuTu = currentQuestion.Answers.Count + 1,
                    NoiDung = answerHtml,
                    LaDapAn = isUnderline,
                    HoanVi = isItalic
                });

                _logger.LogInformation("Thêm đáp án {AnswerKey} với {ImageCount} ảnh, LaDapAn: {IsCorrect}",
                    answerKey, string.IsNullOrEmpty(imgs) ? 0 : imgs.Split("<img").Length - 1, isUnderline);

                return (false, currentQuestion);
            }

            // Check for audio files
            var audioMatch = audioRegex.Match(text);
            if (audioMatch.Success)
            {
                currentQuestion.Files.Add(new FileData
                {
                    FileName = Path.GetFileName(audioMatch.Groups[1].Value.Trim()),
                    FileType = FileType.Audio
                });
                return (false, currentQuestion);
            }

            // Check for image files
            if (text.EndsWith(".unknown", StringComparison.OrdinalIgnoreCase))
            {
                currentQuestion.Files.Add(new FileData
                {
                    FileName = text,
                    FileType = FileType.Image
                });
                return (false, currentQuestion);
            }

            // Append to current question content
            if (!string.IsNullOrEmpty(currentQuestion.NoiDung))
            {
                currentQuestion.NoiDung += htmlText + imgs;
            }
            else if (!string.IsNullOrEmpty(htmlText) || !string.IsNullOrEmpty(imgs))
            {
                currentQuestion.NoiDung = htmlText + imgs;
            }

            return (false, currentQuestion);
        }

        private async Task SaveQuestionsToDatabaseAsync(List<QuestionData> questions, Guid maPhan, string? mediaFolderPath, ImportResult result)
        {
            if (!questions.Any())
            {
                _logger.LogWarning("No questions to save.");
                result.Errors.Add("No questions found to save.");
                return;
            }

            var batches = questions.Chunk(BatchSize);

            using var transaction = await _cauHoiRepository.BeginTransactionAsync();
            try
            {
                foreach (var batch in batches)
                {
                    foreach (var question in batch)
                    {
                        try
                        {
                            var cauHoiDto = new CreateCauHoiWithAnswersDto
                            {
                                MaPhan = maPhan,
                                MaSoCauHoi = await GenerateMaSoCauHoiAsync(),
                                NoiDung = question.NoiDung,
                                HoanVi = question.Answers.Any(a => a.HoanVi),
                                CapDo = 1,
                                SoCauHoiCon = 0,
                                DoPhanCach = 0,
                                XoaTam = false,
                                SoLanDuocThi = 0,
                                SoLanDung = 0,
                                CLO = question.CLO,
                                CauTraLois = question.Answers.Select(a => new CreateCauTraLoiDto
                                {
                                    NoiDung = a.NoiDung,
                                    ThuTu = a.ThuTu,
                                    LaDapAn = a.LaDapAn,
                                    HoanVi = a.HoanVi,
                                    MaCauHoi = Guid.Empty
                                }).ToList(),
                                Files = question.Files.Select(f => new FileDto
                                {
                                    TenFile = f.FileName,
                                    LoaiFile = f.FileType,
                                    MaCauHoi = Guid.Empty
                                }).ToList()
                            };

                            _logger.LogInformation("Lưu câu hỏi: {NoiDung}",
                                cauHoiDto.NoiDung != null ? cauHoiDto.NoiDung.Substring(0, Math.Min(100, cauHoiDto.NoiDung.Length)) : string.Empty);

                            var addedCauHoi = await _cauHoiRepository.AddWithAnswersAsync(cauHoiDto);

                            foreach (var cauTraLoi in cauHoiDto.CauTraLois)
                            {
                                cauTraLoi.MaCauHoi = addedCauHoi.MaCauHoi;
                            }
                            foreach (var file in cauHoiDto.Files)
                            {
                                file.MaCauHoi = addedCauHoi.MaCauHoi;
                            }

                            await ProcessQuestionFiles(question, addedCauHoi, cauHoiDto, mediaFolderPath, result);
                            result.SuccessCount++;

                            _logger.LogInformation("Đã lưu thành công câu hỏi {MaCauHoi} với {AnswerCount} đáp án",
                                addedCauHoi.MaCauHoi, question.Answers.Count);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Lỗi khi lưu câu hỏi: {NoiDung}",
                                question.NoiDung != null ? question.NoiDung.Substring(0, Math.Min(50, question.NoiDung.Length)) : string.Empty);
                            result.Errors.Add($"Error saving question: {ex.Message}");
                        }
                    }
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Successfully saved all batches. Total questions: {SuccessCount}", result.SuccessCount);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error saving question batch: {Message}", ex.Message);
                result.Errors.Add($"Error saving question batch: {ex.Message}");
                throw;
            }
        }

        private async Task ProcessQuestionFiles(QuestionData question, dynamic addedCauHoi, CreateCauHoiWithAnswersDto cauHoiDto, string? mediaFolderPath, ImportResult result)
        {
            foreach (var file in question.Files)
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var destinationPath = Path.Combine(_storagePath, uniqueFileName);
                try
                {
                    if (file.ImageData != null)
                    {
                        await File.WriteAllBytesAsync(destinationPath, file.ImageData);
                        _logger.LogInformation($"Saved embedded image {file.FileName} for question {addedCauHoi.MaCauHoi}");
                    }
                    else if (!string.IsNullOrEmpty(mediaFolderPath))
                    {
                        var fullPath = Path.Combine(mediaFolderPath, file.FileName);
                        if (File.Exists(fullPath))
                        {
                            File.Copy(fullPath, destinationPath, true);
                            var relativePath = Path.Combine("media", uniqueFileName).Replace("\\", "/");
                            var fileDto = cauHoiDto.Files.FirstOrDefault(f => f.TenFile == file.FileName);
                            if (fileDto != null)
                            {
                                fileDto.TenFile = $"<img src=\"{relativePath}\" alt=\"Question Image\" style=\"max-width: 100%; height: auto;\" />";
                            }
                            _logger.LogInformation($"Uploaded file {file.FileName} for question {addedCauHoi.MaCauHoi}");
                        }
                        else
                        {
                            _logger.LogWarning("File does not exist: {FilePath}", fullPath);
                            result.Errors.Add($"File does not exist: {file.FileName}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No mediaFolderPath provided for non-embedded file: {FileName}", file.FileName);
                        result.Errors.Add($"No mediaFolderPath found for file: {file.FileName}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error saving file {file.FileName} for question {addedCauHoi.MaCauHoi}");
                    result.Errors.Add($"Error saving file {file.FileName}: {ex.Message}");
                }
            }
        }

        private async Task<int> GenerateMaSoCauHoiAsync()
        {
            var lastCauHoi = (await _cauHoiRepository.GetAllAsync()).OrderByDescending(c => c.MaSoCauHoi).FirstOrDefault();
            return (lastCauHoi?.MaSoCauHoi ?? 0) + 1;
        }
    }

    public class QuestionData
    {
        public string NoiDung { get; set; } = string.Empty;
        public EnumCLO CLO { get; set; }
        public List<AnswerData> Answers { get; set; } = new List<AnswerData>();
        public List<FileData> Files { get; set; } = new List<FileData>();
    }

    public class AnswerData
    {
        public string NoiDung { get; set; } = string.Empty;
        public int ThuTu { get; set; }
        public bool LaDapAn { get; set; }
        public bool HoanVi { get; set; }
    }

    public class FileData
    {
        public string FileName { get; set; } = string.Empty;
        public FileType FileType { get; set; }
        public byte[]? ImageData { get; set; }
    }

    public class ImportResult
    {
        public int SuccessCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}