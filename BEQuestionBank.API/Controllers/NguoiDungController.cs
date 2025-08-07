using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.user;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungController : ControllerBase
    {
        private readonly INguoiDungService _userService;

        public NguoiDungController(INguoiDungService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy thông tin người dùng theo ID")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                var userDto = new NguoiDungDto
                {
                    MaNguoiDung = user.MaNguoiDung,
                    TenDangNhap = user.TenDangNhap,
                    HoTen = user.HoTen,
                    Email = user.Email,
                    VaiTro = user.VaiTro,
                    BiKhoa = user.BiKhoa
                };
                return Ok(userDto);
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
                var userDto = new NguoiDungDto
                {
                    MaNguoiDung = user.MaNguoiDung,
                    TenDangNhap = user.TenDangNhap,
                    HoTen = user.HoTen,
                    Email = user.Email,
                    VaiTro = user.VaiTro,
                    BiKhoa = user.BiKhoa
                };
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy thông tin người dùng hiện tại");
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin người dùng", chi_tiet = ex.Message });
            }
        }

        [HttpPatch("current")]
        [SwaggerOperation(Summary = "Cập nhật thông tin người dùng hiện tại")]
        [Authorize]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateCurrentUserDto userDto)
        {
            Serilog.Log.Information("Received userDto: {@UserDto}", userDto);
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Serilog.Log.Warning("ModelState invalid: {Errors}", errors);
                return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = errors });
            }

            if (userDto == null)
            {
                Serilog.Log.Warning("userDto is null");
                return BadRequest(new { message = "Dữ liệu người dùng không được để trống" });
            }

            var username = User.Identity?.Name;
            Serilog.Log.Information("Username from token: {Username}", username);
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Không xác định được người dùng" });
            }

            var user = await _userService.GetByUsernameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            user.HoTen = userDto.HoTen?.Trim() ?? user.HoTen;
            user.Email = userDto.Email?.Trim() ?? user.Email;

            if (!string.IsNullOrEmpty(user.Email) && !new EmailAddressAttribute().IsValid(user.Email))
            {
                return BadRequest(new { message = "Email không hợp lệ" });
            }

            try
            {
                var updatedUser = await _userService.UpdateAsync(user.MaNguoiDung, user);
                var updatedUserDto = new NguoiDungDto
                {
                    MaNguoiDung = updatedUser.MaNguoiDung,
                    TenDangNhap = updatedUser.TenDangNhap,
                    HoTen = updatedUser.HoTen,
                    Email = updatedUser.Email,
                    VaiTro = updatedUser.VaiTro,
                    BiKhoa = updatedUser.BiKhoa
                };
                return Ok(updatedUserDto);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi cập nhật người dùng hiện tại");
                return StatusCode(500, new { message = "Lỗi hệ thống", chi_tiet = ex.Message });
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo người dùng mới")]
        public async Task<IActionResult> CreateUser([FromBody] NguoiDungDto userDto)
        {
            try
            {
                if (userDto == null)
                {
                    return BadRequest(new { message = "Dữ liệu người dùng không được để trống" });
                }

                var user = new NguoiDung
                {
                    TenDangNhap = userDto.TenDangNhap,
                    MatKhau = userDto.TenDangNhap,
                    HoTen = userDto.HoTen,
                    Email = userDto.Email,
                    VaiTro = userDto.VaiTro,
                    BiKhoa = userDto.BiKhoa
                };

                var createdUser = await _userService.CreateAsync(user);
                var createdUserDto = new NguoiDungDto
                {
                    MaNguoiDung = createdUser.MaNguoiDung,
                    TenDangNhap = createdUser.TenDangNhap,
                    HoTen = createdUser.HoTen,
                    Email = createdUser.Email,
                    VaiTro = createdUser.VaiTro,
                    BiKhoa = createdUser.BiKhoa
                };
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.MaNguoiDung }, createdUserDto);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi tạo người dùng: {User}", userDto);
                return StatusCode(500, new { message = "Lỗi khi tạo người dùng", chi_tiet = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        [SwaggerOperation(Summary = "Cập nhật thông tin người dùng")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] NguoiDungDto userDto)
        {
            try
            {
                if (userDto == null)
                {
                    return BadRequest(new { message = "Dữ liệu người dùng không được để trống" });
                }

                var user = new NguoiDung
                {
                    TenDangNhap = userDto.TenDangNhap,
                    HoTen = userDto.HoTen,
                    Email = userDto.Email,
                    VaiTro = userDto.VaiTro,
                    BiKhoa = userDto.BiKhoa
                };

                var updatedUser = await _userService.UpdateAsync(id, user);
                var updatedUserDto = new NguoiDungDto
                {
                    MaNguoiDung = updatedUser.MaNguoiDung,
                    TenDangNhap = updatedUser.TenDangNhap,
                    HoTen = updatedUser.HoTen,
                    Email = updatedUser.Email,
                    VaiTro = updatedUser.VaiTro,
                    BiKhoa = updatedUser.BiKhoa
                };
                return Ok(updatedUserDto);
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

        [HttpGet("Active")]
        [SwaggerOperation(Summary = "Lấy danh sách người dùng đang hoạt động (không bị khóa)")]
        public async Task<IActionResult> GetUsersActive()
        {
            try
            {
                var users = await _userService.GetUsersActiveAsync();
                if (users == null || !users.Any())
                    return NotFound("Không tìm thấy người dùng đang hoạt động.");
                var userDtos = users.Select(u => new NguoiDungDto
                {
                    MaNguoiDung = u.MaNguoiDung,
                    TenDangNhap = u.TenDangNhap,
                    HoTen = u.HoTen,
                    Email = u.Email,
                    VaiTro = u.VaiTro,
                    BiKhoa = u.BiKhoa
                });
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy danh sách người dùng đang hoạt động");
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }

        [HttpGet("Locked")]
        [SwaggerOperation(Summary = "Lấy danh sách người dùng bị khóa")]
        public async Task<IActionResult> GetUsersLocked()
        {
            try
            {
                var users = await _userService.GetUsersLockedAsync();
                if (users == null || !users.Any())
                    return NotFound("Không tìm thấy người dùng bị khóa.");
                var userDtos = users.Select(u => new NguoiDungDto
                {
                    MaNguoiDung = u.MaNguoiDung,
                    TenDangNhap = u.TenDangNhap,
                    HoTen = u.HoTen,
                    Email = u.Email,
                    VaiTro = u.VaiTro,
                    BiKhoa = u.BiKhoa
                });
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy danh sách người dùng bị khóa");
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }

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