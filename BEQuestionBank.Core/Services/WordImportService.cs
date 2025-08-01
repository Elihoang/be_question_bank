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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            int imageIndex = 0;

            // Trích xuất hình ảnh nhúng
            var images = document.GetChildNodes(NodeType.Shape, true)
                .Cast<Shape>()
                .Where(s => s.HasImage)
                .ToList();
            var imageFiles = new List<FileData>();
            foreach (var shape in images)
            {
                var imageFileName = $"image_{Guid.NewGuid()}.png";
                imageFiles.Add(new FileData
                {
                    FileName = imageFileName,
                    FileType = FileType.Image,
                    ImageData = shape.ImageData.ImageBytes
                });
                _logger.LogInformation("Đã trích xuất hình ảnh nhúng: {FileName}", imageFileName);
            }

            foreach (var paragraph in paragraphs)
            {
                var runs = paragraph.GetChildNodes(NodeType.Run, true).Cast<Run>();
                var text = string.Join("", runs.Select(r => r.GetText()).Where(t => !string.IsNullOrWhiteSpace(t))).Trim();
                if (string.IsNullOrEmpty(text)) continue;

                _logger.LogDebug("Phân tích đoạn văn: {Text}", text);

                // Xác định câu hỏi mới bằng CLO
                var cloMatch = cloRegex.Match(text);
                if (cloMatch.Success && Enum.TryParse<EnumCLO>($"CLO{cloMatch.Groups[1].Value}", out var clo))
                {
                    if (!string.IsNullOrEmpty(currentQuestion.NoiDung))
                    {
                        currentQuestion.Files.AddRange(imageFiles.Where(f => !questions.Any(q => q.Files.Contains(f))));
                        questions.Add(currentQuestion);
                        currentQuestion = new QuestionData();
                    }
                    currentQuestion.CLO = clo;
                    currentQuestion.NoiDung = text.Substring(cloMatch.Length).Trim();
                    _logger.LogInformation("Đã tìm thấy câu hỏi: {NoiDung}", currentQuestion.NoiDung);
                    continue;
                }

                // Xác định đáp án với định dạng A., B., C., D.
                bool isAnswer = false;
                var answerTextBuilder = new System.Text.StringBuilder();
                char? answerKey = null;
                bool isCorrectAnswer = false; // Biến để xác định đáp án đúng
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
                    var answer = new AnswerData
                    {
                        NoiDung = answerText,
                        ThuTu = currentQuestion.Answers.Count + 1,
                        LaDapAn = isCorrectAnswer || answerText.Contains("Correct", StringComparison.OrdinalIgnoreCase),
                        HoanVi = isItalic
                    };
                    currentQuestion.Answers.Add(answer);
                    _logger.LogInformation("Đã thêm đáp án: {Key}_ {NoiDung}, LaDapAn: {LaDapAn}, HoanVi: {HoanVi}",
                        answerKey, answer.NoiDung, answer.LaDapAn, answer.HoanVi);
                    isAnswer = false;
                    answerKey = null;
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
                }
            }

            if (!string.IsNullOrEmpty(currentQuestion.NoiDung))
            {
                currentQuestion.Files.AddRange(imageFiles.Where(f => !questions.Any(q => q.Files.Contains(f))));
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
                                    await File.WriteAllBytesAsync(destinationPath, file.ImageData);
                                    result.SuccessCount++;
                                    _logger.LogInformation("Lưu hình ảnh nhúng {FileName} cho câu hỏi {MaCauHoi}", file.FileName, addedCauHoi.MaCauHoi);
                                }
                                else if (!string.IsNullOrEmpty(mediaFolderPath))
                                {
                                    var fullPath = Path.Combine(mediaFolderPath, file.FileName);
                                    if (File.Exists(fullPath))
                                    {
                                        File.Copy(fullPath, destinationPath, true);
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