using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhanController : ControllerBase
    {
        private readonly IPhanService _service;
        private readonly ILogger<PhanController> _logger;

        public PhanController(IPhanService service, ILogger<PhanController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/Phan
        [HttpGet]
        [SwaggerOperation("Lấy danh sách tất cả phân")]
        public async Task<IEnumerable<Phan>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        // GET: api/Phan/{id}
        [HttpGet("{id}")]
        [SwaggerOperation("Lấy phân theo ID")]
        public async Task<ActionResult<Phan>> GetByIdAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID phân không hợp lệ: {Id}", id);
                    return BadRequest("ID phân không hợp lệ.");
                }

                var phan = await _service.GetByIdAsync(guidId);
                if (phan == null)
                {
                    _logger.LogWarning("Không tìm thấy phân với ID: {Id}", id);
                    return NotFound($"Không tìm thấy phân với ID: {id}");
                }

                return StatusCode(StatusCodes.Status200OK, phan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy phân theo ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }

        // POST: api/Phan
        [HttpPost]
        [SwaggerOperation("Thêm mới phân")]
        public async Task<ActionResult<Phan>> CreateAsync([FromBody] PhanCreateDto phanDto)
        {
            if (phanDto == null)
            {
                _logger.LogError("Yêu cầu thêm phân không hợp lệ.");
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Dữ liệu đầu vào không hợp lệ: {Errors}",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            try
            {
                var phan = new Phan
                {
                    MaMonHoc = phanDto.MaMonHoc,
                    TenPhan = phanDto.TenPhan,
                    NoiDung = phanDto.NoiDung,
                    ThuTu = phanDto.ThuTu,
                    SoLuongCauHoi = phanDto.SoLuongCauHoi,
                    MaPhanCha = phanDto.MaPhanCha,
                    LaCauHoiNhom = phanDto.LaCauHoiNhom
                };

                await _service.AddAsync(phan);
                _logger.LogInformation("Thêm mới phân thành công với MaMonHoc: {MaMonHoc}, TenPhan: {TenPhan}",
                    phanDto.MaMonHoc, phanDto.TenPhan);
                return StatusCode(StatusCodes.Status201Created,
                    new { Message = "Thêm mới phân thành công", Data = phan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm mới phân.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi thêm mới phân.");
            }
        }

        // PUT: api/Phan/{id}
        [HttpPut("{id}")]
        [SwaggerOperation("Cập nhật phân theo ID")]
        public async Task<ActionResult<Phan>> UpdateAsync(string id, [FromBody] PhanUpdateDto phanDto)
        {
            if (phanDto == null)
            {
                _logger.LogError("Yêu cầu cập nhật phân không hợp lệ.");
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Dữ liệu đầu vào không hợp lệ: {Errors}",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID phân không hợp lệ: {Id}", id);
                    return BadRequest("ID phân không hợp lệ.");
                }

                var existingPhan = await _service.GetByIdAsync(guidId);
                if (existingPhan == null)
                {
                    _logger.LogWarning("Không tìm thấy phân với ID: {Id}", id);
                    return NotFound($"Không tìm thấy phân với ID: {id}");
                }

                existingPhan.MaMonHoc = phanDto.MaMonHoc;
                existingPhan.TenPhan = phanDto.TenPhan;
                existingPhan.NoiDung = phanDto.NoiDung;
                existingPhan.ThuTu = phanDto.ThuTu;
                existingPhan.SoLuongCauHoi = phanDto.SoLuongCauHoi;
                existingPhan.MaPhanCha = phanDto.MaPhanCha;
                existingPhan.LaCauHoiNhom = phanDto.LaCauHoiNhom;

                await _service.UpdateAsync(existingPhan);
                _logger.LogInformation("Cập nhật phân thành công với ID: {Id}", id);
                return StatusCode(StatusCodes.Status200OK,
                    new { Message = "Cập nhật phân thành công", Data = existingPhan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật phân với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi cập nhật phân.");
            }
        }
    }
}