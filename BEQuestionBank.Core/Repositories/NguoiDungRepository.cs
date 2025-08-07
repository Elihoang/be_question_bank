using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BEQuestionBank.Core.Repositories
{
    public class NguoiDungRepository : GenericRepository<NguoiDung>, INguoiDungRepository
    {
        private readonly AppDbContext _context;

        public NguoiDungRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<NguoiDung> GetByIdAsync(Guid maNguoiDung)
        {
            return await _context.NguoiDungs
                .AsNoTracking()
                .FirstOrDefaultAsync(nd => nd.MaNguoiDung == maNguoiDung);
        }

        public async Task<NguoiDung> GetByUsernameAsync(string tenDangNhap)
        {
            return await _context.NguoiDungs
                .AsNoTracking()
                .FirstOrDefaultAsync(nd => nd.TenDangNhap == tenDangNhap);
        }

        public async Task<IEnumerable<NguoiDung>> GetByVaiTroAsync(VaiTroNguoiDung vaiTro)
        {
            return await _context.NguoiDungs
                .AsNoTracking()
                .Where(nd => nd.VaiTro == vaiTro)
                .ToListAsync();
        }

        public async Task<IEnumerable<NguoiDung>> GetByKhoaAsync(Guid maKhoa)
        {
            return await _context.NguoiDungs
                .AsNoTracking()
                .Where(nd => nd.MaKhoa == maKhoa)
                .ToListAsync();
        }

        public async Task<bool> IsLockedAsync(string tenDangNhap)
        {
            return await _context.NguoiDungs
                .AsNoTracking()
                .Where(nd => nd.TenDangNhap == tenDangNhap)
                .Select(nd => nd.BiKhoa)
                .FirstOrDefaultAsync();
        }

        public async Task<NguoiDung> GetByResetCodeAsync(Guid maKhoa)
        {
            return await _context.NguoiDungs
                .AsNoTracking()
                .FirstOrDefaultAsync(nd => nd.MaKhoa == maKhoa);
        }

        public async Task<IEnumerable<NguoiDung>> GetAllAsync()
        {
            return await _context.NguoiDungs
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<NguoiDung>> FindAsync(Expression<Func<NguoiDung, bool>> predicate)
        {
            return await _context.NguoiDungs
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<NguoiDung> FirstOrDefaultAsync(Expression<Func<NguoiDung, bool>> predicate)
        {
            return await _context.NguoiDungs
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(NguoiDung entity)
        {
            await _context.NguoiDungs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NguoiDung entity)
        {
            _context.NguoiDungs.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(NguoiDung entity)
        {
            _context.NguoiDungs.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<NguoiDung, bool>> predicate)
        {
            return await _context.NguoiDungs
                .AnyAsync(predicate);
        }
    }
}
