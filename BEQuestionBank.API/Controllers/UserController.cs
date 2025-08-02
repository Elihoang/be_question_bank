using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.user;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Lấy thông] tin người dùng theo ID")]
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
        [SwaggerOperation(Summary = "Lấy thông tin người dùng hiện tại")]
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
        [SwaggerOperation(Summary = "Tạo người dùng mới")]
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

        [HttpPatch("{id}")]
        [SwaggerOperation(Summary = "Cập nhật thông tin người dùng")]
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
        [SwaggerOperation(Summary = "Xóa người dùng")]
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
        // GET: api/User/Active
        [HttpGet("Active")]
        [SwaggerOperation(Summary = "Lấy danh sách người dùng đang hoạt động (không bị khóa)")]
        public async Task<IActionResult> GetUsersActive()
        {
            try
            {
                var users = await _userService.GetUsersActiveAsync();
                if (users == null || !users.Any())
                    return NotFound("Không tìm thấy người dùng đang hoạt động.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy danh sách người dùng đang hoạt động");
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }

        // GET: api/User/Locked
        [HttpGet("Locked")]
        [SwaggerOperation(Summary = "Lấy danh sách người dùng bị khóa")]
        public async Task<IActionResult> GetUsersLocked()
        {
            try
            {
                var users = await _userService.GetUsersLockedAsync();
                if (users == null || !users.Any())
                    return NotFound("Không tìm thấy người dùng bị khóa.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy danh sách người dùng bị khóa");
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }

        // POST: api/User/Lock/{id}
        [HttpPost("Lock/{id}")]
        [SwaggerOperation(Summary = "Khóa người dùng theo ID")]
        public async Task<IActionResult> LockUser(Guid id)
        {
            try
            {
                var success = await _userService.LockUserAsync(id);
                if (!success)
                    return NotFound("Không tìm thấy người dùng để khóa.");
                return Ok(new { Message = "Khóa người dùng thành công" });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi khóa người dùng với ID: {Id}", id);
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }

        // POST: api/User/Unlock/{id}
        [HttpPost("Unlock/{id}")]
        [SwaggerOperation(Summary = "Mở khóa người dùng theo ID")]
        public async Task<IActionResult> UnlockUser(Guid id)
        {
            try
            {
                var success = await _userService.UnlockUserAsync(id);
                if (!success)
                    return NotFound("Không tìm thấy người dùng để mở khóa.");
                return Ok(new { Message = "Mở khóa người dùng thành công" });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi mở khóa người dùng với ID: {Id}", id);
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }
        [HttpPost("Import")]
        [SwaggerOperation(Summary = "Nhập danh sách người dùng từ file Excel")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportUsers([FromForm] ImportUserFileDto model)
        {
            try
            {
                if (model.File == null || model.File.Length == 0)
                {
                    return BadRequest(new { message = "File không được trống hoặc không hợp lệ." });
                }

                if (!model.File.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "Định dạng file phải là .xlsx." });
                }

                var (successCount, errors) = await _userService.ImportUsersFromExcelAsync(model.File);

                if (errors.Any())
                {
                    return Ok(new
                    {
                        SuccessCount = successCount,
                        ErrorCount = errors.Count,
                        Errors = errors,
                        Message = successCount > 0 
                            ? $"Nhập thành công {successCount} người dùng, nhưng có {errors.Count} lỗi."
                            : "Không nhập được người dùng nào do có lỗi."
                    });
                }

                return Ok(new
                {
                    SuccessCount = successCount,
                    ErrorCount = 0,
                    Errors = new List<string>(),
                    Message = $"Nhập thành công {successCount} người dùng."
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi nhập người dùng từ file Excel.");
                return StatusCode(500, new { message = "Lỗi hệ thống khi nhập người dùng", chi_tiet = ex.Message });
            }
        }

    }
}