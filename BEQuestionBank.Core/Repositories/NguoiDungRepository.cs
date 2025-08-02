using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEQuestionBank.Core.Repositories
{
    public class NguoiDungRepository : GenericRepository<NguoiDung>, INguoiDungRepository
    {
        public NguoiDungRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<NguoiDung>> GetByVaiTroAsync(VaiTroNguoiDung vaiTro)
        {
            return await _dbSet.AsNoTracking().Where(nd => nd.VaiTro == vaiTro).ToListAsync();
        }

        public Task<IEnumerable<NguoiDung>> GetByKhoaAsync(string maKhoa)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<NguoiDung>> GetByKhoaAsync(Guid maKhoa)
        {
            return await _dbSet.AsNoTracking().Where(nd => nd.MaKhoa == maKhoa).ToListAsync();
        }

        public async Task<bool> IsLockedAsync(string tenDangNhap)
        {
            var user = await _dbSet.AsNoTracking().FirstOrDefaultAsync(nd => nd.TenDangNhap == tenDangNhap);
            return user?.BiKhoa ?? false;
        }

        public async Task<NguoiDung> GetByResetCodeAsync(Guid maKhoa)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(nd => nd.MaKhoa == maKhoa) ?? throw new Exception("Invalid reset code");
        }
    }
}
