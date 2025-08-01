using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy thông tin người dùng với ID: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var username = User.Identity.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "Không xác định được người dùng" });
                }

                var user = await _userService.GetByUsernameAsync(username);
                return Ok(new
                {
                    MaNguoiDung = user.MaNguoiDung,
                    TenDangNhap = user.TenDangNhap,
                    HoTen = user.HoTen,
                    Email = user.Email,
       
                    VaiTro = user.VaiTro,
             
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy thông tin người dùng hiện tại");
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin người dùng", chi_tiet = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] NguoiDung user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new { message = "Dữ liệu người dùng không được để trống" });
                }

                var createdUser = await _userService.CreateAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.MaNguoiDung }, createdUser);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi tạo người dùng: {User}", user);
                return StatusCode(500, new { message = "Lỗi khi tạo người dùng", chi_tiet = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] NguoiDung user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new { message = "Dữ liệu người dùng không được để trống" });
                }

                var updatedUser = await _userService.UpdateAsync(id, user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi cập nhật người dùng với ID: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var result = await _userService.DeleteAsync(id);
                if (result)
                {
                    return NoContent();
                }
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi xóa người dùng với ID: {Id}", id);
                return StatusCode(500, new { message = "Lỗi khi xóa người dùng", chi_tiet = ex.Message });
            }
        }
    }
}