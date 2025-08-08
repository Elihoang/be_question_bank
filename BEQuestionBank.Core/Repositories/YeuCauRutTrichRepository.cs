using BEQuestionBank.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Core.Repositories;
using BEQuestionBank.Domain.Interfaces.Repo;

namespace BEQuestionBank.Infrastructure.Repositories
{
    public class YeuCauRutTrichRepository : GenericRepository<YeuCauRutTrich> , IYeuCauRutTrichRepository
    {
        private readonly AppDbContext _context; 

        public YeuCauRutTrichRepository(AppDbContext context) : base(context)
        {
            _context = context;
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
                .AsNoTracking()
                .ToListAsync();
        }

    }
}