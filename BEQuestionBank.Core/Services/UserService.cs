using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEQuestionBank.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<NguoiDung> GetByIdAsync(Guid maNguoiDung)
        {
            try
            {
                return await _userRepository.GetByIdAsync(maNguoiDung);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy thông tin người dùng theo ID: {MaNguoiDung}", maNguoiDung);
                throw;
            }
        }

        public async Task<NguoiDung> CreateAsync(NguoiDung user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "Dữ liệu người dùng không được để trống");
                }

                user.MaNguoiDung = Guid.NewGuid();
                user.MatKhau = BCrypt.Net.BCrypt.HashPassword(user.MatKhau);
                await _userRepository.AddAsync(user);
                return user;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi tạo người dùng: {User}", user);
                throw;
            }
        }

        public async Task<NguoiDung> UpdateAsync(Guid maNguoiDung, NguoiDung user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "Dữ liệu người dùng không được để trống");
                }

                var existingUser = await _userRepository.GetByIdAsync(maNguoiDung);
                if (existingUser == null)
                {
                    throw new Exception("Không tìm thấy người dùng");
                }

                existingUser.TenDangNhap = user.TenDangNhap;
                if (!string.IsNullOrEmpty(user.MatKhau))
                {
                    existingUser.MatKhau = BCrypt.Net.BCrypt.HashPassword(user.MatKhau);
                }
                existingUser.HoTen = user.HoTen;
                existingUser.Email = user.Email;
                existingUser.VaiTro = user.VaiTro;
                existingUser.BiKhoa = user.BiKhoa;
                existingUser.MaKhoa = user.MaKhoa;

                await _userRepository.UpdateAsync(existingUser);
                return existingUser;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi cập nhật người dùng với ID: {MaNguoiDung}", maNguoiDung);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid maNguoiDung)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(maNguoiDung);
                if (user == null)
                {
                    return false;
                }

                await _userRepository.DeleteAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi xóa người dùng với ID: {MaNguoiDung}", maNguoiDung);
                throw;
            }
        }

        public async Task<NguoiDung> GetByUsernameAsync(string tenDangNhap)
        {
            try
            {
                return await _userRepository.GetByUsernameAsync(tenDangNhap);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy thông tin người dùng theo TenDangNhap: {TenDangNhap}", tenDangNhap);
                throw;
            }
        }
        public async Task<IEnumerable<NguoiDung>> GetAllAsync()
        {
            try
            {
                return await _userRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy tất cả thông tin người dùng");
                throw;
            }
        }

        public async Task<IEnumerable<NguoiDung>> FindAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            try
            {
                return await _userRepository.FindAsync(predicate);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi tìm kiếm người dùng với điều kiện");
                throw;
            }
        }

        public async Task<NguoiDung> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            try
            {
                return await _userRepository.FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi tìm người dùng đầu tiên với điều kiện");
                throw;
            }
        }

        public async Task AddAsync(NguoiDung entity)
        {
            try
            {
                await _userRepository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi thêm người dùng: {User}", entity);
                throw;
            }
        }

        public async Task UpdateAsync(NguoiDung entity)
        {
            try
            {
                await _userRepository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi cập nhật người dùng: {User}", entity);
                throw;
            }
        }

        public async Task DeleteAsync(NguoiDung entity)
        {
            try
            {
                await _userRepository.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi xóa người dùng: {User}", entity);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            try
            {
                return await _userRepository.ExistsAsync(predicate);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi kiểm tra sự tồn tại của người dùng với điều kiện");
                throw;
            }
        }
    }
}