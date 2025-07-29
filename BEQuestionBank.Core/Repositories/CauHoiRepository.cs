using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Core.Repositories;
using BEQuestionBank.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.CauHoi;
using BEQuestionBank.Shared.DTOs.CauTraLoi;

namespace BEQuestionBank.Infrastructure.Repositories
{
    public class CauHoiRepository : GenericRepository<CauHoi>, ICauHoiRepository
    {
        private readonly AppDbContext _context;

        public CauHoiRepository(AppDbContext context)
            : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CauHoiDto>> GetAllWithAnswersAsync()
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.XoaTam == false) // Lọc câu hỏi chưa bị xóa tạm
                .AsNoTracking()
                .Select(c => new CauHoiDto
                {
                    MaCauHoi = c.MaCauHoi,
                    MaPhan = c.MaPhan,
                    MaSoCauHoi = c.MaSoCauHoi,
                    NoiDung = c.NoiDung,
                    HoanVi = c.HoanVi,
                    CapDo = c.CapDo,
                    SoCauHoiCon = c.SoCauHoiCon,
                    DoPhanCach = c.DoPhanCach,
                    MaCauHoiCha = c.MaCauHoiCha,
                    XoaTam = c.XoaTam,
                    SoLanDuocThi = c.SoLanDuocThi,
                    SoLanDung = c.SoLanDung,
                    NgayTao = c.NgayTao,
                    NgaySua = c.NgaySua,
                    CLO = c.CLO,
                    CauTraLois = c.CauTraLois.Select(ctl => new CauTraLoiDto
                    {
                        MaCauTraLoi = ctl.MaCauTraLoi,
                        MaCauHoi = ctl.MaCauHoi,
                        NoiDung = ctl.NoiDung,
                        ThuTu = ctl.ThuTu,
                        LaDapAn = ctl.LaDapAn,
                        HoanVi = ctl.HoanVi
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<CauHoiDto> GetByIdWithAnswersAsync(Guid id)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.MaCauHoi == id && c.XoaTam == false)
                .AsNoTracking()
                .Select(c => new CauHoiDto
                {
                    MaCauHoi = c.MaCauHoi,
                    MaPhan = c.MaPhan,
                    MaSoCauHoi = c.MaSoCauHoi,
                    NoiDung = c.NoiDung,
                    HoanVi = c.HoanVi,
                    CapDo = c.CapDo,
                    SoCauHoiCon = c.SoCauHoiCon,
                    DoPhanCach = c.DoPhanCach,
                    MaCauHoiCha = c.MaCauHoiCha,
                    XoaTam = c.XoaTam,
                    SoLanDuocThi = c.SoLanDuocThi,
                    SoLanDung = c.SoLanDung,
                    NgayTao = c.NgayTao,
                    NgaySua = c.NgaySua,
                    CLO = c.CLO,
                    CauTraLois = c.CauTraLois.Select(ctl => new CauTraLoiDto
                    {
                        MaCauTraLoi = ctl.MaCauTraLoi,
                        MaCauHoi = ctl.MaCauHoi,
                        NoiDung = ctl.NoiDung,
                        ThuTu = ctl.ThuTu,
                        LaDapAn = ctl.LaDapAn,
                        HoanVi = ctl.HoanVi
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CauHoi>> GetByCLoAsync(EnumCLO maCLo)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.CLO == maCLo && c.XoaTam == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CauHoi>> GetByMaPhanAsync(Guid maPhan)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.MaPhan == maPhan && c.XoaTam == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CauHoi>> GetByMaMonHocAsync(Guid maMonHoc)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.Phan.MaMonHoc == maMonHoc && c.XoaTam == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CauHoi>> GetByMaDeThiAsync(Guid maDeThi)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.DeThis.Any(d => d.MaDeThi == maDeThi) && c.XoaTam == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CauHoi>> GetByMaCauHoiChasync(Guid maCHCha)
        {
            return await _context.CauHois
                .Include(c => c.CauTraLois)
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                .Where(c => c.MaCauHoiCha == maCHCha && c.XoaTam == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CauHoiDto>> GetAllGroupsAsync()
        {
            Expression<Func<CauHoi, CauHoiDto>> mapCauHoiToDto = c => new CauHoiDto
            {
                MaCauHoi = c.MaCauHoi,
                MaPhan = c.MaPhan,
                MaSoCauHoi = c.MaSoCauHoi,
                NoiDung = c.NoiDung,
                HoanVi = c.HoanVi,
                CapDo = c.CapDo,
                SoCauHoiCon = c.SoCauHoiCon,
                DoPhanCach = c.DoPhanCach,
                MaCauHoiCha = c.MaCauHoiCha,
                XoaTam = c.XoaTam,
                SoLanDuocThi = c.SoLanDuocThi,
                SoLanDung = c.SoLanDung,
                NgayTao = c.NgayTao,
                NgaySua = c.NgaySua,
                CLO = c.CLO,
                CauTraLois = c.CauTraLois != null 
                    ? c.CauTraLois.Select(ctl => new CauTraLoiDto
                    {
                        MaCauTraLoi = ctl.MaCauTraLoi,
                        MaCauHoi = ctl.MaCauHoi,
                        NoiDung = ctl.NoiDung,
                        ThuTu = ctl.ThuTu,
                        LaDapAn = ctl.LaDapAn,
                        HoanVi = ctl.HoanVi
                    }).ToList()
                    : new List<CauTraLoiDto>(),
                CauHoiCons = c.CauHoiCons
                    .Where(con => con.XoaTam == false)
                    .Select(con => new CauHoiDto
                    {
                        MaCauHoi = con.MaCauHoi,
                        MaPhan = con.MaPhan,
                        MaSoCauHoi = con.MaSoCauHoi,
                        NoiDung = con.NoiDung,
                        HoanVi = con.HoanVi,
                        CapDo = con.CapDo,
                        SoCauHoiCon = con.SoCauHoiCon,
                        DoPhanCach = con.DoPhanCach,
                        MaCauHoiCha = con.MaCauHoiCha,
                        XoaTam = con.XoaTam,
                        SoLanDuocThi = con.SoLanDuocThi,
                        SoLanDung = con.SoLanDung,
                        NgayTao = con.NgayTao,
                        NgaySua = con.NgaySua,
                        CLO = con.CLO,
                        CauTraLois = con.CauTraLois != null 
                            ? con.CauTraLois.Select(ctl => new CauTraLoiDto
                            {
                                MaCauTraLoi = ctl.MaCauTraLoi,
                                MaCauHoi = ctl.MaCauHoi,
                                NoiDung = ctl.NoiDung,
                                ThuTu = ctl.ThuTu,
                                LaDapAn = ctl.LaDapAn,
                                HoanVi = ctl.HoanVi
                            }).ToList()
                            : new List<CauTraLoiDto>(),
                        CauHoiCons = new List<CauHoiDto>()
                    }).ToList()
            };

            var result = await _context.CauHois
                .Include(c => c.Phan)
                .Include(c => c.Files)
                .Include(c => c.CauHoiCha)
                .Include(c => c.CauHoiCons)
                    .ThenInclude(con => con.CauTraLois)
                .Where(c => c.CauHoiCons.Any() && c.XoaTam == false)
                .AsNoTracking()
                .Select(mapCauHoiToDto)
                .ToListAsync();

            return result ?? new List<CauHoiDto>();
        }

    }
}
