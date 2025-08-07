using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
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
using BEQuestionBank.Shared.Helpers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SixLabors.ImageSharp;
using File = System.IO.File;

namespace BEQuestionBank.Core.Services
{
    public class WordImportService
    {
        private const bool USE_IMAGE_AS_BASE64 = false; // Thay đổi này sẽ quyết định có sử dụng Base64 hay không
        private readonly ICauHoiRepository _cauHoiRepository;
        private readonly ILogger<WordImportService> _logger;
        private const int BatchSize = 50;
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
        private ImportResult _result = new ImportResult();


        public WordImportService(ICauHoiRepository cauHoiRepository, ILogger<WordImportService> logger)
        {
            _cauHoiRepository = cauHoiRepository ?? throw new ArgumentNullException(nameof(cauHoiRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ImportResult> ImportQuestionsAsync(IFormFile wordFile, Guid maPhan, string? mediaFolderPath)
        {

            // Input validation
            if (wordFile.Length == 0)
            {
                _logger.LogWarning("Tệp Word không hợp lệ: Tệp trống hoặc rỗng.");
                _result.Errors.Add("Tệp Word là bắt buộc và không được để trống.");
                return _result;
            }

            if (wordFile.Length > MaxFileSize)
            {
                _logger.LogWarning("Tệp Word vượt quá giới hạn kích thước: {FileSize} byte.", wordFile.Length);
                _result.Errors.Add($"Tệp Word không được vượt quá {MaxFileSize / (1024 * 1024)}MB.");
                return _result;
            }

            if (!wordFile.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Định dạng tệp không hợp lệ: {FileName}. Cần có .docx.", wordFile.FileName);
                _result.Errors.Add("Tệp phải ở định dạng .docx.");
                return _result;
            }

            if (!string.IsNullOrEmpty(mediaFolderPath) && !Directory.Exists(mediaFolderPath))
            {
                _logger.LogWarning("Thư mục phương tiện không tồn tại: {MediaFolderPath}", mediaFolderPath);
                _result.Errors.Add("Thư mục phương tiện không tồn tại.");
                return _result;
            }

            try
            {
                var questions = await ParseWordFileAsync(wordFile, mediaFolderPath);
                await SaveQuestionsToDatabaseAsync(questions, maPhan, mediaFolderPath, _result);
                _logger.LogInformation("Đã nhập thành công {SuccessCount} câu hỏi có lỗi {ErrorCount}.",
                    _result.SuccessCount, _result.Errors.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi khi nhập câu hỏi từ tệp Word.");
                _result.Errors.Add($"Lỗi hệ thống trong quá trình nhập: {ex.Message}");
            }

            return _result;
        }

        public async Task<List<QuestionData>> ParseWordFileAsync(IFormFile wordFile, string? mediaFolderPath)
        {
            if (wordFile == null || wordFile.Length == 0)
                throw new ArgumentException("File Word không hợp lệ");

            string mediaFolder = mediaFolderPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media");
            if (!Directory.Exists(mediaFolder))
                Directory.CreateDirectory(mediaFolder);

            var questions = new List<QuestionData>();
            string cloPattern = @"\(CLO(\d+)\)";
            string breakTag = "[<br>]";
            string groupPattern = @"\s*\{<(\d+)>\}\s*–\s*\{<(\d+)>\}";
            string answerPrefixPattern = @"^[A-D]\.\s*"; // Regex để xóa A., B., C., D.

            // Copy file tạm
            string tempFile = Path.GetTempFileName();
            await using (var stream = File.Create(tempFile))
            {
                await wordFile.CopyToAsync(stream);
            }

            using (var doc = WordprocessingDocument.Open(tempFile, false))
            {
                var body = doc.MainDocumentPart.Document.Body;
                QuestionData currentQuestion = null;
                QuestionData currentGroup = null;
                bool inGroup = false;
                bool inGroupLead = false;
                var groupLeadContent = new StringBuilder();
                int expectedSubQuestions = 0;
                int currentSubQuestionNumber = 0;

                foreach (var para in body.Elements<Paragraph>())
                {
                    var paraText = para.InnerText.Trim();
                    if (string.IsNullOrEmpty(paraText))
                        continue;

                    // Bắt đầu nhóm
                    if (paraText == "[<sg>]")
                    {
                        inGroup = true;
                        inGroupLead = true;
                        currentGroup = new QuestionData
                        {
                            MaCauHoi = Guid.NewGuid(),
                            IsGroup = true,
                            CauHoiCon = new List<QuestionData>(),
                            Files = new List<FileData>()
                        };
                        continue;
                    }

                    // Kết thúc phần dẫn nhóm, bắt đầu các câu hỏi con
                    if (paraText == "[<egc>]" && inGroup)
                    {
                        inGroupLead = false;
                        currentGroup.NoiDungNhom = groupLeadContent.ToString();
                        groupLeadContent.Clear();
                        continue;
                    }

                    // Kết thúc nhóm
                    if (paraText == "[</sg>]" && inGroup)
                    {
                        if (currentQuestion != null)
                        {
                            currentGroup.CauHoiCon.Add(currentQuestion);
                            currentQuestion = null;
                        }
                        questions.Add(currentGroup);
                        currentGroup = null;
                        inGroup = false;
                        expectedSubQuestions = 0;
                        currentSubQuestionNumber = 0;
                        continue;
                    }

                    // Trong nhóm: phần dẫn
                    if (inGroup && inGroupLead)
                    {
                        groupLeadContent.AppendLine(ParagraphToHtml(para, doc, mediaFolder, ref currentGroup));
                        var groupMatch = Regex.Match(paraText, groupPattern);
                        if (groupMatch.Success)
                        {
                            int start = int.Parse(groupMatch.Groups[1].Value);
                            int end = int.Parse(groupMatch.Groups[2].Value);
                            expectedSubQuestions = end - start + 1;
                        }
                        continue;
                    }

                    // Trong nhóm: các câu hỏi con
                    if (inGroup && !inGroupLead)
                    {
                        // Nếu gặp [<br>] => kết thúc câu hỏi con hiện tại
                        if (paraText == breakTag)
                        {
                            if (currentQuestion != null)
                            {
                                currentGroup.CauHoiCon.Add(currentQuestion);
                                currentQuestion = null;
                            }
                            continue;
                        }

                        // Nếu là câu hỏi mới
                        if (Regex.IsMatch(paraText, @"\(<\d+>\)|\(CLO\d+\)"))
                        {
                            if (currentQuestion != null)
                            {
                                currentGroup.CauHoiCon.Add(currentQuestion);
                            }

                            // Lấy CLO
                            EnumCLO? cloEnum = null;
                            var cloMatch = Regex.Match(paraText, cloPattern);
                            if (cloMatch.Success && int.TryParse(cloMatch.Groups[1].Value, out int cloVal))
                                cloEnum = (EnumCLO)cloVal;

                            currentQuestion = new QuestionData
                            {
                                MaCauHoi = Guid.NewGuid(),
                                MaCauHoiCha = currentGroup.MaCauHoi,
                                CLO = cloEnum,
                                Answers = new List<AnswerData>(),
                                Files = new List<FileData>(),
                                NoiDung = ParagraphToHtml(para, doc, mediaFolder, ref currentGroup)
                            };
                            currentSubQuestionNumber++;
                            continue;
                        }

                        // Nếu là đáp án và có câu hỏi hiện tại
                        if (IsAnswerLine(paraText) && currentQuestion != null)
                        {
                            ProcessAnswerParagraph(para, doc, mediaFolder, currentQuestion, currentGroup);
                            continue;
                        }

                        // Nội dung phụ
                        if (currentQuestion != null)
                        {
                            currentQuestion.NoiDung += ParagraphToHtml(para, doc, mediaFolder, ref currentGroup);
                        }
                        continue;
                    }

                    // Ngoài nhóm: câu hỏi đơn
                    if (paraText == breakTag)
                    {
                        if (currentQuestion != null)
                        {
                            questions.Add(currentQuestion);
                            currentQuestion = null;
                        }
                        continue;
                    }

                    // Nếu là câu hỏi mới (đơn hoặc trong nhóm)
                    if (IsQuestionLine(paraText) || Regex.IsMatch(paraText, cloPattern))
                    {
                        if (currentQuestion != null)
                        {
                            questions.Add(currentQuestion);
                        }

                        EnumCLO? cloEnum = null;
                        var cloMatch = Regex.Match(paraText, cloPattern);
                        if (cloMatch.Success && int.TryParse(cloMatch.Groups[1].Value, out int cloVal))
                            cloEnum = (EnumCLO)cloVal;

                        // Sửa lỗi: tạo biến tạm, truyền vào ParagraphToHtml, sau đó gán lại cho currentQuestion
                        var tempQuestion = new QuestionData
                        {
                            MaCauHoi = Guid.NewGuid(),
                            MaCauHoiCha = Guid.Empty,
                            CLO = cloEnum,
                            Answers = new List<AnswerData>(),
                            Files = new List<FileData>(),
                        };
                        tempQuestion.NoiDung = ParagraphToHtml(para, doc, mediaFolder, ref tempQuestion);
                        currentQuestion = tempQuestion;
                        continue;
                    }

                    // Nếu là đáp án và có câu hỏi hiện tại
                    if (IsAnswerLine(paraText) && currentQuestion != null)
                    {
                        ProcessAnswerParagraph(para, doc, mediaFolder, currentQuestion, null);
                        continue;
                    }

                    // Nội dung phụ
                    if (currentQuestion != null)
                    {
                        currentQuestion.NoiDung += ParagraphToHtml(para, doc, mediaFolder, ref currentQuestion);
                    }
                }

                // Thêm câu hỏi cuối cùng nếu còn
                if (inGroup && currentGroup != null)
                {
                    if (currentQuestion != null)
                    {
                        currentGroup.CauHoiCon.Add(currentQuestion);
                    }
                    questions.Add(currentGroup);
                }
                else if (currentQuestion != null)
                {
                    questions.Add(currentQuestion);
                }

                File.Delete(tempFile);
                return questions;
            }
        }
            
        // Method mới để xử lý paragraph chứa đáp án
        private void ProcessAnswerParagraph(Paragraph para, WordprocessingDocument doc, string mediaFolder,
            QuestionData currentQuestion, QuestionData currentGroup)
        {
            var runs = para.Elements<Run>().ToList();
            var answerMap = new Dictionary<char, (List<Run> runs, bool isCorrect, bool isShuffle)>();

            // Duyệt qua tất cả runs để tìm và nhóm các đáp án
            for (int i = 0; i < runs.Count; i++)
            {
                var run = runs[i];
                string runText = run.InnerText?.Trim();

                // Tìm pattern A. B. C. D.
                var match = Regex.Match(runText ?? "", @"^([A-D])\.(.*)$");
                if (match.Success)
                {
                    char answerLetter = match.Groups[1].Value[0];
                    string restOfAnswer = match.Groups[2].Value.Trim();

                    // Kiểm tra định dạng
                    bool isCorrect = false;
                    bool isShuffle = false;

                    if (run.RunProperties != null)
                    {
                        if (run.RunProperties.Underline != null && run.RunProperties.Underline.Val != UnderlineValues.None)
                            isCorrect = true;
                        if (run.RunProperties.Italic != null)
                            isShuffle = true;
                    }

                    // Nếu đáp án chưa tồn tại, tạo mới
                    if (!answerMap.ContainsKey(answerLetter))
                    {
                        answerMap[answerLetter] = (new List<Run>(), isCorrect, isShuffle);
                    }

                    // Tạo run mới với nội dung đã loại bỏ prefix
                    var newRun = new Run();
                    if (!string.IsNullOrEmpty(restOfAnswer))
                    {
                        newRun.AppendChild(new Text(restOfAnswer));
                    }

                    // Copy properties nếu có
                    if (run.RunProperties != null)
                    {
                        newRun.RunProperties = (RunProperties)run.RunProperties.CloneNode(true);
                    }

                    answerMap[answerLetter].runs.Add(newRun);

                    // Cập nhật trạng thái correct/shuffle nếu run này có định dạng
                    if (isCorrect)
                        answerMap[answerLetter] = (answerMap[answerLetter].runs, true, answerMap[answerLetter].isShuffle);
                    if (isShuffle)
                        answerMap[answerLetter] = (answerMap[answerLetter].runs, answerMap[answerLetter].isCorrect, true);
                }
                else
                {
                    // Nếu không phải đáp án mới, kiểm tra xem có phải nội dung tiếp theo của đáp án trước không
                    // Tìm đáp án gần nhất phía trước
                    char? lastAnswerLetter = null;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        string prevRunText = runs[j].InnerText?.Trim();
                        var prevMatch = Regex.Match(prevRunText ?? "", @"^([A-D])\..*$");
                        if (prevMatch.Success)
                        {
                            lastAnswerLetter = prevMatch.Groups[1].Value[0];
                            break;
                        }
                    }

                    // Nếu tìm thấy đáp án trước đó và run hiện tại có nội dung
                    if (lastAnswerLetter.HasValue && !string.IsNullOrEmpty(runText) && answerMap.ContainsKey(lastAnswerLetter.Value))
                    {
                        // Kiểm tra định dạng cho run hiện tại
                        bool isCorrect = false;
                        bool isShuffle = false;

                        if (run.RunProperties != null)
                        {
                            if (run.RunProperties.Underline != null && run.RunProperties.Underline.Val != UnderlineValues.None)
                                isCorrect = true;
                            if (run.RunProperties.Italic != null)
                                isShuffle = true;
                        }

                        // Clone run thay vì sử dụng trực tiếp
                        var clonedRun = (Run)run.CloneNode(true);
                        answerMap[lastAnswerLetter.Value].runs.Add(clonedRun);

                        // Cập nhật trạng thái correct/shuffle
                        if (isCorrect)
                            answerMap[lastAnswerLetter.Value] = (answerMap[lastAnswerLetter.Value].runs, true, answerMap[lastAnswerLetter.Value].isShuffle);
                        if (isShuffle)
                            answerMap[lastAnswerLetter.Value] = (answerMap[lastAnswerLetter.Value].runs, answerMap[lastAnswerLetter.Value].isCorrect, true);
                    }
                }
            }

            // Tạo đáp án từ answerMap
            foreach (var kvp in answerMap.OrderBy(x => x.Key))
            {
                char answerLetter = kvp.Key;
                var answerData = kvp.Value;

                int orderNumber = char.ToUpper(answerLetter) - 'A' + 1;

                // Tạo paragraph từ runs đã clone
                var tempPara = new Paragraph();
                foreach (var run in answerData.runs)
                {
                    tempPara.AppendChild(run);
                }

                string answerContent = ParagraphToHtml(tempPara, doc, mediaFolder, ref currentQuestion);

                currentQuestion.Answers.Add(new AnswerData
                {
                    NoiDung = answerContent,
                    ThuTu = orderNumber,
                    LaDapAn = answerData.isCorrect,
                    HoanVi = answerData.isShuffle
                });
            }
        }

        private string ParagraphToHtml(Paragraph para, WordprocessingDocument doc, string mediaFolder, ref QuestionData question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question), "QuestionData không được null");

            if (question.Files == null)
                question.Files = new List<FileData>();

            var sb = new StringBuilder();
            var audioSb = new StringBuilder(); // StringBuilder riêng cho audio
            string cloPattern = @"\(CLO(\d+)\)";
            string audioPattern = @"\[<audio>\](.*?)\[</audio>\]";

            var mainPart = doc.MainDocumentPart;
            if (mainPart == null)
            {
                _logger.LogWarning("MainDocumentPart is null");
                return string.Empty;
            }

            // Danh sách định dạng ImageSharp hỗ trợ
            var supportedFormats = new HashSet<string>
                                    {
                                        "image/jpeg", "image/png", "image/gif", "image/bmp",
                                        "image/webp", "image/tiff", "image/x-tga", "image/x-portable-bitmap"
                                    };

            foreach (var run in para.Elements<Run>())
            {
                // ==== XỬ LÝ TEXT ====
                foreach (var textElement in run.Elements<Text>())
                {
                    string text = textElement.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        // Loại bỏ đoạn dạng (<1>), (<23>), v.v.
                        text = Regex.Replace(text, @"\(<\d+>\)", string.Empty);
                        sb.Append(text);
                    }
                }
            }


            // Thêm ảnh vào HTML
            sb.Append(ExtractImagesFromParagraph(para, doc, mediaFolder, ref question));


            string html = sb.ToString();

            // ==== XỬ LÝ AUDIO ====
            var audioMatches = Regex.Matches(html, audioPattern, RegexOptions.IgnoreCase);
            foreach (Match match in audioMatches)
            {
                if (match.Groups.Count > 1)
                {
                    string originalFileName = match.Groups[1].Value.Trim();
                    string destFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media");
                    if (!Directory.Exists(destFolder))
                        Directory.CreateDirectory(destFolder);

                    string extension = Path.GetExtension(originalFileName);
                    string newFileName = $"{Guid.NewGuid()}{extension}";
                    string destPath = Path.Combine(destFolder, newFileName);
                    string sourcePath = Path.Combine(mediaFolder, originalFileName);

                    try
                    {
                        if (File.Exists(sourcePath))
                        {
                            File.Copy(sourcePath, destPath, true);

                            question.Files.Add(new FileData
                            {
                                FileName = newFileName,
                                FileType = FileType.Audio
                            });

                            // Thêm HTML audio player giới hạn 3 lần nghe
                            audioSb.Append($@"
                                        <div>
                                            <audio id=""audio_{newFileName}"" controls preload=""none"">
                                                <source src=""/media/{newFileName}"" type=""audio/mpeg"">
                                            </audio>
                                            <script>
                                                (function() {{
                                                    const audio = document.getElementById('audio_{newFileName}');
                                                    let playCount = 0;
                                                    audio.addEventListener('play', function() {{
                                                        playCount++;
                                                        if (playCount > 3) {{
                                                            audio.pause();
                                                            audio.src = '';
                                                            audio.controls = false;
                                                            const msg = document.createElement('p');
                                                            msg.textContent = 'Bạn chỉ được nghe tối đa 3 lần.';
                                                            audio.parentNode.appendChild(msg);
                                                        }}
                                                    }});
                                                }})();
                                            </script>
                                        </div>");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Lỗi xử lý audio: {originalFileName}");
                    }

                    // XÓA thẻ [<audio>]...[/audio>] này khỏi HTML sau khi xử lý xong
                    html = html.Replace(match.Value, string.Empty);
                }
            }

            html = Regex.Replace(html, cloPattern, "").Trim();
            html = html.Replace("[<br>]", "").Trim();

            // Kết hợp nội dung chính và audio (audio nằm cuối)
            if (audioSb.Length > 0)
            {
                html = $"<span>{html}{audioSb}</span>";
            }
            else
            {
                html = $"<span>{html}</span>";
            }

            return html;
        }

        private string ExtractImagesFromParagraph(Paragraph para, WordprocessingDocument doc, string mediaFolder, ref QuestionData question)
        {
            var sb = new StringBuilder();
            var mainPart = doc.MainDocumentPart;


            // 1️⃣ Lấy tất cả ImageData trong VML Shape
            var shapes = para.Descendants<DocumentFormat.OpenXml.Vml.Shape>().ToList();
            foreach (var shape in shapes)
            {
                var imageData = shape.Descendants<DocumentFormat.OpenXml.Vml.ImageData>().FirstOrDefault();
                if (imageData?.RelationshipId?.Value != null)
                {
                    SaveImagePart(imageData.RelationshipId.Value, mainPart, mediaFolder, ref question, sb);
                }
            }

            // 2️⃣ Lấy tất cả ảnh trong Drawing (Blip)
            var blips = para.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().ToList();
            foreach (var blip in blips)
            {
                if (blip?.Embed?.Value != null)
                {
                    SaveImagePart(blip.Embed.Value, mainPart, mediaFolder, ref question, sb);
                }
            }

            return sb.ToString();
        }

        private void SaveImagePart(string relationshipId, MainDocumentPart mainPart, string mediaFolder, ref QuestionData question, StringBuilder sb)
        {
            if (!mainPart.Parts.Any(p => p.RelationshipId == relationshipId))
            {
                _logger.LogWarning($"ImagePart với relationshipId {relationshipId} không tồn tại.");
                return;
            }

            var imagePart = (ImagePart)mainPart.GetPartById(relationshipId);
            try
            {
                using var stream = imagePart.GetStream();
                using var img = System.Drawing.Image.FromStream(stream);

                // Resize nếu ảnh lớn hơn 480px
                System.Drawing.Image finalImg = img;
                if (img.Width > 480)
                {
                    finalImg = ImageHelper.ResizeImage(480, 320, img);
                }

                SaveImageToFileOrBase64(finalImg, imagePart.ContentType, ref question, sb);

                if (!ReferenceEquals(finalImg, img))
                    finalImg.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi đọc ảnh từ relationshipId {relationshipId}");
            }
        }

        private void SaveImageToFileOrBase64(System.Drawing.Image img, string contentType, ref QuestionData question, StringBuilder sb)
        {
            string ext = contentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/bmp" => ".bmp",
                "image/x-wmf" => ".png",
                "image/x-emf" => ".png",
                _ => ".png"
            };

            string fileName = $"{Guid.NewGuid()}{ext}";
            string imagesFolder = Path.Combine(_storagePath, "images");
            string filePath = Path.Combine(imagesFolder, fileName);

            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            if (!USE_IMAGE_AS_BASE64)
            {
                // Lưu ảnh luôn dưới dạng PNG thật
                img.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                question.Files.Add(new FileData
                {
                    FileName = fileName,
                    FileType = FileType.Image
                });

                sb.Append($"<img src=\"/images/{fileName}\" style=\"max-width:100%; height:auto;\" />");
            }
            else
            {
                using var ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                string base64 = Convert.ToBase64String(ms.ToArray());
                sb.Append($"<img src=\"data:image/png;base64,{base64}\" style=\"max-width:100%; height:auto;\" />");
            }
        }

        private bool IsQuestionLine(string text)
        {
            return Regex.IsMatch(text, @"\(CLO(\d+)\)");
        }

        private bool IsAnswerLine(string text)
        {
            text = text.Trim().ToUpper();
            var options = new[] { "A.", "B.", "C.", "D." };
            return options.Any(o => text.StartsWith(o));
        }

        private async Task<int> GenerateMaSoCauHoiAsync()
        {
            var lastCauHoi = (await _cauHoiRepository.GetAllAsync()).OrderByDescending(c => c.MaSoCauHoi)
                .FirstOrDefault();
            return (lastCauHoi?.MaSoCauHoi ?? 0) + 1;
        }

        public async Task SaveQuestionsToDatabaseAsync(List<QuestionData> questions, Guid maPhan, string? mediaFolderPath, ImportResult result)
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
                        try
                        {
                            if (question.IsGroup)
                            {
                                // Xử lý câu hỏi nhóm
                                var groupDto = new CreateCauHoiWithAnswersDto
                                {
                                    MaPhan = maPhan,
                                    MaSoCauHoi = await GenerateMaSoCauHoiAsync(),
                                    NoiDung = question.NoiDungNhom, // Nội dung nhóm
                                    HoanVi = false, // Nhóm không có đáp án nên HoanVi là false
                                    CapDo = 1,
                                    SoCauHoiCon = question.CauHoiCon.Count,
                                    DoPhanCach = 0,
                                    XoaTam = false,
                                    SoLanDuocThi = 0,
                                    SoLanDung = 0,
                                    CLO = null, // Nhóm không có CLO
                                    CauTraLois = new List<CreateCauTraLoiDto>(), // Nhóm không có đáp án
                                    Files = question.Files.Select(f => new FileDto
                                    {
                                        TenFile = f.FileName, // Sử dụng FileName thay vì TenFile
                                        LoaiFile = f.FileType, // Sử dụng FileType thay vì LoaiFile
                                        MaCauHoi = Guid.Empty
                                    }).ToList()
                                };

                                _logger.LogInformation("Đang lưu câu hỏi nhóm: {NoiDung}", groupDto.NoiDung);

                                // Lưu câu hỏi nhóm
                                var addedGroup = await _cauHoiRepository.AddWithAnswersAsync(groupDto);

                                if (addedGroup != null && addedGroup.MaCauHoi != Guid.Empty)
                                {
                                    // Cập nhật MaCauHoi cho files của nhóm
                                    foreach (var file in groupDto.Files)
                                    {
                                        file.MaCauHoi = addedGroup.MaCauHoi;
                                        _logger.LogInformation("Đã cập nhật MaCauHoi {MaCauHoi} cho file {TenFile} của nhóm",
                                            addedGroup.MaCauHoi, file.TenFile);
                                    }

                                    result.SuccessCount++;

                                    // Lưu các câu hỏi con
                                    foreach (var subQuestion in question.CauHoiCon)
                                    {
                                        try
                                        {
                                            var subQuestionDto = new CreateCauHoiWithAnswersDto
                                            {
                                                MaPhan = maPhan,
                                                MaSoCauHoi = await GenerateMaSoCauHoiAsync(),
                                                NoiDung = subQuestion.NoiDung,
                                                HoanVi = subQuestion.Answers.Any(a => a.HoanVi),
                                                CapDo = 1,
                                                SoCauHoiCon = 0,
                                                DoPhanCach = 0,
                                                XoaTam = false,
                                                SoLanDuocThi = 0,
                                                SoLanDung = 0,
                                                CLO = subQuestion.CLO,
                                                MaCauHoiCha = addedGroup.MaCauHoi, // Liên kết với nhóm
                                                CauTraLois = subQuestion.Answers.Select(a => new CreateCauTraLoiDto
                                                {
                                                    NoiDung = a.NoiDung,
                                                    ThuTu = a.ThuTu,
                                                    LaDapAn = a.LaDapAn,
                                                    HoanVi = a.HoanVi,
                                                    MaCauHoi = Guid.Empty
                                                }).ToList(),
                                                Files = subQuestion.Files.Select(f => new FileDto
                                                {
                                                    TenFile = f.FileName,
                                                    LoaiFile = f.FileType,
                                                    MaCauHoi = Guid.Empty
                                                }).ToList()
                                            };

                                            _logger.LogInformation("Đang lưu câu hỏi con: {NoiDung}", subQuestionDto.NoiDung);

                                            // Lưu câu hỏi con
                                            var addedSubQuestion = await _cauHoiRepository.AddWithAnswersAsync(subQuestionDto);

                                            if (addedSubQuestion != null && addedSubQuestion.MaCauHoi != Guid.Empty)
                                            {
                                                // Cập nhật MaCauHoi cho files và answers của câu hỏi con
                                                foreach (var file in subQuestionDto.Files)
                                                {
                                                    file.MaCauHoi = addedSubQuestion.MaCauHoi;
                                                    _logger.LogInformation("Đã cập nhật MaCauHoi {MaCauHoi} cho file {TenFile} của câu hỏi con",
                                                        addedSubQuestion.MaCauHoi, file.TenFile);
                                                }

                                                foreach (var cauTraLoi in subQuestionDto.CauTraLois)
                                                {
                                                    cauTraLoi.MaCauHoi = addedSubQuestion.MaCauHoi;
                                                }

                                                result.SuccessCount++;
                                            }
                                            else
                                            {
                                                _logger.LogError("Không thể lưu câu hỏi con: {NoiDung}", subQuestionDto.NoiDung);
                                                result.Errors.Add($"Không thể lưu câu hỏi con: {subQuestionDto.NoiDung}");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex, "Lỗi khi lưu câu hỏi con: {NoiDung}", subQuestion.NoiDung);
                                            result.Errors.Add($"Lỗi khi lưu câu hỏi con '{subQuestion.NoiDung}': {ex.Message}");
                                        }
                                    }
                                }
                                else
                                {
                                    _logger.LogError("Không thể lưu câu hỏi nhóm: {NoiDung}", groupDto.NoiDung);
                                    result.Errors.Add($"Không thể lưu câu hỏi nhóm: {groupDto.NoiDung}");
                                }
                            }
                            else
                            {
                                // Xử lý câu hỏi đơn
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

                                _logger.LogInformation("Đang lưu câu hỏi đơn: {NoiDung}", cauHoiDto.NoiDung);

                                // Lưu câu hỏi đơn
                                var addedCauHoi = await _cauHoiRepository.AddWithAnswersAsync(cauHoiDto);

                                if (addedCauHoi != null && addedCauHoi.MaCauHoi != Guid.Empty)
                                {
                                    // Cập nhật MaCauHoi cho files và answers
                                    foreach (var file in cauHoiDto.Files)
                                    {
                                        file.MaCauHoi = addedCauHoi.MaCauHoi;
                                        _logger.LogInformation("Đã cập nhật MaCauHoi {MaCauHoi} cho file {TenFile}",
                                            addedCauHoi.MaCauHoi, file.TenFile);
                                    }

                                    foreach (var cauTraLoi in cauHoiDto.CauTraLois)
                                    {
                                        cauTraLoi.MaCauHoi = addedCauHoi.MaCauHoi;
                                    }

                                    result.SuccessCount++;
                                }
                                else
                                {
                                    _logger.LogError("Không thể lưu câu hỏi đơn: {NoiDung}", cauHoiDto.NoiDung);
                                    result.Errors.Add($"Không thể lưu câu hỏi đơn: {cauHoiDto.NoiDung}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Lỗi khi lưu câu hỏi: {NoiDung}", question.NoiDung);
                            result.Errors.Add($"Lỗi khi lưu câu hỏi '{question.NoiDung}': {ex.Message}");
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

        /// <summary>
        /// Thêm method debug này để phân tích cấu trúc paragraph
        /// </summary>
        /// <param name="para"></param>
        private void DebugParagraphStructure(Paragraph para)
        {
            _logger.LogInformation($"=== ANALYZING PARAGRAPH ===");
            _logger.LogInformation($"Paragraph InnerText: '{para.InnerText}'");
            _logger.LogInformation($"Paragraph OuterXml: {para.OuterXml}");

            var runs = para.Elements<Run>().ToList();
            _logger.LogInformation($"Total Runs: {runs.Count}");

            for (int i = 0; i < runs.Count; i++)
            {
                var run = runs[i];
                _logger.LogInformation($"--- Run {i + 1} ---");
                _logger.LogInformation($"Run InnerText: '{run.InnerText}'");
                _logger.LogInformation($"Run OuterXml: {run.OuterXml}");

                // Kiểm tra các Text elements trong Run
                var textElements = run.Elements<Text>().ToList();
                _logger.LogInformation($"Text elements in run: {textElements.Count}");
                foreach (var textEl in textElements)
                {
                    _logger.LogInformation($"  Text: '{textEl.Text}'");
                }

                // Kiểm tra Drawing elements
                var drawings = run.Descendants<Drawing>().ToList();
                _logger.LogInformation($"Drawings in run: {drawings.Count}");

                if (drawings.Any())
                {
                    foreach (var drawing in drawings)
                    {
                        var blip = drawing.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().FirstOrDefault();
                        _logger.LogInformation($"  Drawing with embedId: {blip?.Embed?.Value}");
                    }
                }

                // Kiểm tra các elements khác
                _logger.LogInformation($"All child elements: {string.Join(", ", run.ChildElements.Select(e => e.GetType().Name))}");
            }
            _logger.LogInformation($"=== END ANALYSIS ===");
        }
        
    }

    public class QuestionData
    {
        public Guid MaCauHoi { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public EnumCLO? CLO { get; set; }
        public Guid? MaCauHoiCha { get; set; }
        public int? ThuTuTrongNhom { get; set; }
        public List<AnswerData> Answers { get; set; } = new List<AnswerData>();
        public List<FileData> Files { get; set; } = new List<FileData>();
        // --- Thêm cho nhóm câu hỏi ---
        public string NoiDungNhom { get; set; } = string.Empty;
        public bool IsGroup { get; set; } = false;
        public List<QuestionData> CauHoiCon { get; set; } = new List<QuestionData>();
    }

    public class FileData
    {
        public string FileName { get; set; } = string.Empty;
        public FileType FileType { get; set; } = FileType.Image; // Mặc định là hình ảnh
        public Guid? MaCauHoi { get; set; } = null; // Tham chiếu đến câu hỏi nếu có
        public Guid? MaCauTraLoi { get; set; } = null; // Tham chiếu đến câu trả lời nếu có
        public byte[] DataFile { get; set; } = Array.Empty<byte>(); // Dữ liệu file
    }

    public class AnswerData
    {
        public Guid MaCauHoi { get; set; } = Guid.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public int ThuTu { get; set; }
        public bool LaDapAn { get; set; }
        public bool HoanVi { get; set; }
    }

    public class ImportResult
    {
        public int SuccessCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}