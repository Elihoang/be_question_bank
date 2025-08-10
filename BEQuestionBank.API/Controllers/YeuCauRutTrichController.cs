using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YeuCauRutTrichController : ControllerBase
    {
        private readonly IYeuCauRutTrichService _service;
        private readonly ILogger<YeuCauRutTrichController> _logger;

        public YeuCauRutTrichController(IYeuCauRutTrichService service , ILogger<YeuCauRutTrichController> logger)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy yêu cầu rút trích theo mã")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                return Ok(new YeuCauRutTrichDto
                {
                    MaYeuCau = result.MaYeuCau,
                    MaNguoiDung = result.MaNguoiDung,
                    MaMonHoc = result.MaMonHoc,
                    NoiDungRutTrich = result.NoiDungRutTrich,
                    GhiChu = result.GhiChu,
                    NgayYeuCau = result.NgayYeuCau,
                    NgayXuLy = result.NgayXuLy,
                    DaXuLy = result.DaXuLy
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy yêu cầu rút trích theo mã.");
                return StatusCode(500,
                    new { message = "Lỗi hệ thống khi lấy yêu cầu rút trích", chi_tiet = ex.Message });
            }
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy tất cả yêu cầu rút trích")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result.Select(e => new YeuCauRutTrichDto
                {
                    MaYeuCau = e.MaYeuCau,
                    MaNguoiDung = e.MaNguoiDung,
                    MaMonHoc = e.MaMonHoc,
                    NoiDungRutTrich = e.NoiDungRutTrich,
                    GhiChu = e.GhiChu,
                    NgayYeuCau = e.NgayYeuCau,
                    NgayXuLy = e.NgayXuLy,
                    DaXuLy = e.DaXuLy,
                    TenNguoiDung = e.NguoiDung?.TenDangNhap,
                }));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy danh sách yêu cầu rút trích.");
                return StatusCode(500,
                    new { message = "Lỗi hệ thống khi lấy danh sách yêu cầu rút trích", chi_tiet = ex.Message });
            }
        }

        [HttpGet("NguoiDung/{maNguoiDung}")]
        [SwaggerOperation(Summary = "Lấy yêu cầu rút trích theo mã người dùng")]
        public async Task<IActionResult> GetByMaNguoiDung(Guid maNguoiDung)
        {
            try
            {
                var result = await _service.GetByMaNguoiDungAsync(maNguoiDung);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy yêu cầu rút trích theo mã người dùng.");
                return StatusCode(500,
                    new { message = "Lỗi hệ thống khi lấy yêu cầu theo mã người dùng", chi_tiet = ex.Message });
            }
        }

        [HttpGet("MonHoc/{maMonHoc}")]
        [SwaggerOperation(Summary = "Lấy yêu cầu rút trích theo mã môn học")]
        public async Task<IActionResult> GetByMaMonHoc(Guid maMonHoc)
        {
            try
            {
                var result = await _service.GetByMaMonHocAsync(maMonHoc);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy yêu cầu rút trích theo mã môn học.");
                return StatusCode(500,
                    new { message = "Lỗi hệ thống khi lấy yêu cầu theo mã môn học", chi_tiet = ex.Message });
            }
        }

        [HttpGet("ChuaXuLy")]
        [SwaggerOperation(Summary = "Lấy danh sách yêu cầu rút trích chưa xử lý")]
        public async Task<IActionResult> GetChuaXuLy()
        {
            try
            {
                var result = await _service.GetChuaXuLyAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy danh sách yêu cầu rút trích chưa xử lý.");
                return StatusCode(500,
                    new { message = "Lỗi hệ thống khi lấy danh sách yêu cầu chưa xử lý", chi_tiet = ex.Message });
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo yêu cầu rút trích mới")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateYeuCauRutTrichDto YeuCauRutTrichDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Dữ liệu không hợp lệ.",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var entity = new YeuCauRutTrich
                {
                    MaNguoiDung = YeuCauRutTrichDto.MaNguoiDung,
                    MaMonHoc = YeuCauRutTrichDto.MaMonHoc,
                    NoiDungRutTrich = YeuCauRutTrichDto.NoiDungRutTrich,
                    GhiChu = YeuCauRutTrichDto.GhiChu,
                    NgayYeuCau = DateTime.UtcNow,
                    NgayXuLy = null,
                    DaXuLy = false
                };

                await _service.AddAsync(entity);
                _logger.LogInformation("Thêm mới câu hỏi thành công với MaYeuCau: {MaYeuCau}", entity.MaYeuCau);
                return StatusCode(StatusCodes.Status201Created,
                    new { Message = "Thêm mới câu hỏi thành công", Data = entity });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm mới câu hỏi.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi thêm mới câu hỏi.");
            }
        }
    

    [HttpPatch("{id}")]
        [SwaggerOperation(Summary = "Cập nhật yêu cầu rút trích")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateYeuCauRutTrichDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }

                var entity = await _service.GetByIdAsync(id);
                entity.MaNguoiDung = dto.MaNguoiDung;
                entity.MaMonHoc = dto.MaMonHoc;
                entity.NoiDungRutTrich = dto.NoiDungRutTrich;
                entity.GhiChu = dto.GhiChu;
                entity.NgayYeuCau = dto.NgayYeuCau ?? entity.NgayYeuCau;
                entity.NgayXuLy = dto.NgayXuLy;
                entity.DaXuLy = dto.DaXuLy ?? entity.DaXuLy;

                await _service.UpdateAsync(entity);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi cập nhật yêu cầu rút trích.");
                return StatusCode(500, new { message = "Lỗi hệ thống khi cập nhật yêu cầu rút trích", chi_tiet = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa yêu cầu rút trích")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var entity = await _service.GetByIdAsync(id);
                await _service.DeleteAsync(entity);
                return NoContent();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi xóa yêu cầu rút trích.");
                return StatusCode(500, new { message = "Lỗi hệ thống khi xóa yêu cầu rút trích", chi_tiet = ex.Message });
            }
        }
        [HttpPatch("{id}/status")]
        [SwaggerOperation(Summary = "Thay đổi trạng thái xử lý của yêu cầu rút trích")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] bool daXuLy)
        {
            try
            {
                var result = await _service.ChangerStatusAsync(id, daXuLy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi thay đổi trạng thái yêu cầu rút trích.");
                return StatusCode(500, new { message = "Lỗi hệ thống khi thay đổi trạng thái yêu cầu rút trích", chi_tiet = ex.Message });
            }
        }
    }
}