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
using BEQuestionBank.Shared.DTOs;

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
                    NgayCapNhap = DateTime.UtcNow,
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
                existing.NgayCapNhap = DateTime.UtcNow;

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
        [SwaggerOperation("Nhập ma trận từ file Excel")]
        public async Task<IActionResult> ImportMaTranFromExcelAsync(Guid maYeuCau, IFormFile excelFile)
        {
            var result = await _service.ImportMaTranFromExcelAsync(maYeuCau, excelFile);
            return Ok(result);
        }

       
        [HttpPatch("{id}/ChangeStatus")]
        [SwaggerOperation("Thay đổi trạng thái duyệt của đề thi")]
        public async Task<IActionResult> ChangeStatusAsync(string id, [FromBody] bool daDuyet)
        {
            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID đề thi không hợp lệ: {Id}", id);
                    return BadRequest("ID đề thi không hợp lệ.");
                }

                var updatedDeThi = await _service.ChangerStatusAsync(guidId, daDuyet);
                if (updatedDeThi == null)
                {
                    _logger.LogWarning("Không tìm thấy đề thi với ID: {Id}", id);
                    return NotFound($"Không tìm thấy đề thi với ID: {id}");
                }

                _logger.LogInformation("Thay đổi trạng thái duyệt của đề thi thành công với ID: {Id}", id);
                return Ok(new { Message = "Thay đổi trạng thái duyệt thành công", Data = updatedDeThi });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thay đổi trạng thái duyệt của đề thi với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi thay đổi trạng thái duyệt.");
            }
        }
        
        [HttpGet("{maDeThi}/CauTraLoi")]
        [SwaggerOperation("Lấy danh sách câu trả lời của câu hỏi trong đề thi")]
        public async Task<IActionResult> GetCauTraLoiByDeThiAsync(string maDeThi)
        {
            try
            {
                if (!Guid.TryParse(maDeThi, out var guidId))
                {
                    _logger.LogWarning("ID đề thi không hợp lệ: {maDeThi}", maDeThi);
                    return BadRequest("ID đề thi không hợp lệ.");
                }

                var cauTraLoiList = await _service.GetCauTraLoiByDeThiAsync(guidId);
                if (cauTraLoiList == null || !cauTraLoiList.Any())
                {
                    _logger.LogWarning("Không tìm thấy câu trả lời nào cho đề thi với ID: {maDeThi}", maDeThi);
                    return NotFound($"Không tìm thấy câu trả lời nào cho đề thi với ID: {maDeThi}");
                }

                _logger.LogInformation("Lấy danh sách câu trả lời thành công với ID đề thi: {maDeThi}", maDeThi);
                return Ok(cauTraLoiList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách câu trả lời với ID đề thi: {maDeThi}", maDeThi);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }
     
        [HttpGet("{maDeThi}/WithChiTietAndCauTraLoi")]
        [SwaggerOperation("Lấy thông tin đề thi, chi tiết và câu trả lời")]
        public async Task<IActionResult> GetDeThiWithChiTietAndCauTraLoiAsync(string maDeThi)
        {
            try
            {
                if (!Guid.TryParse(maDeThi, out var guidId))
                {
                    _logger.LogWarning("ID đề thi không hợp lệ: {maDeThi}", maDeThi);
                    return BadRequest("ID đề thi không hợp lệ.");
                }

                var result = await _service.GetDeThiWithChiTietAndCauTraLoiAsync(guidId);
                if (result == null)
                {
                    _logger.LogWarning("Không tìm thấy thông tin cho đề thi với ID: {maDeThi}", maDeThi);
                    return NotFound($"Không tìm thấy thông tin cho đề thi với ID: {maDeThi}");
                }

                _logger.LogInformation("Lấy thông tin đề thi, chi tiết và câu trả lời thành công với ID: {maDeThi}", maDeThi);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin đề thi, chi tiết và câu trả lời với ID: {maDeThi}", maDeThi);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }
    [HttpPost("rut-trich")]
    [SwaggerOperation(Summary = "Rút trích đề thi từ yêu cầu")]
    public async Task<IActionResult> RutTrichDeThi([FromBody] Guid maYeuCau)
    {
        try
        {
            var result = await _service.RutTrichDeThiFromYeuCauAsync(maYeuCau);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Lỗi khi rút trích đề thi.");
            return StatusCode(500, new { message = "Lỗi hệ thống khi rút trích đề thi", chi_tiet = ex.Message });
        }
    }
    // ENDPOINT MỚI 1: Check trước rút trích (POST)
    [HttpPost("CheckExtraction")]
    [SwaggerOperation("Kiểm tra trước khi rút trích đề thi (simulate stats)")]
    public async Task<IActionResult> CheckExtractionAsync([FromBody] ExtractionCheckDto dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.MaTran))
            {
                _logger.LogWarning("Yêu cầu check không hợp lệ.");
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            var result = await _service.CheckExtractionAsync(dto.MaMonHoc, dto.MaTran);
            _logger.LogInformation("Check extraction thành công cho MaMonHoc: {MaMonHoc}", dto.MaMonHoc);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi check extraction.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    // ENDPOINT MỚI 2: Load stats/matrix cho môn học (GET)
    [HttpGet("MonHocStats/{maMonHoc}")]
    [SwaggerOperation("Lấy stats và matrix CLO cho môn học")]
    public async Task<IActionResult> GetMonHocStatsAsync(Guid maMonHoc)
    {
        try
        {
            var stats = await _service.GetMonHocStatsAsync(maMonHoc);
            _logger.LogInformation("Load stats thành công cho MaMonHoc: {MaMonHoc}", maMonHoc);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy stats môn học.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    }
}
