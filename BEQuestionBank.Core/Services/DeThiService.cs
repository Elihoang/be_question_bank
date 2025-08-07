using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.DeThi;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Shared.DTOs.CauTraLoi;
using BEQuestionBank.Shared.DTOs.ChiTietDeThi;
using BEQuestionBank.Shared.Helpers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using FontSize = DocumentFormat.OpenXml.Spreadsheet.FontSize;

namespace BEQuestionBank.Core.Services
{
    public class DeThiService : IDeThiService
    {
        private readonly IDeThiRepository _deThiRepository;
        private readonly IYeuCauRutTrichRepository _yeuCauRutTrichRepository;
        private readonly ICauHoiRepository _cauHoiRepository;
        private readonly ICauTraLoiRepository _cauTraLoiRepository;

        public DeThiService(IDeThiRepository deThiRepository, IYeuCauRutTrichRepository yeuCauRutTrichRepository, ICauHoiRepository cauHoiRepository, ICauTraLoiRepository cauTraLoiRepository)
        {
            _deThiRepository = deThiRepository;
            _yeuCauRutTrichRepository = yeuCauRutTrichRepository;
            _cauHoiRepository = cauHoiRepository;
            _cauTraLoiRepository = cauTraLoiRepository;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
        
        public Task<DeThi> GetByIdAsync(Guid id)
        {
            return _deThiRepository.GetByIdAsync(id);
        }
        
        public Task<IEnumerable<DeThi>> GetAllAsync()
        {
            return _deThiRepository.GetAllAsync();
        }

        public Task<IEnumerable<DeThi>> FindAsync(Expression<Func<DeThi, bool>> predicate)
        {
            return _deThiRepository.FindAsync(predicate);
        }

        public Task<DeThi> FirstOrDefaultAsync(Expression<Func<DeThi, bool>> predicate)
        {
            return _deThiRepository.FirstOrDefaultAsync(predicate);
        }

        public Task AddAsync(DeThi entity)
        {
            return _deThiRepository.AddAsync(entity);
        }

        public Task UpdateAsync(DeThi entity)
        {
            return _deThiRepository.UpdateAsync(entity);
        }

        public Task DeleteAsync(DeThi entity)
        {
            return _deThiRepository.DeleteAsync(entity);
        }

        public Task<bool> ExistsAsync(Expression<Func<DeThi, bool>> predicate)
        {
            return _deThiRepository.ExistsAsync(predicate);
        }

        public Task<DeThiDto> GetByIdWithChiTietAsync(Guid id)
        {
            return _deThiRepository.GetByIdWithChiTietAsync(id);
        }

        public Task<IEnumerable<DeThiDto>> GetAllWithChiTietAsync()
        {
            return _deThiRepository.GetAllWithChiTietAsync();
        }

        public Task<DeThiDto> AddWithChiTietAsync(DeThiDto deThiDto)
        {
            return _deThiRepository.AddWithChiTietAsync(deThiDto);
        }

        public Task<DeThiDto> UpdateWithChiTietAsync(DeThiDto deThiDto)
        {
            return _deThiRepository.UpdateWithChiTietAsync(deThiDto);
        }

        public async Task<IEnumerable<DeThi>> GetByMaMonHocAsync(Guid maMonHoc)
        {
            return await _deThiRepository.GetByMaMonHocAsync(maMonHoc);
        }

        public async Task<IEnumerable<DeThiDto>> GetApprovedDeThisAsync()
        {
            return await _deThiRepository.GetApprovedDeThisAsync();
        }
        
      public async Task<DeThiDto> ImportMaTranFromExcelAsync(Guid maYeuCau, IFormFile excelFile)
{
    // Kiểm tra file Excel
    if (excelFile == null || excelFile.Length == 0)
        throw new ArgumentException("File Excel không hợp lệ hoặc trống.");
    if (!excelFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Chỉ hỗ trợ file Excel định dạng .xlsx.");

    // Lấy thông tin yêu cầu rút trích
    var yeuCau = await _yeuCauRutTrichRepository.GetByIdAsync(maYeuCau);
    if (yeuCau == null)
        throw new ArgumentException($"Không tìm thấy yêu cầu rút trích với mã {maYeuCau}.");
    var maMonHoc = yeuCau.MaMonHoc;
    if (maMonHoc == Guid.Empty)
        throw new ArgumentException("Yêu cầu rút trích không chứa mã môn học hợp lệ.");

    var chiTietDeThis = new List<ChiTietDeThiDto>();
    int totalCauHoi = 0;
    var maDeThi = Guid.NewGuid(); // Tạo MaDeThi sớm

    // Đọc file Excel
    using (var stream = excelFile.OpenReadStream())
    using (var package = new ExcelPackage(stream))
    {
        var worksheet = package.Workbook.Worksheets[0];
        int rowCount = worksheet.Dimension?.Rows ?? 0;
        if (rowCount < 2)
            throw new ArgumentException("File Excel không chứa dữ liệu hợp lệ.");

        var usedCauHoiIds = new HashSet<Guid>(); // Ngăn trùng lặp MaCauHoi

        for (int row = 2; row <= rowCount; row++)
        {
            // Lấy giá trị CLO
            string cloText = worksheet.Cells[row, 1].Text?.Trim();
            if (string.IsNullOrWhiteSpace(cloText))
                continue;

            // Thử ánh xạ CLO
            EnumCLO clo;
            if (!Enum.TryParse<EnumCLO>(cloText, true, out clo))
            {
                if (int.TryParse(cloText, out var cloNumber))
                {
                    if (!Enum.TryParse<EnumCLO>($"CLO{cloNumber}", true, out clo))
                        throw new ArgumentException($"CLO không hợp lệ tại dòng {row}: {cloText}. Vui lòng sử dụng giá trị như '1', '2' hoặc 'CLO1', 'CLO2'.");
                }
                else
                {
                    throw new ArgumentException($"CLO không hợp lệ tại dòng {row}: {cloText}. Vui lòng sử dụng giá trị như '1', '2' hoặc 'CLO1', 'CLO2'.");
                }
            }

            // Lấy số lượng câu hỏi
            string soCauHoiText = worksheet.Cells[row, 2].Text?.Trim();
            if (string.IsNullOrWhiteSpace(soCauHoiText))
                continue;
            if (!int.TryParse(soCauHoiText, out var soCauHoi) || soCauHoi <= 0)
                throw new ArgumentException($"Số câu hỏi không hợp lệ tại dòng {row}: {soCauHoiText}. Vui lòng nhập số nguyên dương.");

            // Lấy câu hỏi
            var cauHois = await _cauHoiRepository.GetByCLoAsync(clo);
            var filteredCauHois = cauHois
                .Where(c => c.Phan != null && c.Phan.MaMonHoc == maMonHoc && c.XoaTam == false)
                .Where(c => !usedCauHoiIds.Contains(c.MaCauHoi))
                .OrderBy(_ => Guid.NewGuid())
                .Take(soCauHoi)
                .ToList();

            if (filteredCauHois.Count < soCauHoi)
                throw new ArgumentException($"Không đủ câu hỏi cho CLO {clo} tại dòng {row}. Yêu cầu {soCauHoi}, chỉ tìm thấy {filteredCauHois.Count}.");

            // Thêm MaCauHoi vào danh sách đã sử dụng
            foreach (var cauHoi in filteredCauHois)
            {
                usedCauHoiIds.Add(cauHoi.MaCauHoi);
            }

            // Thêm vào chi tiết đề thi
            totalCauHoi += filteredCauHois.Count;
            chiTietDeThis.AddRange(filteredCauHois.Select((c, index) => new ChiTietDeThiDto
            {
                MaDeThi = maDeThi, // Gán MaDeThi để khớp JSON mẫu
                MaCauHoi = c.MaCauHoi,
                MaPhan = c.MaPhan,
                ThuTu = chiTietDeThis.Count + index + 1
            }));
        }
    }

    if (totalCauHoi == 0)
        throw new ArgumentException("Không có câu hỏi nào được rút trích từ file Excel.");

    // Kiểm tra MaCauHoi trùng lặp
    if (chiTietDeThis.GroupBy(c => c.MaCauHoi).Any(g => g.Count() > 1))
        throw new ArgumentException("Danh sách ChiTietDeThi chứa MaCauHoi trùng lặp.");

    // Tạo DeThiDto
    var deThiDto = new DeThiDto
    {
        MaDeThi = maDeThi,
        MaMonHoc = maMonHoc,
        TenDeThi = $"Đề thi từ yêu cầu {maYeuCau}",
        DaDuyet = false,
        SoCauHoi = totalCauHoi,
        NgayTao = DateTime.UtcNow,
        NgayCapNhap = DateTime.UtcNow,
        ChiTietDeThis = chiTietDeThis
    };

    // Lưu đề thi
    await _deThiRepository.AddWithChiTietAsync(deThiDto);

    // Cập nhật trạng thái yêu cầu
    yeuCau.DaXuLy = true;
    yeuCau.NgayXuLy = DateTime.UtcNow;
    await _yeuCauRutTrichRepository.UpdateAsync(yeuCau);

    return deThiDto;
}


       public async Task<DeThiDto> ManualSelectCauHoiAsync(Guid maYeuCau, List<Guid> maCauHoiList)
        {
            if (maCauHoiList == null || !maCauHoiList.Any())
                throw new Exception("Danh sách câu hỏi không hợp lệ hoặc trống.");

            var yeuCau = await _yeuCauRutTrichRepository.GetByIdAsync(maYeuCau);
            if (yeuCau == null)
                throw new Exception($"Không tìm thấy yêu cầu rút trích với mã {maYeuCau}.");

            var maMonHoc = yeuCau.MaMonHoc;
            var chiTietDeThis = new List<ChiTietDeThiDto>();

            foreach (var maCauHoi in maCauHoiList)
            {
                var cauHoi = await _cauHoiRepository.GetByIdAsync(maCauHoi);
                if (cauHoi == null)
                    throw new Exception($"Câu hỏi với mã {maCauHoi} không hợp lệ hoặc đã bị xóa tạm.");

                chiTietDeThis.Add(new ChiTietDeThiDto
                {
                    MaCauHoi = cauHoi.MaCauHoi,
                    MaPhan = cauHoi.MaPhan,
                    ThuTu = chiTietDeThis.Count + 1
                });
            }

            var deThiDto = new DeThiDto
            {
                MaDeThi = Guid.NewGuid(),
                MaMonHoc = maMonHoc,
                TenDeThi = $"Đề thi từ yêu cầu {maYeuCau} (thủ công)",
                DaDuyet = false,
                SoCauHoi = maCauHoiList.Count,
                NgayTao = DateTime.UtcNow,
                NgayCapNhap = DateTime.UtcNow,
                ChiTietDeThis = chiTietDeThis
            };

            await AddWithChiTietAsync(deThiDto);

            yeuCau.DaXuLy = true;
            yeuCau.NgayXuLy = DateTime.UtcNow;
            await _yeuCauRutTrichRepository.UpdateAsync(yeuCau);

            return deThiDto;
        }

        public async Task<DeThiDto> ChangerStatusAsync(Guid id, bool DaDuyet)
        {
            var deThi = await _deThiRepository.GetByIdAsync(id);
            if (deThi == null)
                throw new Exception($"Không tìm thấy đề thi với mã {id}.");

            deThi.DaDuyet = DaDuyet;
            deThi.NgayCapNhap = DateTime.UtcNow;

            await _deThiRepository.UpdateAsync(deThi);

            return new DeThiDto
            {
                MaDeThi = deThi.MaDeThi,
                MaMonHoc = deThi.MaMonHoc,
                TenDeThi = deThi.TenDeThi,
                DaDuyet = deThi.DaDuyet,
                SoCauHoi = deThi.SoCauHoi,
                NgayTao = deThi.NgayTao,
                NgayCapNhap = deThi.NgayCapNhap
            };
            
        }
        public async Task<MemoryStream> ExportWordTemplateAsync(Guid maDeThi)
        {
            // Lấy thông tin đề thi với chi tiết và câu trả lời
            var deThiDto = await _deThiRepository.GetDeThiWithChiTietAndCauTraLoiAsync(maDeThi);
            if (deThiDto == null)
            {
                return null;
            }
        
            // Đường dẫn đến template .dotx
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "DefaultExamTemplate.dotx");
            if (!System.IO.File.Exists(templatePath))
            {
                throw new FileNotFoundException("Tệp template không tồn tại.", templatePath);
            }
        
            // Tạo tệp Word mới dựa trên template
            string filePath = Path.Combine(Path.GetTempPath(), $"DeThi_{maDeThi}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx");
            System.IO.File.Copy(templatePath, filePath, true);
        
            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
        
                // Thêm tiêu đề và thông tin đề thi
                var paragraph = new Paragraph(new Run(new Text($"Đề thi: {deThiDto.TenDeThi}")));
        
                body.Append(new Paragraph(new Run(new Text("")))); 
        
                // Thêm danh sách câu hỏi và đáp án
                int cauHoiIndex = 1;
                foreach (var chiTiet in deThiDto.ChiTietDeThis)
                {
                    var cauHoi = (await _cauHoiRepository.GetByIdAsync(chiTiet.MaCauHoi)) ?? new CauHoi { NoiDung = "N/A", MaSoCauHoi = -1, CLO = EnumCLO.CLO1 };
        
                    // Câu hỏi dòng 1
                    paragraph = new Paragraph();
                    var run = new Run();
                    run.Append(new Text($"Câu {cauHoiIndex} ({cauHoi.CLO}): {cauHoi.NoiDung}"));
                    paragraph.Append(run);
                    body.Append(paragraph);
        
                    // Đáp án a), b), c), d)
                    if (deThiDto.CauTraLoiByCauHoi.TryGetValue(chiTiet.MaCauHoi, out var cauTraLoiList))
                    {
                        char dapAnLabel = 'A';
                        foreach (var cauTraLoi in cauTraLoiList)
                        {
                            paragraph = new Paragraph();
                            run = new Run();
                            run.Append(new Text($"{dapAnLabel}. {cauTraLoi.NoiDung}"));
                            paragraph.Append(run);
                            body.Append(paragraph);
        
                            dapAnLabel++;
                        }
                    }
        
                    body.Append(new Paragraph(new Run(new Text("")))); 
                    cauHoiIndex++;
                }
        
                doc.MainDocumentPart.Document.Save();
            }
        
            // Chuyển file thành MemoryStream để trả về
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;
            System.IO.File.Delete(filePath); // Xóa file tạm sau khi sử dụng
        
            return memoryStream;
        }
       // public async Task<MemoryStream> ExportWordTemplateAsync(Guid maDeThi, ExamTemplateParametersDto parameters)
       //      {
       //          // Retrieve exam information with details and answers
       //          var deThiDto = await _deThiRepository.GetDeThiWithChiTietAndCauTraLoiAsync(maDeThi);
       //          if (deThiDto == null)
       //          {
       //              return null;
       //          }
       //
       //          // Fallback values if parameters are not provided
       //          string monThi = parameters.MonThi ?? deThiDto.TenDeThi ?? "N/A";
       //         
       //          string soTinChi = parameters.SoTinChi ?? "3";
       //          string hocKy = parameters.HocKy ?? "N/A";
       //          string lop = parameters.Lop ?? "N/A";
       //          string ngayThi = parameters.NgayThi ?? DateTime.Now.ToString("dd/MM/yyyy");
       //          string thoiGianLam = parameters.ThoiGianLam ?? "N/A";
       //          string hinhThuc = parameters.HinhThuc ?? "N/A";
       //          string maDe = parameters.MaDe ?? "N/A";
       //          string taiLieu = parameters.TaiLieuCo == true ? "CÓ" : "KHÔNG";
       //
       //          // Create a temporary Word document
       //          string filePath = Path.Combine(Path.GetTempPath(), $"DeThi_{maDeThi}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx");
       //          var memoryStream = new MemoryStream();
       //          using (var doc = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
       //          {
       //              // Initialize main document part
       //              MainDocumentPart mainPart = doc.AddMainDocumentPart();
       //              mainPart.Document = new Document();
       //              Body body = new Body();
       //              mainPart.Document.Append(body);
       //
       //              // Add header section
       //              Paragraph header = new Paragraph(new Run(new Text("BM:03/QT02-P.KT")));
       //              header.ParagraphProperties = new ParagraphProperties(
       //                  new Justification { Val = JustificationValues.Center },
       //                  new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto }
       //              );
       //              body.Append(header);
       //
       //              // Add exam title and details
       //              body.Append(new Paragraph(
       //                  new Run(new Text($"ĐỀ THI HỌC KỲ {hocKy} NĂM HỌC"))
       //                  {
       //                      RunProperties = new RunProperties(new Bold(), new FontSize { Val = 28 })
       //                  }
       //              ) { ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center }) });
       //
       //              body.Append(new Paragraph(
       //                  new Run(new Text($"Lớp: {lop}"))
       //              ) { ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Left }) });
       //
       //              body.Append(new Paragraph(
       //                  new Run(new Text($"Môn thi: {monThi}"))
       //              ) { ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Left }) });
       //
       //              body.Append(new Paragraph(
       //                  new Run(new Text($"SoTC: {soTinChi}"))
       //              ) { ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Left }) });
       //
       //              body.Append(new Paragraph(
       //                  new Run(new Text($"Ngày thi: {ngayThi}"))
       //              ) { ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Left }) });
       //
       //              body.Append(new Paragraph(
       //                  new Run(new Text($"Thời gian làm bài: {thoiGianLam}"))
       //              ) { ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Left }) });
       //
       //              body.Append(new Paragraph(
       //                  new Run(new Text($"Hình thức thi: {hinhThuc}"))
       //              ) { ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Left }) });
       //
       //              body.Append(new Paragraph(
       //                  new Run(new Text($"Mã đề: {maDe}"))
       //              ) { ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Left }) });
       //
       //              // Add "SỬ DỤNG TÀI LIỆU" section
       //              body.Append(new Paragraph(
       //                  new Run(new Text($"SỬ DỤNG TÀI LIỆU: {taiLieu}"))
       //              ) { ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center }) });
       //            
       //              foreach (var chiTiet in deThiDto.ChiTietDeThis)
       //              {
       //                  var cauHoi = await _cauHoiRepository.GetByIdAsync(chiTiet.MaCauHoi) ?? new CauHoi { NoiDung = "N/A", MaSoCauHoi = -1, CLO = EnumCLO.CLO1 };
       //                  Paragraph questionPara = new Paragraph(
       //                      new Run(new Text($"  - STT: {chiTiet.ThuTu}, Nội dung: {cauHoi.NoiDung}, MaSoCauHoi: {cauHoi.MaSoCauHoi}, CLO: {cauHoi.CLO}"))
       //                  );
       //                  body.Append(questionPara);
       //
       //                  if (deThiDto.CauTraLoiByCauHoi.TryGetValue(chiTiet.MaCauHoi, out var cauTraLoiList))
       //                  {
       //                      foreach (var cauTraLoi in cauTraLoiList)
       //                      {
       //                          body.Append(new Paragraph(
       //                              new Run(new Text($"     - Đáp án {cauTraLoi.ThuTu}: {cauTraLoi.NoiDung} {(cauTraLoi.LaDapAn ? "(Đáp án đúng)" : "")}"))
       //                          ));
       //                      }
       //                  }
       //              }
       //
       //              // Save document
       //              mainPart.Document.Save();
       //          }
       //
       //          memoryStream.Position = 0;
       //          return memoryStream;
       //      }
                  
       public async Task<MemoryStream> ExportWordTemplateAsync(Guid maDeThi, ExamTemplateParametersDto parameters = null)
       {
           var deThiDto = await _deThiRepository.GetDeThiWithChiTietAndCauTraLoiAsync(maDeThi);
           if (deThiDto == null)
           {
               return null;
           }

           return await HtmlConverter.ExportWordTemplateAsync(deThiDto, parameters);
       }
        public async Task<IEnumerable<CauTraLoiDto>> GetCauTraLoiByDeThiAsync(Guid maDeThi)
        {
            // Lấy thông tin đề thi với chi tiết
            var deThiDto = await _deThiRepository.GetByIdWithChiTietAsync(maDeThi);
            if (deThiDto == null || deThiDto.ChiTietDeThis == null)
            {
                return Enumerable.Empty<CauTraLoiDto>(); 
            }

            // Lấy danh sách MaCauHoi từ ChiTietDeThi
            var maCauHoiList = deThiDto.ChiTietDeThis.Select(ct => ct.MaCauHoi).Distinct().ToList();

            // Lấy tất cả câu trả lời liên quan đến các MaCauHoi
            var cauTraLoiList = await _cauTraLoiRepository.FindAsync(ct => maCauHoiList.Contains(ct.MaCauHoi));

            // Chuyển đổi sang DTO
            var cauTraLoiDtos = cauTraLoiList.Select(ct => new CauTraLoiDto
            {
                MaCauTraLoi = ct.MaCauTraLoi,
                MaCauHoi = ct.MaCauHoi,
                NoiDung = ct.NoiDung,
                ThuTu = ct.ThuTu,
                LaDapAn = ct.LaDapAn,
                HoanVi = ct.HoanVi
            }).ToList();

            return cauTraLoiDtos;
        }
        
        public async Task<DeThiWithChiTietAndCauTraLoiDto> GetDeThiWithChiTietAndCauTraLoiAsync(Guid maDeThi)
        {
            // Lấy thông tin đề thi với chi tiết
            var deThiDto = await _deThiRepository.GetByIdWithChiTietAsync(maDeThi);
            if (deThiDto == null || deThiDto.ChiTietDeThis == null)
            {
                return null; // Trả về null nếu không tìm thấy đề thi
            }

            // Tạo DTO tổng hợp
            var result = new DeThiWithChiTietAndCauTraLoiDto
            {
                MaDeThi = deThiDto.MaDeThi,
                MaMonHoc = deThiDto.MaMonHoc,
                TenDeThi = deThiDto.TenDeThi,
                DaDuyet = deThiDto.DaDuyet ?? false,
                SoCauHoi = deThiDto.SoCauHoi ?? 0,
                NgayTao = deThiDto.NgayTao,
                NgayCapNhap = deThiDto.NgayCapNhap,
                ChiTietDeThis = deThiDto.ChiTietDeThis.ToList(),
                CauTraLoiByCauHoi = new Dictionary<Guid, List<CauTraLoiDto>>()
            };

            // Lấy danh sách MaCauHoi từ ChiTietDeThi
            var maCauHoiList = deThiDto.ChiTietDeThis.Select(ct => ct.MaCauHoi).Distinct().ToList();

            // Lấy tất cả câu trả lời liên quan đến các MaCauHoi
            var cauTraLoiList = await _cauTraLoiRepository.FindAsync(ct => maCauHoiList.Contains(ct.MaCauHoi));

            // Nhóm câu trả lời theo MaCauHoi
            var cauTraLoiByCauHoi = cauTraLoiList
                .GroupBy(ct => ct.MaCauHoi)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(ct => new CauTraLoiDto
                    {
                        MaCauTraLoi = ct.MaCauTraLoi,
                        MaCauHoi = ct.MaCauHoi,
                        NoiDung = ct.NoiDung,
                        ThuTu = ct.ThuTu,
                        LaDapAn = ct.LaDapAn,
                        HoanVi = ct.HoanVi
                    }).ToList()
                );

            result.CauTraLoiByCauHoi = cauTraLoiByCauHoi;

            return result;
        }
    }
}