using BEQuestionBank.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Core.Repositories;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Domain.Interfaces.Repo;

namespace BEQuestionBank.Infrastructure.Repositories
{
    public class YeuCauRutTrichRepository : GenericRepository<YeuCauRutTrich> , IYeuCauRutTrichRepository
    {
        private readonly AppDbContext _context; 
        public Dictionary<EnumCLO, int> Clos { get; set; }

        public YeuCauRutTrichRepository(AppDbContext context) : base(context)
        {
            _context = context;
            Clos = new Dictionary<EnumCLO, int>(); 
        }
        public async Task<bool> ExistsPhanAsync(Guid maPhan)
        {
            return await _context.Phans.AnyAsync(p => p.MaPhan == maPhan);
        }

        public async Task<IEnumerable<YeuCauRutTrich>> GetByMaNguoiDungAsync(Guid maNguoiDung)
        {
            return await _context.YeuCauRutTrichs
                .Include(y => y.NguoiDung)
                .Include(y => y.MonHoc)
                .Where(y => y.MaNguoiDung == maNguoiDung)
                .ToListAsync();
        }

        public async Task<IEnumerable<YeuCauRutTrich>> GetByMaMonHocAsync(Guid maMonHoc)
        {
            return await _context.YeuCauRutTrichs
                .Include(y => y.NguoiDung)
                .Include(y => y.MonHoc)
                .Where(y => y.MaMonHoc == maMonHoc)
                .ToListAsync();
        }

        public async Task<IEnumerable<YeuCauRutTrich>> GetChuaXuLyAsync()
        {
            return await _context.YeuCauRutTrichs
                .Include(y => y.NguoiDung)
                .Include(y => y.MonHoc)
                .Where(y => y.DaXuLy == false)
                .ToListAsync();
        }

        public async Task<bool> ExistsNguoiDungAsync(Guid maNguoiDung)
        {
            return await _context.NguoiDungs.AnyAsync(n => n.MaNguoiDung == maNguoiDung);
        }

        public async Task<bool> ExistsMonHocAsync(Guid maMonHoc)
        {
            return await _context.MonHocs.AnyAsync(m => m.MaMonHoc == maMonHoc);
        }
        public async Task<IEnumerable<YeuCauRutTrich>> GetAllAsync()
        {
            return await _context.YeuCauRutTrichs
                .Include(nd => nd.NguoiDung)
                .Include(y => y.MonHoc)
                .ThenInclude(ct => ct.Khoa)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<YeuCauRutTrich> GetByIdAsync(Guid id)
        {
            return await _context.YeuCauRutTrichs
                .Include(y => y.NguoiDung)
                .Include(y => y.MonHoc)
                .ThenInclude(m => m.Khoa)
                .AsNoTracking()
                .FirstOrDefaultAsync(y => y.MaYeuCau == id);
        }

    }
}