using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Shared.DTOs.DeThi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeThiController : ControllerBase
    {
        private readonly IDeThiService _service;
        private readonly ILogger<DeThiController> _logger;

        public DeThiController(IDeThiService service, ILogger<DeThiController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/DeThi
        [HttpGet]
        [SwaggerOperation("Lấy danh sách tất cả đề thi")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var deThis = await _service.GetAllWithChiTietAsync();
                if (deThis == null || !deThis.Any())
                {
                    _logger.LogWarning("Không tìm thấy đề thi nào.");
                    return NotFound("Không tìm thấy đề thi nào.");
                }
                _logger.LogInformation("Lấy danh sách đề thi thành công.");
                return Ok(deThis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách đề thi");
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }

        // GET: api/DeThi/{id}
        [HttpGet("{id}")]
        [SwaggerOperation("Lấy đề thi theo ID")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID đề thi không hợp lệ: {Id}", id);
                    return BadRequest("ID đề thi không hợp lệ.");
                }

                var deThi = await _service.GetByIdWithChiTietAsync(guidId);

                if (deThi == null)
                {
                    _logger.LogWarning("Không tìm thấy đề thi với ID: {Id}", id);
                    return NotFound($"Không tìm thấy đề thi với ID: {id}");
                }
                _logger.LogInformation("Lấy đề thi thành công với ID: {Id}", id);
                return Ok(deThi);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy đề thi theo ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }

        // POST: api/DeThi
        [HttpPost]
        [SwaggerOperation("Thêm mới đề thi")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateDeThiDto createDto)
        {
            if (createDto == null)
            {
                _logger.LogError("Yêu cầu thêm đề thi không hợp lệ.");
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Dữ liệu đầu vào không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            try
            {
                var deThiDto = new DeThiDto
                {
                    MaMonHoc = createDto.MaMonHoc,
                    TenDeThi = createDto.TenDeThi,
                    DaDuyet = createDto.DaDuyet,
                    SoCauHoi = createDto.SoCauHoi,
                    NgayTao = DateTime.UtcNow,
                    NgaySua = DateTime.UtcNow,
                    ChiTietDeThis = createDto.ChiTietDeThis
                };

                var result = await _service.AddWithChiTietAsync(deThiDto);

                _logger.LogInformation("Thêm mới đề thi thành công với tên: {Name}", createDto.TenDeThi);

                return StatusCode(StatusCodes.Status201Created, new { Message = "Thêm mới đề thi thành công", Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm mới đề thi.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi thêm mới đề thi.");
            }
        }

        // PUT: api/DeThi/{id}
        [HttpPatch("{id}")]
        [SwaggerOperation("Cập nhật đề thi theo ID")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateDeThiDto updateDto)
        {
            if (updateDto == null)
            {
                _logger.LogError("Yêu cầu cập nhật đề thi không hợp lệ.");
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Dữ liệu đầu vào không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID đề thi không hợp lệ: {Id}", id);
                    return BadRequest("ID đề thi không hợp lệ.");
                }

                // Kiểm tra tồn tại
                var existing = await _service.GetByIdWithChiTietAsync(guidId);
                if (existing == null)
                {
                    _logger.LogWarning("Không tìm thấy đề thi với ID: {Id}", id);
                    return NotFound($"Không tìm thấy đề thi với ID: {id}");
                }

                // Map DTO update sang DeThiDto
                existing.MaMonHoc = updateDto.MaMonHoc;
                existing.TenDeThi = updateDto.TenDeThi;
                existing.DaDuyet = updateDto.DaDuyet;
                existing.SoCauHoi = updateDto.SoCauHoi;
                existing.NgaySua = DateTime.UtcNow;
                existing.ChiTietDeThis = updateDto.ChiTietDeThis;

                var result = await _service.UpdateWithChiTietAsync(existing);

                _logger.LogInformation("Cập nhật đề thi thành công với ID: {Id}", id);

                return Ok(new { Message = "Cập nhật đề thi thành công", Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật đề thi với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi cập nhật đề thi.");
            }
        }

        // DELETE: api/DeThi/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation("Xóa đề thi theo ID")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID đề thi không hợp lệ: {Id}", id);
                    return BadRequest("ID đề thi không hợp lệ.");
                }

                var deThi = await _service.GetByIdAsync(guidId);
                if (deThi == null)
                {
                    _logger.LogWarning("Không tìm thấy đề thi với ID: {Id}", id);
                    return NotFound($"Không tìm thấy đề thi với ID: {id}");
                }

                await _service.DeleteAsync(deThi);

                _logger.LogInformation("Xóa đề thi thành công với ID: {Id}", id);

                return Ok(new { Message = "Xóa đề thi thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa đề thi với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi xóa đề thi.");
            }
        }
        
        [HttpGet("MonHoc/{maMonHoc}")]
        [SwaggerOperation("Lấy danh sách đề thi theo mã môn học")]
        public async Task<IActionResult> GetByMaMonHocAsync(string maMonHoc)
        {
            try
            {
                if (!Guid.TryParse(maMonHoc, out var monHocId))
                {
                    _logger.LogWarning("Mã môn học không hợp lệ: {maMonHoc}", maMonHoc);
                    return BadRequest("Mã môn học không hợp lệ.");
                }
    
                var deThis = await _service.GetByMaMonHocAsync(monHocId);
                if (deThis == null || !deThis.Any())
                {
                    _logger.LogWarning("Không tìm thấy đề thi nào với mã môn học: {maMonHoc}", maMonHoc);
                    return NotFound($"Không tìm thấy đề thi nào với mã môn học: {maMonHoc}");
                }
                _logger.LogInformation("Lấy danh sách đề thi thành công với mã môn học: {maMonHoc}", maMonHoc);
                return Ok(deThis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách đề thi theo mã môn học: {maMonHoc}", maMonHoc);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }
        [HttpGet("Approved")]
        [SwaggerOperation("Lấy danh sách đề thi đã được duyệt")]
        public async Task<IActionResult> GetApprovedDeThisAsync()
        {
            try
            {
                var deThis = await _service.GetApprovedDeThisAsync();
                if (deThis == null || !deThis.Any())
                {
                    _logger.LogWarning("Không tìm thấy đề thi đã được duyệt.");
                    return NotFound("Không tìm thấy đề thi đã được duyệt.");
                }
                return Ok(deThis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách đề thi đã được duyệt.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }
        [HttpPost("ImportExcel")]
        public async Task<IActionResult> ImportMaTranFromExcelAsync(Guid maYeuCau, IFormFile excelFile)
        {
            var result = await _service.ImportMaTranFromExcelAsync(maYeuCau, excelFile);
            return Ok(result);
        }

        [HttpPost("ManualSelect")]
        public async Task<IActionResult> ManualSelectCauHoiAsync(Guid maYeuCau, [FromBody] List<Guid> maCauHoiList)
        {
            var result = await _service.ManualSelectCauHoiAsync(maYeuCau, maCauHoiList);
            return Ok(result);
        }
    }
}
