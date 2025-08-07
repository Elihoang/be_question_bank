using BCrypt.Net;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.user;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEQuestionBank.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly INguoiDungRepository _nguoiDungRepository;
        private readonly JwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _memoryCache;

        public AuthService(INguoiDungRepository nguoiDungRepository, JwtService jwtService, IEmailService emailService, IMemoryCache memoryCache)
        {
            _nguoiDungRepository = nguoiDungRepository;
            _jwtService = jwtService;
            _emailService = emailService;
            _memoryCache = memoryCache;
        }

        public async Task<NguoiDung> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _nguoiDungRepository.GetByIdAsync(id);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy người dùng");
                }
                return user;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy thông tin người dùng theo ID: {MaNguoiDung}", id);
                throw;
            }
        }

        public async Task<IEnumerable<NguoiDung>> GetAllAsync()
        {
            try
            {
                return await _nguoiDungRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy danh sách tất cả người dùng");
                throw;
            }
        }

        public async Task<IEnumerable<NguoiDung>> FindAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            try
            {
                return await _nguoiDungRepository.FindAsync(predicate);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi tìm kiếm người dùng theo điều kiện");
                throw;
            }
        }

        public async Task<NguoiDung> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            try
            {
                return await _nguoiDungRepository.FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy người dùng đầu tiên theo điều kiện");
                throw;
            }
        }

        public async Task AddAsync(NguoiDung entity)
        {
            try
            {
                await _nguoiDungRepository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi thêm người dùng");
                throw;
            }
        }

        public async Task UpdateAsync(NguoiDung entity)
        {
            try
            {
                await _nguoiDungRepository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi cập nhật người dùng");
                throw;
            }
        }

        public async Task DeleteAsync(NguoiDung entity)
        {
            try
            {
                await _nguoiDungRepository.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi xóa người dùng");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            try
            {
                return await _nguoiDungRepository.ExistsAsync(predicate);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi kiểm tra sự tồn tại của người dùng");
                throw;
            }
        }

        public async Task<AuthResponseDto> GenerateJwtTokenAsync(string username)
        {
            try
            {
                var user = await _nguoiDungRepository.FirstOrDefaultAsync(u => u.TenDangNhap == username);
                if (user == null) throw new UnauthorizedAccessException("User not found");
                return _jwtService.GenerateJwtToken(username, user.VaiTro.ToString());
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi tạo token JWT cho người dùng: {Username}", username);
                throw;
            }
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            try
            {
                if (await _nguoiDungRepository.IsLockedAsync(username))
                {
                    throw new UnauthorizedAccessException("Tài khoản đã bị khóa");
                }
                var user = await _nguoiDungRepository.FirstOrDefaultAsync(u => u.TenDangNhap == username);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không hợp lệ");
                }
                return BCrypt.Net.BCrypt.Verify(password, user.MatKhau);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi xác thực người dùng: {Username}", username);
                throw;
            }
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var existingUser = await _nguoiDungRepository.FirstOrDefaultAsync(u => u.TenDangNhap == registerDto.TenDangNhap);
                if (existingUser != null) return false;

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.MatKhau);
                var newUser = new NguoiDung
                {
                    MaNguoiDung = Guid.NewGuid(),
                    TenDangNhap = registerDto.TenDangNhap,
                    MatKhau = hashedPassword,
                    HoTen = registerDto.HoTen,
                    Email = registerDto.Email,
                    VaiTro = registerDto.VaiTro,
                    BiKhoa = false
                };

                await _nguoiDungRepository.AddAsync(newUser);
                return true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi đăng ký người dùng: {TenDangNhap}", registerDto.TenDangNhap);
                throw;
            }
        }

        public async Task<bool> SendOtpAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await _nguoiDungRepository.FirstOrDefaultAsync(u => u.TenDangNhap == forgotPasswordDto.TenDangNhap);
                if (user == null || user.Email != forgotPasswordDto.Email)
                {
                    return false;
                }

                // Tạo mã OTP 6 số ngẫu nhiên
                var otp = new Random().Next(100000, 999999).ToString();
                var otpKey = $"OTP_{user.MaNguoiDung}"; // Khóa cache dựa trên MaNguoiDung

                // Lưu OTP vào cache với thời gian hết hạn 10 phút
                _memoryCache.Set(otpKey, otp, TimeSpan.FromMinutes(10));

                // Không cần cập nhật MaKhoa nếu không liên quan đến OTP
                await _nguoiDungRepository.UpdateAsync(user); // Chỉ cập nhật nếu cần

                // Gửi email với OTP
                await _emailService.SendOtpEmail(user.Email, otp);
                return true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi gửi OTP cho người dùng: {TenDangNhap}", forgotPasswordDto.TenDangNhap);
                throw;
            }
        }

        public async Task<bool> ResetPasswordWithOtpAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _nguoiDungRepository.FirstOrDefaultAsync(u => u.TenDangNhap == resetPasswordDto.TenDangNhap);
                if (user == null)
                {
                    return false;
                }

                var otpKey = $"OTP_{user.MaNguoiDung}";
                if (!_memoryCache.TryGetValue(otpKey, out string cachedOtp) || cachedOtp != resetPasswordDto.Otp)
                {
                    return false;
                }

                _memoryCache.Remove(otpKey);
                user.MatKhau = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
                await _nguoiDungRepository.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi đặt lại mật khẩu cho người dùng: {TenDangNhap}", resetPasswordDto.TenDangNhap);
                throw;
            }
        }
    }
}
