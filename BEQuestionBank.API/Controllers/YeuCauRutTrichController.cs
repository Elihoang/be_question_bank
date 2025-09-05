
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BEQuestionBank.Shared.DTOs.DeThi;
using Newtonsoft.Json;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YeuCauRutTrichController : ControllerBase
    {
        private readonly IYeuCauRutTrichService _service;
        private readonly IDeThiService _deThiService;
        private readonly ILogger<YeuCauRutTrichController> _logger;

        public YeuCauRutTrichController(IYeuCauRutTrichService service, ILogger<YeuCauRutTrichController> logger, IDeThiService deThiService)
        {
            _logger = logger;
            _service = service;
            _deThiService = deThiService;
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
                    DaXuLy = result.DaXuLy,
                    TenNguoiDung = result.NguoiDung?.TenDangNhap,
                    TenMonHoc = result.MonHoc?.TenMonHoc,
                    TenKhoa = result.MonHoc?.Khoa?.TenKhoa,
                    MaTran = result.MaTran
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy yêu cầu rút trích theo mã.");
                return StatusCode(500, new { message = "Lỗi hệ thống khi lấy yêu cầu rút trích", chi_tiet = ex.Message });
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
                    TenMonHoc = e.MonHoc?.TenMonHoc,
                    TenKhoa = e.MonHoc?.Khoa?.TenKhoa,
                    MaTran = e.MaTran
                }));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy danh sách yêu cầu rút trích.");
                return StatusCode(500, new { message = "Lỗi hệ thống khi lấy danh sách yêu cầu rút trích", chi_tiet = ex.Message });
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
                return StatusCode(500, new { message = "Lỗi hệ thống khi lấy yêu cầu theo mã người dùng", chi_tiet = ex.Message });
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
                return StatusCode(500, new { message = "Lỗi hệ thống khi lấy yêu cầu theo mã môn học", chi_tiet = ex.Message });
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
                return StatusCode(500, new { message = "Lỗi hệ thống khi lấy danh sách yêu cầu chưa xử lý", chi_tiet = ex.Message });
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

        /// <summary>
        /// Tạo yêu cầu rút trích mới và tự động rút trích đề thi nếu có ma trận JSON.
        /// </summary>
        /// <param name="requestDto">Yêu cầu rút trích với MaTran dạng JSON.</param>
        /// <returns>Yêu cầu vừa tạo và đề thi (nếu được rút trích).</returns>
        [HttpPost]
        public async Task<ActionResult<object>> CreateYeuCau([FromBody] YeuCauRutTrichDto requestDto)
        {
            if (requestDto == null)
                return BadRequest("Dữ liệu yêu cầu không hợp lệ.");

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var entity = new YeuCauRutTrich
                {
                    MaNguoiDung = requestDto.MaNguoiDung,
                    MaMonHoc = requestDto.MaMonHoc,
                    NoiDungRutTrich = requestDto.NoiDungRutTrich,
                    GhiChu = requestDto.GhiChu,
                    NgayYeuCau = DateTime.UtcNow,
                    DaXuLy = false,
                    MaTran = requestDto.MaTran
                };

                var result = await _service.AddAsync(entity);

                DeThiDto deThi = null;
                if (!string.IsNullOrWhiteSpace(result.MaTran))
                {
                    try
                    {
                        deThi = await _deThiService.RutTrichDeThiFromYeuCauAsync(result.MaYeuCau);
                        // Cập nhật trạng thái DaXuLy và NgayXuLy nếu rút trích thành công
                        result.DaXuLy = true;
                        result.NgayXuLy = DateTime.UtcNow;
                        await _service.UpdateAsync(result);
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Error(ex, "Lỗi khi rút trích đề thi cho yêu cầu {MaYeuCau}", result.MaYeuCau);
                        return StatusCode(500, new
                        {
                            message = "Tạo yêu cầu thành công nhưng lỗi khi rút trích đề thi",
                            yeuCau = result,
                            chi_tiet = ex.Message
                        });
                    }
                }

                var dto = new YeuCauRutTrichDto
                {
                    MaYeuCau = result.MaYeuCau,
                    MaNguoiDung = result.MaNguoiDung,
                    MaMonHoc = result.MaMonHoc,
                    NoiDungRutTrich = result.NoiDungRutTrich,
                    GhiChu = result.GhiChu,
                    NgayYeuCau = result.NgayYeuCau,
                    NgayXuLy = result.NgayXuLy,
                    DaXuLy = result.DaXuLy,
                    TenNguoiDung = result.NguoiDung?.TenDangNhap,
                    TenMonHoc = result.MonHoc?.TenMonHoc,
                    TenKhoa = result.MonHoc?.Khoa?.TenKhoa,
                    MaTran = result.MaTran
                };

                return Ok(new { YeuCauRutTrich = dto, DeThi = deThi });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi tạo yêu cầu rút trích.");
                return StatusCode(500, new { message = "Lỗi hệ thống khi tạo yêu cầu rút trích", chi_tiet = ex.Message });
            }
        }
    }
}
