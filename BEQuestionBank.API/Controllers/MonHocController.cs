using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonHocController(IMonHocService service, ILogger<MonHocController> logger) : ControllerBase
    {
        private readonly IMonHocService _service = service;
        private readonly ILogger<MonHocController> _logger = logger;

        [HttpGet("{id}")]
        [SwaggerOperation("Tìm môn học theo mã")]
        public async Task<ActionResult<MonHoc>> GetByIdAsync(string id)
        {
            try
            {
                var monHoc = await _service.GetByIdAsync(Guid.Parse(id));
                if (monHoc == null)
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy môn học với mã {id}");

                return StatusCode(StatusCodes.Status200OK, monHoc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm môn học");
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi máy chủ");
            }
        }

        [HttpGet]
        [SwaggerOperation("Lấy danh sách tất cả môn học")]
        public async Task<IEnumerable<MonHoc>> GetAllAsync()
        {
            var list = await _service.GetAllAsync();
            return list;
        }

        [HttpGet("khoa/{maKhoa}")]
        [SwaggerOperation("Lấy danh sách môn học theo mã khoa")]
        public async Task<IEnumerable<MonHocDto>> GetByMaKhoaAsync(string maKhoa)
        {
            var ds = await _service.GetByMaKhoaAsync(Guid.Parse(maKhoa));
            return ds.Select(m => new MonHocDto
            {
                MaMonHoc = m.MaMonHoc,
                MaSoMonHoc = m.MaSoMonHoc,
                TenMonHoc = m.TenMonHoc,
                MaKhoa = m.MaKhoa,
                XoaTam = m.XoaTam
            });
        }

        [HttpPost]
        [SwaggerOperation("Thêm mới môn học")]
        public async Task<IActionResult> CreateAsync([FromBody] MonHocCreateDto model)
        {
            try
            {
                var monHoc = new MonHoc
                {
                    MaSoMonHoc = model.MaSoMonHoc,
                    TenMonHoc = model.TenMonHoc,
                    MaKhoa = model.MaKhoa,
                    XoaTam = model.XoaTam ?? false
                };

                await _service.AddAsync(monHoc);
                return StatusCode(StatusCodes.Status201Created, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm môn học");
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi máy chủ");
            }
        }

        [HttpPatch("{id}")]
        [SwaggerOperation("Cập nhật thông tin môn học")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] MonHocUpdateDto model)
        {
            try
            {
                var monHoc = await _service.GetByIdAsync(Guid.Parse(id));
                if (monHoc == null)
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy môn học với mã {id}");

                monHoc.TenMonHoc = model.TenMonHoc;
                monHoc.MaKhoa = model.MaKhoa;
                monHoc.XoaTam = model.XoaTam ?? monHoc.XoaTam;

                await _service.UpdateAsync(monHoc);

                return StatusCode(StatusCodes.Status200OK, new MonHocDto
                {
                    MaMonHoc = monHoc.MaMonHoc,
                    TenMonHoc = monHoc.TenMonHoc,
                    MaKhoa = monHoc.MaKhoa,
                    XoaTam = monHoc.XoaTam
                });
            }
            catch (FormatException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "ID không đúng định dạng GUID.");
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi khi cập nhật môn học: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        [SwaggerOperation("Xóa môn học")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var monHoc = await _service.GetByIdAsync(Guid.Parse(id));
            if (monHoc == null)
                return NotFound($"Không tìm thấy môn học với mã {id}");

            await _service.DeleteAsync(monHoc);
            return StatusCode(StatusCodes.Status200OK, $"Đã xóa môn học có mã {id}");
        }

        [HttpPatch("{id}/XoaTam")]
        [SwaggerOperation("Xóa tạm thời môn học")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var monHoc = await _service.GetByIdAsync(Guid.Parse(id));
            if (monHoc == null)
                return NotFound($"Không tìm thấy môn học với mã {id}");

            monHoc.XoaTam = true;
            await _service.UpdateAsync(monHoc);
            return StatusCode(StatusCodes.Status200OK, $"Đã xóa tạm thời môn học: {monHoc.TenMonHoc}");
        }

        [HttpPatch("{id}/KhoiPhuc")]
        [SwaggerOperation("Khôi phục môn học đã xóa tạm")]
        public async Task<IActionResult> Restore(string id)
        {
            var monHoc = await _service.GetByIdAsync(Guid.Parse(id));
            if (monHoc == null)
                return NotFound($"Không tìm thấy môn học với mã {id}");

            monHoc.XoaTam = false;
            await _service.UpdateAsync(monHoc);
            return StatusCode(StatusCodes.Status200OK, $"Đã khôi phục môn học: {monHoc.TenMonHoc}");
        }
    }
}
