using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Shared.DTOs.user;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Đăng nhập")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (await _authService.ValidateUserAsync(loginDto.TenDangNhap, loginDto.MatKhau))
                {
                    var response = await _authService.GenerateJwtTokenAsync(loginDto.TenDangNhap);
                    if (response == null)
                    {
                        return Unauthorized(new { error = "Tên đăng nhập hoặc mật khẩu không hợp lệ hoặc tài khoản bị khóa" });
                    }
                    return Ok(response);
                }
                return Unauthorized(new { error = "Tên đăng nhập hoặc mật khẩu không hợp lệ hoặc tài khoản bị khóa" });
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { error = "Đã xảy ra lỗi trong quá trình đăng nhập", details = ex.Message });
            }
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Đăng kí")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (await _authService.RegisterAsync(registerDto))
                {
                    return Ok("Đăng ký thành công");
                }
                return BadRequest("Tên đăng nhập đã tồn tại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình đăng ký");
            }
        }

        [HttpPost("forgot-password")]
        [SwaggerOperation(Summary = "Yêu cầu mã OTP để đặt lại mật khẩu")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (await _authService.SendOtpAsync(forgotPasswordDto))
                {
                    return Ok("Mã OTP đã được gửi đến email của bạn");
                }
                return BadRequest("Tên đăng nhập hoặc email không hợp lệ");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Đã xảy ra lỗi trong quá trình yêu cầu mã OTP", details = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        [SwaggerOperation(Summary = "Đặt lại mật khẩu bằng mã OTP")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (await _authService.ResetPasswordWithOtpAsync(resetPasswordDto))
                {
                    return Ok("Đặt lại mật khẩu thành công");
                }
                return BadRequest("Mã OTP hoặc tên đăng nhập không hợp lệ");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Đã xảy ra lỗi trong quá trình đặt lại mật khẩu", details = ex.Message });
            }
        }
    }
}
