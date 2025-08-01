using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEQuestionBank.Domain.Interfaces.Service
{
    public interface IAuthService : IService<NguoiDung>
    {
        Task<AuthResponseDto> GenerateJwtTokenAsync(string username);
        Task<bool> ValidateUserAsync(string username, string password);
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
