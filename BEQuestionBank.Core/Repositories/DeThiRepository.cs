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
                .FirstOrDefaultAsync(dt => dt.MaDeThi == id);

            if (deThi == null)
                return null;

            return new DeThiDto
            {
                MaDeThi = deThi.MaDeThi,
                MaMonHoc = deThi.MaMonHoc,
                TenDeThi = deThi.TenDeThi,
                DaDuyet = deThi.DaDuyet,
                SoCauHoi = deThi.SoCauHoi,
                NgayTao = deThi.NgayTao,
                NgaySua = deThi.NgayCapNhap,
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
                .ToListAsync();

            return deThis.Select(deThi => new DeThiDto
            {
                MaDeThi = deThi.MaDeThi,
                MaMonHoc = deThi.MaMonHoc,
                TenDeThi = deThi.TenDeThi,
                DaDuyet = deThi.DaDuyet,
                SoCauHoi = deThi.SoCauHoi,
                NgayTao = deThi.NgayTao,
                NgaySua = deThi.NgayCapNhap,
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

            var deThi = new DeThi
            {
                MaDeThi = Guid.NewGuid(),
                MaMonHoc = deThiDto.MaMonHoc,
                TenDeThi = deThiDto.TenDeThi,
                DaDuyet = deThiDto.DaDuyet ?? false,
                SoCauHoi = deThiDto.SoCauHoi,
                NgayTao = deThiDto.NgayTao,
                NgayCapNhap = deThiDto.NgaySua,
                ChiTietDeThis = deThiDto.ChiTietDeThis?.Select(ct => new ChiTietDeThi
                {
                    MaDeThi = ct.MaDeThi ?? Guid.NewGuid(),
                    MaPhan = ct.MaPhan,
                    MaCauHoi = ct.MaCauHoi,
                    ThuTu = ct.ThuTu
                }).ToList() ?? new List<ChiTietDeThi>()
            };

            await AddAsync(deThi);
            await _context.SaveChangesAsync();

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
                NgaySua = DateTime.UtcNow,
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
                NgaySua = deThi.NgayCapNhap,
                ChiTietDeThis = deThi.ChiTietDeThis?.Select(ct => new ChiTietDeThiDto
                {
                    MaDeThi = ct.MaDeThi,
                    MaPhan = ct.MaPhan,
                    MaCauHoi = ct.MaCauHoi,
                    ThuTu = ct.ThuTu
                }).ToList() ?? new List<ChiTietDeThiDto>()
            }).ToList();
        }

    }
}