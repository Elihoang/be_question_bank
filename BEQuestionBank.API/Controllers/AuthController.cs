using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Shared.DTOs.user;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (await _authService.ValidateUserAsync(loginDto.TenDangNhap, loginDto.MatKhau))
            {
                var response = await _authService.GenerateJwtTokenAsync(loginDto.TenDangNhap);
                return Ok(response);
            }

            return Unauthorized("Invalid credentials or account locked");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (await _authService.RegisterAsync(registerDto))
            {
                return Ok("Registration successful");
            }
            return BadRequest("Username already exists");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (await _authService.ForgotPasswordAsync(forgotPasswordDto))
            {
                return Ok("Password reset link sent");
            }
            return BadRequest("Invalid username or email");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (await _authService.ResetPasswordAsync(resetPasswordDto))
            {
                return Ok("Password reset successful");
            }
            return BadRequest("Invalid token or username");
        }
    }
}
