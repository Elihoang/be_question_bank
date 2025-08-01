using Aspose.Words;
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

namespace BEQuestionBank.Core.Services
{
    public class WordImportService
    {
        private readonly ICauHoiRepository _cauHoiRepository;
        private readonly ILogger<WordImportService> _logger;
        private readonly string _storagePath; // Đường dẫn lưu trữ tệp (ví dụ: wwwroot/media)
        private const int BatchSize = 50; // Xử lý 50 câu hỏi mỗi lô
        private const long MaxFileSize = 10 * 1024 * 1024; // Giới hạn 10MB

        public WordImportService(ICauHoiRepository cauHoiRepository, string storagePath, ILogger<WordImportService> logger)
        {
            _cauHoiRepository = cauHoiRepository ?? throw new ArgumentNullException(nameof(cauHoiRepository));
            _storagePath = storagePath ?? throw new ArgumentNullException(nameof(storagePath));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ImportResult> ImportQuestionsAsync(IFormFile wordFile, Guid maPhan, string? mediaFolderPath)
        {
            var result = new ImportResult();

            // Kiểm tra đầu vào
            if (wordFile == null || wordFile.Length == 0)
            {
                _logger.LogWarning("Tệp Word không hợp lệ.");
                result.Errors.Add("Tệp Word là bắt buộc và không được rỗng.");
                return result;
            }

            if (wordFile.Length > MaxFileSize)
            {
                _logger.LogWarning("Tệp Word vượt quá kích thước cho phép: {FileSize} bytes.", wordFile.Length);
                result.Errors.Add($"Tệp Word không được vượt quá {MaxFileSize / (1024 * 1024)}MB.");
                return result;
            }

            if (!wordFile.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Tệp không phải định dạng .docx: {FileName}", wordFile.FileName);
                result.Errors.Add("Tệp phải có định dạng .docx.");
                return result;
            }

            if (!string.IsNullOrEmpty(mediaFolderPath) && !Directory.Exists(mediaFolderPath))
            {
                _logger.LogWarning("Thư mục media không tồn tại: {MediaFolderPath}", mediaFolderPath);
                result.Errors.Add("Thư mục chứa tệp media không tồn tại.");
                return result;
            }

            try
            {
                var questions = await ParseWordFileAsync(wordFile);
                await SaveQuestionsToDatabaseAsync(questions, maPhan, mediaFolderPath, result);
                _logger.LogInformation("Nhập {SuccessCount} câu hỏi thành công, {ErrorCount} lỗi.", result.SuccessCount, result.Errors.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi nhập câu hỏi từ file Word.");
                result.Errors.Add("Đã xảy ra lỗi hệ thống khi nhập câu hỏi: " + ex.Message);
            }

            return result;
        }

        private async Task<List<QuestionData>> ParseWordFileAsync(IFormFile wordFile)
        {
            var questions = new List<QuestionData>();
            using var memoryStream = new MemoryStream();
            await wordFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var document = new Document(memoryStream);
            var cloRegex = new Regex(@"\(CLO(\d+)\)");
            var audioRegex = new Regex(@"\[<audio>\](.*?)\[</audio>\]");
            var currentQuestion = new QuestionData();
            var paragraphs = document.GetChildNodes(NodeType.Paragraph, true).Cast<Paragraph>();

            // Trích xuất hình ảnh nhúng và tạo mapping với vị trí
            var imageMapping = new Dictionary<Shape, string>();
            var images = document.GetChildNodes(NodeType.Shape, true)
                .Cast<Shape>()
                .Where(s => s.HasImage)
                .ToList();

            foreach (var shape in images)
            {
                var imageBytes = shape.ImageData.ImageBytes;
                var mimeType = GetMimeType(shape.ImageData.ImageType); // Hàm xác định MIME type
                var base64Image = Convert.ToBase64String(imageBytes);
                var imgTag = $"<img src=\"data:{mimeType};base64,{base64Image}\" alt=\"Question Image\" style=\"max-width: 100%; height: auto;\" />";
                imageMapping[shape] = imgTag;
                _logger.LogInformation("Đã tạo base64 cho hình ảnh nhúng, MIME: {MimeType}", mimeType);
            }

            foreach (var paragraph in paragraphs)
            {
                var runs = paragraph.GetChildNodes(NodeType.Run, true).Cast<Run>();
                var shapes = paragraph.GetChildNodes(NodeType.Shape, true).Cast<Shape>();
                var text = string.Join("", runs.Select(r => r.GetText()).Where(t => !string.IsNullOrWhiteSpace(t))).Trim();

                // Tạo nội dung HTML cho đoạn văn
                var htmlContent = BuildHtmlContent(paragraph, imageMapping);

                if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(htmlContent)) continue;

                _logger.LogDebug("Phân tích đoạn văn: {Text}", text);

                // Xác định câu hỏi mới bằng CLO
                var cloMatch = cloRegex.Match(text);
                if (cloMatch.Success && Enum.TryParse<EnumCLO>($"CLO{cloMatch.Groups[1].Value}", out var clo))
                {
                    if (!string.IsNullOrEmpty(currentQuestion.NoiDung))
                    {
                        questions.Add(currentQuestion);
                        currentQuestion = new QuestionData();
                    }
                    currentQuestion.CLO = clo;

                    // Lấy nội dung sau CLO
                    var questionText = text.Substring(cloMatch.Length).Trim();
                    var questionHtml = !string.IsNullOrEmpty(questionText) ? $"<p>{HttpUtility.HtmlEncode(questionText)}</p>" : "";

                    // Thêm hình ảnh ngay sau nội dung CLO nếu có
                    var paragraphImages = GetImagesInParagraph(paragraph, imageMapping);
                    currentQuestion.NoiDung = questionHtml + paragraphImages;

                    _logger.LogInformation("Đã tìm thấy câu hỏi: {NoiDung}", currentQuestion.NoiDung);
                    continue;
                }
                // Xác định đáp án với định dạng A., B., C., D.
                var answerInfo = ParseAnswerFromParagraph(paragraph, runs);
                if (answerInfo != null)
                {
                    // Wrap nội dung đáp án trong thẻ p và mã hóa HTML
                    var answerHtml = $"<p>{HttpUtility.HtmlEncode(answerInfo.Content)}</p>";
                    var answerImages = GetImagesInParagraph(paragraph, imageMapping);

                    var answer = new AnswerData
                    {
                        NoiDung = answerHtml + answerImages,
                        ThuTu = currentQuestion.Answers.Count + 1,
                        LaDapAn = answerInfo.IsCorrect,
                        HoanVi = answerInfo.IsItalic
                    };
                    currentQuestion.Answers.Add(answer);
                    _logger.LogInformation("Đã thêm đáp án: {Key} {NoiDung}, LaDapAn: {LaDapAn}, HoanVi: {HoanVi}",
                        answerInfo.Key, answer.NoiDung, answer.LaDapAn, answer.HoanVi);
                    continue;
                }

                // Xác định tệp âm thanh
                var audioMatch = audioRegex.Match(text);
                if (audioMatch.Success)
                {
                    var audioPath = audioMatch.Groups[1].Value.Trim();
                    currentQuestion.Files.Add(new FileData { FileName = Path.GetFileName(audioPath), FileType = FileType.Audio });
                    _logger.LogInformation("Đã tìm thấy tệp âm thanh: {FileName}", audioPath);
                    continue;
                }

                // Xác định tệp hình ảnh tham chiếu
                if (text.EndsWith(".unknown", StringComparison.OrdinalIgnoreCase))
                {
                    currentQuestion.Files.Add(new FileData { FileName = text, FileType = FileType.Image });
                    _logger.LogInformation("Đã tìm thấy tệp hình ảnh tham chiếu: {FileName}", text);
                    continue;
                }

                // Nếu là nội dung thường, thêm vào câu hỏi hiện tại với thẻ p
                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(currentQuestion.NoiDung))
                {
                    var additionalContent = $"<p>{text}</p>" + GetImagesInParagraph(paragraph, imageMapping);
                    currentQuestion.NoiDung += additionalContent;
                }
            }

            if (!string.IsNullOrEmpty(currentQuestion.NoiDung))
            {
                questions.Add(currentQuestion);
                _logger.LogInformation("Đã thêm câu hỏi cuối: {NoiDung}", currentQuestion.NoiDung);
            }

            if (!questions.Any())
            {
                _logger.LogWarning("Không có câu hỏi nào hợp lệ sau khi phân tích.");
                return new List<QuestionData>();
            }

            _logger.LogInformation("Tổng số câu hỏi hợp lệ: {Count}", questions.Count);
            return questions;
        }

        private string BuildHtmlContent(Paragraph paragraph, Dictionary<Shape, string> imageMapping)
        {
            var content = new StringBuilder();
            var runs = paragraph.GetChildNodes(NodeType.Run, true).Cast<Run>();
            var text = string.Join("", runs.Select(r => r.GetText()).Where(t => !string.IsNullOrWhiteSpace(t))).Trim();

            if (!string.IsNullOrEmpty(text))
            {
                // Mã hóa HTML để xử lý ký tự đặc biệt
                content.Append($"<p>{HttpUtility.HtmlEncode(text)}</p>");
            }

            // Thêm hình ảnh trong đoạn văn
            content.Append(GetImagesInParagraph(paragraph, imageMapping));

            return content.ToString();
        }

        private string GetImagesInParagraph(Paragraph paragraph, Dictionary<Shape, string> imageMapping)
        {
            var imageHtml = new StringBuilder();
            var shapes = paragraph.GetChildNodes(NodeType.Shape, true).Cast<Shape>();

            foreach (var shape in shapes)
            {
                if (imageMapping.TryGetValue(shape, out var imgTag))
                {
                    imageHtml.Append(imgTag);
                }
            }

            return imageHtml.ToString();
        }

        private AnswerInfo ParseAnswerFromParagraph(Paragraph paragraph, IEnumerable<Run> runs)
        {
            var answerTextBuilder = new StringBuilder();
            char? answerKey = null;
            bool isCorrectAnswer = false;
            bool isAnswer = false;

            foreach (var run in runs)
            {
                var runText = run.GetText().Trim();

                // Nhận dạng dòng bắt đầu bằng "A.", "B.",...
                if (runText.Length >= 2 && "ABCD".Contains(runText[0]) && runText[1] == '.')
                {
                    answerKey = runText[0];
                    answerTextBuilder.Clear();
                    isAnswer = true;

                    // Kiểm tra xem đáp án có được gạch chân hoặc in đậm không để xác định đáp án đúng
                    isCorrectAnswer = run.Font.Underline != Underline.None || run.Font.Bold;

                    // Nếu có nội dung phía sau "A.", thêm luôn
                    if (runText.Length > 2)
                    {
                        answerTextBuilder.Append(runText.Substring(2).TrimStart());
                    }
                    continue;
                }

                // Nếu đang trong đáp án, tiếp tục gom nội dung
                if (isAnswer)
                {
                    answerTextBuilder.Append(" " + runText);
                }
            }

            if (isAnswer && answerKey != null)
            {
                var answerText = answerTextBuilder.ToString().Trim();
                var isItalic = runs.Any(r => r.Font.Italic);

                return new AnswerInfo
                {
                    Key = answerKey.Value,
                    Content = answerText,
                    IsCorrect = isCorrectAnswer || answerText.Contains("Correct", StringComparison.OrdinalIgnoreCase),
                    IsItalic = isItalic
                };
            }

            return null;
        }

        // Helper class để lưu thông tin đáp án
        private class AnswerInfo
        {
            public char Key { get; set; }
            public string Content { get; set; }
            public bool IsCorrect { get; set; }
            public bool IsItalic { get; set; }
        } 
        
        private async Task SaveQuestionsToDatabaseAsync(List<QuestionData> questions, Guid maPhan, string? mediaFolderPath, ImportResult result)
{
    if (!questions.Any())
    {
        _logger.LogWarning("Danh sách câu hỏi rỗng, không có dữ liệu để lưu.");
        result.Errors.Add("Không có câu hỏi nào để lưu.");
        return;
    }

    var batches = questions.Chunk(BatchSize);

    // Sử dụng giao dịch toàn cục
    using var transaction = await _cauHoiRepository.BeginTransactionAsync();
    try
    {
        foreach (var batch in batches)
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

                // Ghi log để theo dõi
                _logger.LogInformation("Đang lưu câu hỏi: {NoiDung}", cauHoiDto.NoiDung);

                var addedCauHoi = await _cauHoiRepository.AddWithAnswersAsync(cauHoiDto);

                foreach (var cauTraLoi in cauHoiDto.CauTraLois)
                {
                    cauTraLoi.MaCauHoi = addedCauHoi.MaCauHoi;
                }
                foreach (var file in cauHoiDto.Files)
                {
                    file.MaCauHoi = addedCauHoi.MaCauHoi;
                }

                foreach (var file in question.Files)
                {
                    var destinationPath = Path.Combine(_storagePath, file.FileName);
                    try
                    {
                        if (file.ImageData != null)
                        {
                            // Lưu hình ảnh nhúng
                            await File.WriteAllBytesAsync(destinationPath, file.ImageData);
                            result.SuccessCount++;
                            _logger.LogInformation("Lưu hình ảnh nhúng {FileName} cho câu hỏi {MaCauHoi}", file.FileName, addedCauHoi.MaCauHoi);
                        }
                        else if (!string.IsNullOrEmpty(mediaFolderPath))
                        {
                            // Lưu tệp hình ảnh không nhúng và tạo thẻ <img>
                            var fullPath = Path.Combine(mediaFolderPath, file.FileName);
                            if (File.Exists(fullPath))
                            {
                                File.Copy(fullPath, destinationPath, true);
                                var relativePath = Path.Combine("media", file.FileName).Replace("\\", "/");
                                // Cập nhật FileName thành thẻ <img> để lưu vào cơ sở dữ liệu
                                cauHoiDto.Files.FirstOrDefault(f => f.TenFile == file.FileName).TenFile = 
                                    $"<img src=\"{relativePath}\" alt=\"Question Image\" style=\"max-width: 100%; height: auto;\" />";
                                result.SuccessCount++;
                                _logger.LogInformation("Tải lên tệp {FileName} cho câu hỏi {MaCauHoi}", file.FileName, addedCauHoi.MaCauHoi);
                            }
                            else
                            {
                                _logger.LogWarning("Tệp không tồn tại: {FilePath}", fullPath);
                                result.Errors.Add($"Tệp không tồn tại: {file.FileName}");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Không có mediaFolderPath cho tệp không nhúng: {FileName}", file.FileName);
                            result.Errors.Add($"Không tìm thấy mediaFolderPath cho tệp: {file.FileName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi lưu tệp {FileName} cho câu hỏi {MaCauHoi}", file.FileName, addedCauHoi.MaCauHoi);
                        result.Errors.Add($"Lỗi khi lưu tệp {file.FileName}: {ex.Message}");
                    }
                }
            }
        }
        await transaction.CommitAsync();
        _logger.LogInformation("Lưu tất cả batch thành công. Tổng số câu hỏi: {SuccessCount}", result.SuccessCount);
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        _logger.LogError(ex, "Lỗi khi lưu batch câu hỏi: {Message}", ex.Message);
        result.Errors.Add($"Lỗi khi lưu batch câu hỏi: {ex.Message}");
    }
}

        private async Task<int> GenerateMaSoCauHoiAsync()
        {
            var lastCauHoi = (await _cauHoiRepository.GetAllAsync()).OrderByDescending(c => c.MaSoCauHoi).FirstOrDefault();
            return (lastCauHoi?.MaSoCauHoi ?? 0) + 1;
        }

        private string GetMimeType(ImageType imageType)
        {
            switch (imageType)
            {
                case ImageType.Png: return "image/png";
                case ImageType.Jpeg: return "image/jpeg";
                case ImageType.Bmp: return "image/bmp";
                case ImageType.Gif: return "image/gif";
                default: return "image/png"; // Mặc định nếu không xác định được
            }
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
        public byte[]? ImageData { get; set; } // Dữ liệu hình ảnh nhúng
    }

    public class ImportResult
    {
        public int SuccessCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}