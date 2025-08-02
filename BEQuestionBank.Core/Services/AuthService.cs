using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.user;
using BCrypt.Net;
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

        public AuthService(INguoiDungRepository nguoiDungRepository, JwtService jwtService)
        {
            _nguoiDungRepository = nguoiDungRepository;
            _jwtService = jwtService;
        }

        public async Task<NguoiDung> GetByIdAsync(Guid id)
        {
            return await _nguoiDungRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<NguoiDung>> GetAllAsync()
        {
            return await _nguoiDungRepository.GetAllAsync();
        }

        public async Task<IEnumerable<NguoiDung>> FindAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            return await _nguoiDungRepository.FindAsync(predicate);
        }

        public async Task<NguoiDung> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            return await _nguoiDungRepository.FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(NguoiDung entity)
        {
            await _nguoiDungRepository.AddAsync(entity);
        }

        public async Task UpdateAsync(NguoiDung entity)
        {
            await _nguoiDungRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(NguoiDung entity)
        {
            await _nguoiDungRepository.DeleteAsync(entity);
        }

        public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            return await _nguoiDungRepository.ExistsAsync(predicate);
        }

        public async Task<AuthResponseDto> GenerateJwtTokenAsync(string username)
        {
            var user = await _nguoiDungRepository.FirstOrDefaultAsync(u => u.TenDangNhap == username);
            if (user == null) throw new UnauthorizedAccessException("User not found");
            return _jwtService.GenerateJwtToken(username, user.VaiTro.ToString());
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            if (await _nguoiDungRepository.IsLockedAsync(username)) return false;
            var user = await _nguoiDungRepository.FirstOrDefaultAsync(u => u.TenDangNhap == username);
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.MatKhau); // So sánh mật khẩu đã hash
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _nguoiDungRepository.FirstOrDefaultAsync(u => u.TenDangNhap == registerDto.TenDangNhap);
            if (existingUser != null) return false;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.MatKhau); // Hash mật khẩu
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

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _nguoiDungRepository.FirstOrDefaultAsync(u => u.TenDangNhap == forgotPasswordDto.TenDangNhap);
            if (user == null || user.Email != forgotPasswordDto.Email) return false;

            user.MaKhoa = Guid.NewGuid();
            await _nguoiDungRepository.UpdateAsync(user);

            // Gửi email với MaKhoa (cần tích hợp email service)
            // Ví dụ: await _emailService.SendPasswordResetEmail(user.Email, user.MaKhoa);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _nguoiDungRepository.GetByResetCodeAsync(resetPasswordDto.MaKhoa);
            if (user.TenDangNhap != resetPasswordDto.TenDangNhap) return false;

            user.MatKhau = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword); // Hash mật khẩu mới
            user.MaKhoa = null;
            await _nguoiDungRepository.UpdateAsync(user);
            return true;
        }
    }
}
