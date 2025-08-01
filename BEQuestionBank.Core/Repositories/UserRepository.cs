using BEQuestionBank.Core.Configurations;
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
    public class UserRepository : GenericRepository<NguoiDung>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<NguoiDung> GetByIdAsync(Guid maNguoiDung)
        {
            try
            {
                var user = await _dbSet.AsNoTracking().FirstOrDefaultAsync(nd => nd.MaNguoiDung == maNguoiDung);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy người dùng");
                }
                return user;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu có Serilog cấu hình)
                Serilog.Log.Error(ex, "Lỗi khi lấy thông tin người dùng theo ID: {MaNguoiDung}", maNguoiDung);
                throw; // Ném lại ngoại lệ để xử lý ở tầng trên
            }
        }
        public async Task<NguoiDung> GetByUsernameAsync(string tenDangNhap)
        {
            try
            {
                var user = await _dbSet.AsNoTracking().FirstOrDefaultAsync(nd => nd.TenDangNhap == tenDangNhap);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy người dùng với tên đăng nhập này");
                }
                return user;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy thông tin người dùng theo TenDangNhap: {TenDangNhap}", tenDangNhap);
                throw;
            }
        }
    }
}
