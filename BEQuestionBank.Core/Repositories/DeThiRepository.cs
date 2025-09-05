using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.DeThi;
using BEQuestionBank.Shared.DTOs.ChiTietDeThi;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Shared.DTOs.CauHoi;
using BEQuestionBank.Shared.DTOs.CauTraLoi;

namespace BEQuestionBank.Core.Repositories
{
    public class DeThiRepository : GenericRepository<DeThi>, IDeThiRepository
    {
        private readonly AppDbContext _context;

        public DeThiRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<DeThiDto> GetByIdWithChiTietAsync(Guid id)
        {
            var deThi = await _context.DeThis
                .Include(dt => dt.ChiTietDeThis)
                .Include(dt => dt.MonHoc)
                .ThenInclude(mh => mh.Khoa)
                .FirstOrDefaultAsync(dt => dt.MaDeThi == id);

            if (deThi == null)
                return null;

            return new DeThiDto
            {
                MaDeThi = deThi.MaDeThi,
                MaMonHoc = deThi.MaMonHoc,
                TenDeThi = deThi.TenDeThi,
                TenMonHoc = deThi.MonHoc?.TenMonHoc,
                TenKhoa = deThi.MonHoc?.Khoa?.TenKhoa,
                DaDuyet = deThi.DaDuyet,
                SoCauHoi = deThi.SoCauHoi,
                NgayTao = deThi.NgayTao,
                NgayCapNhap = deThi.NgayCapNhap,
                ChiTietDeThis = deThi.ChiTietDeThis?.Select(ct => new ChiTietDeThiDto
                {
                    MaDeThi = ct.MaDeThi,
                    MaPhan = ct.MaPhan,
                    MaCauHoi = ct.MaCauHoi,
                    ThuTu = ct.ThuTu
                }).ToList() ?? new List<ChiTietDeThiDto>()
            };
        }

        public async Task<IEnumerable<DeThiDto>> GetAllWithChiTietAsync()
        {
            var deThis = await _context.DeThis
                .Include(dt => dt.ChiTietDeThis)
                .Include(dt => dt.MonHoc)
                .ThenInclude(mh => mh.Khoa)
                .ToListAsync();

            return deThis.Select(deThi => new DeThiDto
            {
                MaDeThi = deThi.MaDeThi,
                MaMonHoc = deThi.MaMonHoc,
                TenDeThi = deThi.TenDeThi,
                DaDuyet = deThi.DaDuyet,
                SoCauHoi = deThi.SoCauHoi,
                TenMonHoc = deThi.MonHoc?.TenMonHoc,
                TenKhoa = deThi.MonHoc?.Khoa?.TenKhoa,
                NgayTao = deThi.NgayTao,
                NgayCapNhap = deThi.NgayCapNhap,
                ChiTietDeThis = deThi.ChiTietDeThis?.Select(ct => new ChiTietDeThiDto
                {
                    MaDeThi = ct.MaDeThi,
                    MaPhan = ct.MaPhan,
                    MaCauHoi = ct.MaCauHoi,
                    ThuTu = ct.ThuTu
                }).ToList() ?? new List<ChiTietDeThiDto>()
            }).ToList();
        }

        public async Task<DeThiDto> AddWithChiTietAsync(DeThiDto deThiDto)
        {
            if (deThiDto == null)
                throw new ArgumentNullException(nameof(deThiDto));

            // Sử dụng MaDeThi từ deThiDto hoặc tạo mới nếu không có
            var maDeThi = deThiDto.MaDeThi != Guid.Empty ? deThiDto.MaDeThi : Guid.NewGuid();
            var deThi = new DeThi
            {
                MaDeThi = maDeThi,
                MaMonHoc = deThiDto.MaMonHoc,
                TenDeThi = deThiDto.TenDeThi,
                DaDuyet = deThiDto.DaDuyet ?? false,
                SoCauHoi = deThiDto.SoCauHoi,
                NgayTao = deThiDto.NgayTao,
                NgayCapNhap = deThiDto.NgayCapNhap,
                ChiTietDeThis = new List<ChiTietDeThi>()
            };

            // Thêm ChiTietDeThi với cùng MaDeThi
            if (deThiDto.ChiTietDeThis != null)
            {
                foreach (var chiTietDto in deThiDto.ChiTietDeThis)
                {
                    deThi.ChiTietDeThis.Add(new ChiTietDeThi
                    {
                        MaDeThi = maDeThi, // Sử dụng maDeThi của DeThi
                        MaPhan = chiTietDto.MaPhan,
                        MaCauHoi = chiTietDto.MaCauHoi,
                        ThuTu = chiTietDto.ThuTu
                    });
                }
            }

            // Thêm DeThi vào context
            await _context.DeThis.AddAsync(deThi);
            await _context.SaveChangesAsync();

            // Cập nhật deThiDto
            deThiDto.MaDeThi = deThi.MaDeThi;
            return deThiDto;
        }

        public async Task<DeThiDto> UpdateWithChiTietAsync(DeThiDto deThiDto)
        {
            if (deThiDto == null)
                throw new ArgumentNullException(nameof(deThiDto));

            var deThi = await _context.DeThis
                .Include(dt => dt.ChiTietDeThis)
                .FirstOrDefaultAsync(dt => dt.MaDeThi == deThiDto.MaDeThi);

            if (deThi == null)
                return null;

            deThi.MaMonHoc = deThiDto.MaMonHoc;
            deThi.TenDeThi = deThiDto.TenDeThi;
            deThi.DaDuyet = deThiDto.DaDuyet ?? false;
            deThi.SoCauHoi = deThiDto.SoCauHoi;
            deThi.NgayCapNhap = DateTime.UtcNow;

            // Cập nhật Chi Tiết Đề Thi
            if (deThi.ChiTietDeThis != null)
            {
                _context.ChiTietDeThis.RemoveRange(deThi.ChiTietDeThis);
            }

            deThi.ChiTietDeThis = deThiDto.ChiTietDeThis?.Select(ct => new ChiTietDeThi
            {
                MaDeThi = ct.MaDeThi ?? deThi.MaDeThi,
                MaPhan = ct.MaPhan,
                MaCauHoi = ct.MaCauHoi,
                ThuTu = ct.ThuTu
            }).ToList() ?? new List<ChiTietDeThi>();

            await _context.SaveChangesAsync();

            return new DeThiDto
            {
                MaDeThi = deThi.MaDeThi,
                MaMonHoc = deThi.MaMonHoc,
                TenDeThi = deThi.TenDeThi,
                DaDuyet = deThi.DaDuyet,
                SoCauHoi = deThi.SoCauHoi,
                NgayTao = deThi.NgayTao,
                NgayCapNhap = DateTime.UtcNow,
                ChiTietDeThis = deThi.ChiTietDeThis?.Select(ct => new ChiTietDeThiDto
                {
                    MaDeThi = ct.MaDeThi,
                    MaPhan = ct.MaPhan,
                    MaCauHoi = ct.MaCauHoi,
                    ThuTu = ct.ThuTu
                }).ToList() ?? new List<ChiTietDeThiDto>()
            };

        }

        public async Task<IEnumerable<DeThi>> GetByMaMonHocAsync(Guid maMonHoc)
        {
            return await _context.DeThis
                .Where(d => d.MaMonHoc == maMonHoc)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeThiDto>> GetApprovedDeThisAsync()
        {
            var deThis = await _context.DeThis.
                Include(dt => dt.ChiTietDeThis)
                .Where(d => d.DaDuyet == true)
                .ToListAsync();
            
            return deThis.Select(deThi => new DeThiDto
            {
                MaDeThi = deThi.MaDeThi,
                MaMonHoc = deThi.MaMonHoc,
                TenDeThi = deThi.TenDeThi,
                
                DaDuyet = deThi.DaDuyet,
                SoCauHoi = deThi.SoCauHoi,
                NgayTao = deThi.NgayTao,
                NgayCapNhap = deThi.NgayCapNhap,
                ChiTietDeThis = deThi.ChiTietDeThis?.Select(ct => new ChiTietDeThiDto
                {
                    MaDeThi = ct.MaDeThi,
                    MaPhan = ct.MaPhan,
                    MaCauHoi = ct.MaCauHoi,
                    ThuTu = ct.ThuTu
                }).ToList() ?? new List<ChiTietDeThiDto>()
            }).ToList();
        }

        
    public async Task<DeThiWithChiTietAndCauTraLoiDto> GetDeThiWithChiTietAndCauTraLoiAsync(Guid maDeThi)
{
    var deThi = await _context.DeThis
        .Where(dt => dt.MaDeThi == maDeThi)
        .Include(dt => dt.MonHoc)
        .ThenInclude(mh => mh.Khoa)
        .Include(dt => dt.ChiTietDeThis)
            .ThenInclude(ct => ct.CauHoi)
                .ThenInclude(ch => ch.CauTraLois)
        .Include(dt => dt.ChiTietDeThis) // Thêm Include cho CauHoiCons
            .ThenInclude(ct => ct.CauHoi)
                .ThenInclude(ch => ch.CauHoiCons) // Lấy câu hỏi con
                .ThenInclude(con => con.CauTraLois) // Lấy câu trả lời của câu hỏi con
        .Select(dt => new DeThiWithChiTietAndCauTraLoiDto
        {
            MaDeThi = dt.MaDeThi,
            TenDeThi = dt.TenDeThi,
            DaDuyet = dt.DaDuyet,
            SoCauHoi = dt.SoCauHoi,
            MaMonHoc = dt.MaMonHoc,
            TenMonHoc = dt.MonHoc.TenMonHoc,
            TenKhoa = dt.MonHoc.Khoa.TenKhoa,
            NgayTao = dt.NgayTao,
            NgayCapNhap = dt.NgayCapNhap,
            ChiTietDeThis = dt.ChiTietDeThis.Select(ct => new ChiTietDeThiDto
            {
                MaDeThi = ct.MaDeThi,
                MaPhan = ct.MaPhan,
                MaCauHoi = ct.MaCauHoi,
                ThuTu = ct.ThuTu,
                CauHoi = new CauHoiDto
                {
                    MaCauHoi = ct.CauHoi.MaCauHoi,
                    MaPhan = ct.CauHoi.MaPhan,
                    MaSoCauHoi = ct.CauHoi.MaSoCauHoi,
                    NoiDung = ct.CauHoi.NoiDung,
                    HoanVi = ct.CauHoi.HoanVi,
                    CapDo = ct.CauHoi.CapDo,
                    SoCauHoiCon = ct.CauHoi.SoCauHoiCon,
                    MaCauHoiCha = ct.CauHoi.MaCauHoiCha,
                    SoLanDuocThi = ct.CauHoi.SoLanDuocThi,
                    SoLanDung = ct.CauHoi.SoLanDung,
                    DoPhanCach = ct.CauHoi.DoPhanCach,
                    XoaTam = ct.CauHoi.XoaTam,
                    CLO = ct.CauHoi.CLO,
                    NgaySua = ct.CauHoi.NgaySua,
                    CauTraLois = ct.CauHoi.CauTraLois.Select(ctl => new CauTraLoiDto
                    {
                        MaCauTraLoi = ctl.MaCauTraLoi,
                        MaCauHoi = ctl.MaCauHoi,
                        NoiDung = ctl.NoiDung,
                        ThuTu = ctl.ThuTu,
                        HoanVi = ctl.HoanVi,
                        LaDapAn = ctl.LaDapAn
                    }).ToList(),
                    CauHoiCons = ct.CauHoi.CauHoiCons.Select(con => new CauHoiDto
                    {
                        MaCauHoi = con.MaCauHoi,
                        MaPhan = con.MaPhan,
                        MaSoCauHoi = con.MaSoCauHoi,
                        NoiDung = con.NoiDung,
                        HoanVi = con.HoanVi,
                        CapDo = con.CapDo,
                        SoCauHoiCon = con.SoCauHoiCon,
                        MaCauHoiCha = con.MaCauHoiCha,
                        SoLanDuocThi = con.SoLanDuocThi,
                        SoLanDung = con.SoLanDung,
                        DoPhanCach = con.DoPhanCach,
                        XoaTam = con.XoaTam,
                        CLO = con.CLO,
                        NgaySua = con.NgaySua,
                        CauTraLois = con.CauTraLois.Select(ctl => new CauTraLoiDto
                        {
                            MaCauTraLoi = ctl.MaCauTraLoi,
                            MaCauHoi = ctl.MaCauHoi,
                            NoiDung = ctl.NoiDung,
                            ThuTu = ctl.ThuTu,
                            HoanVi = ctl.HoanVi,
                            LaDapAn = ctl.LaDapAn
                        }).ToList(),
                        CauHoiCons = new List<CauHoiDto>() // Câu hỏi con không có câu hỏi con
                    }).ToList()
                }
            }).ToList()
        })
        .FirstOrDefaultAsync();

    if (deThi == null)
    {
        throw new KeyNotFoundException($"Không tìm thấy đề thi với MaDeThi: {maDeThi}");
    }

    return deThi;
}


    }
}