using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.DeThi;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Shared.DTOs.ChiTietDeThi;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace BEQuestionBank.Core.Services
{
    public class DeThiService : IDeThiService
    {
        private readonly IDeThiRepository _deThiRepository;
        private readonly IYeuCauRutTrichRepository _yeuCauRutTrichRepository;
        private readonly ICauHoiRepository _cauHoiRepository;

        public DeThiService(IDeThiRepository deThiRepository, IYeuCauRutTrichRepository yeuCauRutTrichRepository, ICauHoiRepository cauHoiRepository)
        {
            _deThiRepository = deThiRepository;
            _yeuCauRutTrichRepository = yeuCauRutTrichRepository;
            _cauHoiRepository = cauHoiRepository;
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
        NgaySua = DateTime.UtcNow,
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
                NgaySua = DateTime.UtcNow,
                ChiTietDeThis = chiTietDeThis
            };

            await AddWithChiTietAsync(deThiDto);

            yeuCau.DaXuLy = true;
            yeuCau.NgayXuLy = DateTime.UtcNow;
            await _yeuCauRutTrichRepository.UpdateAsync(yeuCau);

            return deThiDto;
        }

    }
}