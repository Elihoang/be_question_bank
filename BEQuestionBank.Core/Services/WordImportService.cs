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
            try
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
                        _logger.LogWarning("Empty or invalid image data for relId: {RelId}", relId);
                        continue;
                    }

                    string mimeType = imagePart.ContentType;
                    _logger.LogInformation("Processing image relId={RelId}, mimeType={MimeType}, size={Size} bytes", relId, mimeType, imageBytes.Length);

                    if (!mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Invalid image format: {ContentType}, relId: {RelId}", mimeType, relId);
                        continue;
                    }

                    if (mimeType.Equals("image/x-wmf", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            _logger.LogInformation("Converting WMF to PNG using ImageMagick.");
                            using var image = new MagickImage(imageBytes);
                            image.Format = MagickFormat.Png;
                            imageBytes = image.ToByteArray();
                            mimeType = "image/png";
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to convert WMF to PNG for relId: {RelId}", relId);
                            imageMapping[relId] = "<img src='placeholder.png' alt='Failed to convert WMF' />";
                            continue;
                        }
                    }

                    var base64Image = Convert.ToBase64String(imageBytes);
                    var imgTag = $@"<img src=""data:{mimeType};base64,{base64Image}"" alt=""Question Image"" style=""max-width: 100%; height: auto;"" />";
                    imageMapping[relId] = imgTag;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing images in Word document.");
                throw;
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

            var imgs = string.Join("",
                para.Descendants<DocumentFormat.OpenXml.Drawing.Blip>()
                    .Select(b => b.Embed?.Value)
                    .Where(relId => relId != null && imageMapping.ContainsKey(relId))
                    .Select(relId => imageMapping[relId])
            );

            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(imgs))
                return (false, currentQuestion);

            var cloMatch = cloRegex.Match(text);
            if (cloMatch.Success && Enum.TryParse<EnumCLO>($"CLO{cloMatch.Groups[1].Value}", out var clo))
            {
                var newQuestion = new QuestionData { CLO = clo };
                var questionText = text.Substring(cloMatch.Length).Trim();
                newQuestion.NoiDung = $"<p>{HttpUtility.HtmlEncode(questionText)}{imgs}</p>";
                return (true, newQuestion);
            }

            if (text.Length >= 2 && "ABCD".Contains(text[0]) && text[1] == '.')
            {
                var answerKey = text[0];
                var answerContent = text.Substring(2).Trim();

                bool isUnderline = para.Descendants<RunProperties>().Any(rp => rp.Underline != null);
                bool isItalic = para.Descendants<RunProperties>().Any(rp => rp.Italic != null);

                currentQuestion.Answers.Add(new AnswerData
                {
                    ThuTu = currentQuestion.Answers.Count + 1,
                    NoiDung = $"<p>{HttpUtility.HtmlEncode(answerContent)}{imgs}</p>",
                    LaDapAn = isUnderline,
                    HoanVi = isItalic
                });
                return (false, currentQuestion);
            }

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

            if (text.EndsWith(".unknown", StringComparison.OrdinalIgnoreCase))
            {
                currentQuestion.Files.Add(new FileData
                {
                    FileName = text,
                    FileType = FileType.Image
                });
                return (false, currentQuestion);
            }

            if (!string.IsNullOrEmpty(currentQuestion.NoiDung))
            {
                currentQuestion.NoiDung += htmlText + imgs;
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
                await Parallel.ForEachAsync(batches, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (batch, ct) =>
                {
                    foreach (var question in batch)
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

                        _logger.LogInformation("Saving question: {NoiDung}", cauHoiDto.NoiDung != null ? cauHoiDto.NoiDung.Substring(0, Math.Min(50, cauHoiDto.NoiDung.Length)) : string.Empty);

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
                        lock (result) // Thread-safe increment
                        {
                            result.SuccessCount++;
                        }
                    }
                });

                await transaction.CommitAsync();
                _logger.LogInformation("Successfully saved all batches. Total questions: {SuccessCount}", result.SuccessCount);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error saving question batch: {Message}", ex.Message);
                result.Errors.Add($"Error saving question batch: {ex.Message}");
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
                        _logger.LogInformation("Saved embedded image {FileName} for question {MaCauHoi}");
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
                            _logger.LogInformation("Uploaded file {FileName} for question {MaCauHoi}");
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
                    _logger.LogError(ex, "Error saving file {FileName} for question {MaCauHoi}");
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