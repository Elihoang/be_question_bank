using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using BEQuestionBank.Shared.DTOs.DeThi;

namespace BEQuestionBank.Shared.Helpers
{
    public static class HtmlConverter
    {
        public static async Task<MemoryStream> ExportWordTemplateAsync(
            DeThiWithChiTietAndCauTraLoiDto deThiDto,
            ExamTemplateParametersDto parameters = null)
        {
             var memoryStream = new MemoryStream();
            using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
            {
                // Tạo cấu trúc tài liệu Word
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Thêm tiêu đề đề thi
                Paragraph titlePara = body.AppendChild(new Paragraph());
                Run titleRun = titlePara.AppendChild(new Run());
                titleRun.AppendChild(new Text(parameters?.TenDeThi ?? deThiDto.TenDeThi ?? "Đề thi"));
                titlePara.ParagraphProperties = new ParagraphProperties(
                    new ParagraphStyleId { Val = "Heading1" },
                    new Justification { Val = JustificationValues.Center }
                );
                RunProperties titleRunProps = titleRun.AppendChild(new RunProperties());
                titleRunProps.Bold = new Bold();
                titleRunProps.FontSize = new FontSize { Val = "32" }; // Kích thước chữ 16pt

                // Thêm khoảng cách sau tiêu đề
                body.AppendChild(new Paragraph());

                // Thêm thông tin đề thi
                AddExamInfo(body, deThiDto, parameters);

                // Thêm danh sách câu hỏi và đáp án
                int cauHoiIndex = 1;
                foreach (var chiTiet in deThiDto.ChiTietDeThis)
                {
                    // Chuyển đổi nội dung câu hỏi thành văn bản thuần túy
                    string noiDungCauHoi = ConvertHtmlToPlainText(chiTiet.CauHoi.NoiDung ?? "N/A");

                    // Thêm câu hỏi
                    Paragraph questionPara = body.AppendChild(new Paragraph());
                    Run questionRun = questionPara.AppendChild(new Run());
                    questionRun.AppendChild(new Text($"Câu {cauHoiIndex} ({chiTiet.CauHoi.CLO?.ToString() ?? "N/A"}): {noiDungCauHoi}"));
                    questionPara.ParagraphProperties = new ParagraphProperties(
                        new ParagraphStyleId { Val = "Normal" },
                        new SpacingBetweenLines { Line = "360", LineRule = LineSpacingRuleValues.Auto }
                    );

                    // Thêm đáp án
                    // if (deThiDto.CauTraLoiByCauHoi.TryGetValue(chiTiet.MaCauHoi, out var cauTraLoiList))
                    // {
                    //     char dapAnLabel = 'A';
                    //     foreach (var cauTraLoi in cauTraLoiList)
                    //     {
                    //         string noiDungDapAn = ConvertHtmlToPlainText(cauTraLoi.NoiDung ?? "N/A");
                    //         Paragraph answerPara = body.AppendChild(new Paragraph());
                    //         Run answerRun = answerPara.AppendChild(new Run());
                    //         if (cauTraLoi.LaDapAn)
                    //         {
                    //             RunProperties runProps = answerRun.AppendChild(new RunProperties());
                    //             runProps.Bold = new Bold();
                    //         }
                    //         answerRun.AppendChild(new Text($"{dapAnLabel}. {noiDungDapAn}"));
                    //         answerPara.ParagraphProperties = new ParagraphProperties(
                    //             new Indentation { Left = "720" } // Thụt lề 1cm
                    //         );
                    //         dapAnLabel++;
                    //     }
                    // }

                    // Thêm khoảng cách giữa các câu hỏi
                    body.AppendChild(new Paragraph());
                    cauHoiIndex++;
                }

                // Lưu tài liệu
                mainPart.Document.Save();
            }

            memoryStream.Position = 0;
            return await Task.FromResult(memoryStream);
        }

        private static void AddExamInfo(Body body, DeThiWithChiTietAndCauTraLoiDto deThiDto, ExamTemplateParametersDto parameters)
        {
            // Thêm thông tin đề thi
            AddParagraph(body, $"Mã đề: {parameters?.MaDe ?? deThiDto.MaDeThi.ToString()}");
            AddParagraph(body, $"Môn thi: {parameters?.MonThi ?? deThiDto.TenMonHoc ?? "N/A"}");
            AddParagraph(body, $"Khoa: {parameters?.TenKhoa ?? deThiDto.TenKhoa ?? "N/A"}");
            AddParagraph(body, $"Số tín chỉ: {parameters?.SoTinChi ?? "N/A"}");
            AddParagraph(body, $"Học kỳ: {parameters?.HocKy ?? "N/A"}");
            AddParagraph(body, $"Lớp: {parameters?.Lop ?? "N/A"}");
            AddParagraph(body, $"Ngày thi: {parameters?.NgayThi ?? deThiDto.NgayTao.ToString("dd/MM/yyyy")}");
            AddParagraph(body, $"Thời gian làm bài: {parameters?.ThoiGianLam ?? "N/A"}");
            AddParagraph(body, $"Hình thức thi: {parameters?.HinhThuc ?? "N/A"}");
            AddParagraph(body, $"SỬ DỤNG TÀI LIỆU: {(parameters?.TaiLieuCo ?? false ? "CÓ" : "KHÔNG")}", isBold: true, isCentered: true, color: "FF0000");
        }

        private static void AddParagraph(Body body, string text, bool isBold = false, bool isCentered = false, string color = null)
        {
            Paragraph para = body.AppendChild(new Paragraph());
            Run run = para.AppendChild(new Run());
            if (isBold || color != null)
            {
                RunProperties runProps = run.AppendChild(new RunProperties());
                if (isBold) runProps.Bold = new Bold();
                if (color != null) runProps.Color = new Color { Val = color };
            }
            run.AppendChild(new Text(text));
            if (isCentered)
            {
                para.ParagraphProperties = new ParagraphProperties(
                    new Justification { Val = JustificationValues.Center }
                );
            }
        }

        private static string ConvertHtmlToPlainText(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return "N/A";

            // Thay thế thẻ <img>
            html = Regex.Replace(html, @"<[Ii][Mm][Gg][^>]*[Ss][Rr][Cc]\s*=\s*[""']?([^""']*)[""']?[^>]*>", 
                match => $"[Hình ảnh: {Path.GetFileName(match.Groups[1].Value)}]");

            // Thay thế thẻ <audio>
            html = Regex.Replace(html, @"<[Aa][Uu][Dd][Ii][Oo][^>]*>[^>]*[Ss][Rr][Cc]\s*=\s*[""']?([^""']*)[""']?[^>]*</[Aa][Uu][Dd][Ii][Oo]>", 
                match => $"[Âm thanh: {Path.GetFileName(match.Groups[1].Value)}]");

            // Loại bỏ các thẻ HTML khác
            html = Regex.Replace(html, @"<[^>]+>", "");
            html = Regex.Replace(html, @"<!\[CDATA\[.*?\]\]>", "");
            html = Regex.Replace(html, @"<!--.*?-->", "", RegexOptions.Singleline);

            // Thay thế các ký tự HTML đặc biệt
            var entities = new Dictionary<string, string>
            {
                { "&nbsp;", " " },
                { "&amp;", "&" },
                { "&quot;", "\"" },
                { "&lt;", "<" },
                { "&gt;", ">" },
                { "&ldquo;", "\"" },
                { "&rdquo;", "\"" },
                { "&ndash;", "-" },
                { "&mdash;", "-" },
                { "&bull;", "*" },
                { "&copy;", "(c)" },
                { "&reg;", "(r)" }
            };

            foreach (var entity in entities)
            {
                html = html.Replace(entity.Key, entity.Value);
            }

            // Loại bỏ các ký tự HTML không xác định
            html = Regex.Replace(html, @"&(.{2,6});", "");

            // Chuẩn hóa khoảng trắng
            html = Regex.Replace(html, @"\s{2,}", " ");
            html = html.Trim();

            return html;
        }
    }
}