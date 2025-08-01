using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs;
using BEQuestionBank.Shared.DTOs.Khoa;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhoaController(IKhoaService service, ILogger<KhoaController> logger) : ControllerBase
    {
        private readonly IKhoaService _service = service;
        private readonly ILogger<KhoaController> _logger = logger;

        // GET: api/Khoa/{id}
        [HttpGet("{id}")]
        [SwaggerOperation("Tìm Khoa theo Mã Khoa")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Khoa>> GetKhoaByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu tìm Khoa theo mã: {MaKhoa}", id);

                var khoa = await _service.GetByIdAsync(Guid.Parse(id));
                if (khoa == null)
                {
                    _logger.LogWarning("Không tìm thấy Khoa với mã: {MaKhoa}", id);
                    return NotFound($"Không tìm thấy Khoa với mã: {id}");
                }

                _logger.LogInformation("Tìm thấy Khoa: {TenKhoa}", khoa.TenKhoa);
                return Ok(khoa);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Lỗi khi lấy thông tin Khoa theo mã: {MaKhoa}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }

        //GET: api/Khoa
        [HttpGet]
        [SwaggerOperation("Lấy danh sách Khoa")]
        public async Task<IEnumerable<KhoaDto>> GetAllAsync()
        {
            var list = await _service.GetAllAsync(); // Khoa có Include DanhSachMonHoc

            var result = list.Select(k => new KhoaDto
            {
                MaKhoa = k.MaKhoa,
                TenKhoa = k.TenKhoa,
                XoaTam = k.XoaTam,
                DanhSachMonHoc = k.DanhSachMonHoc?.Select(m => new MonHocDto
                {
                    MaMonHoc = m.MaMonHoc,
                    TenMonHoc = m.TenMonHoc,
                    SoTinChi = m.SoTinChi
                }).ToList() ?? new()
            });

            return result;
        }


        //POST: api/Khoa
        [HttpPost]
        [SwaggerOperation("Thêm một Khoa")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateKhoaAsync([FromBody] KhoaCreateDto model)
        {
            try
            {
                _logger.LogInformation("Bắt đầu thêm Khoa. Tên: {TenKhoa}", model.TenKhoa);

                var existingKhoa = await _service.GetByTenKhoaAsync(model.TenKhoa);
                if (existingKhoa != null)
                {
                    if (existingKhoa.XoaTam == true)
                    {
                        _logger.LogInformation("Khoa tạm xóa được phát hiện: {TenKhoa}", existingKhoa.TenKhoa);
                        return StatusCode(StatusCodes.Status409Conflict, new KhoaDto
                        {
                            MaKhoa = existingKhoa.MaKhoa,
                            TenKhoa = existingKhoa.TenKhoa,
                            XoaTam = existingKhoa.XoaTam
                        });
                    }

                    _logger.LogWarning("Khoa với tên {TenKhoa} đã tồn tại.", model.TenKhoa);
                    return Conflict($"Khoa với tên {model.TenKhoa} đã tồn tại");
                }

                var newKhoa = new Khoa
                {
                    TenKhoa = model.TenKhoa,
                    XoaTam = model.XoaTam ?? false
                };

                await _service.AddAsync(newKhoa);
                _logger.LogInformation("Thêm Khoa thành công: {TenKhoa}", model.TenKhoa);

                return StatusCode(StatusCodes.Status201Created, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi thêm Khoa: {TenKhoa}", model.TenKhoa);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi trong quá trình xử lý.");
            }
        }

        //PATCH: api/Khoa/{id}
        [HttpPatch("{id}")]
        [SwaggerOperation(Summary = "Cập nhật thông tin Khoa")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KhoaDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateKhoaAsync(string id, [FromBody] KhoaUpdateDto model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu cập nhật Khoa có mã: {MaKhoa}", id);

                var existingKhoa = await _service.GetByIdAsync(Guid.Parse(id));
                if (existingKhoa == null)
                {
                    _logger.LogWarning("Không tìm thấy Khoa với mã: {MaKhoa}", id);
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy Khoa với mã {id}");
                }

                existingKhoa.TenKhoa = model.TenKhoa;
                existingKhoa.XoaTam = model.XoaTam ?? existingKhoa.XoaTam;

                await _service.UpdateAsync(existingKhoa);

                var khoaDto = new KhoaDto
                {
                    MaKhoa = existingKhoa.MaKhoa,
                    TenKhoa = existingKhoa.TenKhoa,
                    XoaTam = existingKhoa.XoaTam
                };

                _logger.LogInformation("Cập nhật thành công Khoa: {TenKhoa}", existingKhoa.TenKhoa);

                return StatusCode(StatusCodes.Status200OK, khoaDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật Khoa có mã: {MaKhoa}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi cập nhật Khoa.");
            }
        }

        // DELETE: api/Khoa/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa Khoa (mềm hoặc cứng)")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteKhoaAsync(string id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu xóa Khoa có mã: {MaKhoa}", id);

                var existingKhoa = await _service.GetByIdAsync(Guid.Parse(id));
                if (existingKhoa == null)
                {
                    _logger.LogWarning("Không tìm thấy Khoa với mã: {MaKhoa}", id);
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy Khoa với mã {id}");
                }

                await _service.DeleteAsync(existingKhoa);
                _logger.LogInformation("Đã xóa Khoa có mã: {MaKhoa}", id);

                return StatusCode(StatusCodes.Status200OK, $"Đã xóa Khoa với mã {id} thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa Khoa với mã: {MaKhoa}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi xóa Khoa.");
            }
        }

        // PATCH: api/Khoa/{id}/XoaTam
        [HttpPatch("{id}/XoaTam")]
        [SwaggerOperation(Summary = "Xóa Khoa tạm thời")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KhoaDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var existingKhoa = await _service.GetByIdAsync(Guid.Parse(id));
            if (existingKhoa == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy Khoa với mã {id}");
            }
            existingKhoa.XoaTam = true;
            await _service.UpdateAsync(existingKhoa);
            return StatusCode(StatusCodes.Status200OK, $"Đã xóa tạm {existingKhoa.TenKhoa} thành công");
        }

        // PATCH: api/Khoa/{id}/KhoiPhuc
        [HttpPatch("{id}/KhoiPhuc")]
        [SwaggerOperation(Summary = "Khôi phục Khoa đã xóa tạm")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KhoaDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> Restore(string id)
        {
            var existingKhoa = await _service.GetByIdAsync(Guid.Parse(id));
            if (existingKhoa == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy Khoa với mã {id}");
            }
            existingKhoa.XoaTam = false;
            await _service.UpdateAsync(existingKhoa);
            return StatusCode(StatusCodes.Status200OK, $"Đã khôi phục {existingKhoa.TenKhoa} thành công");
        }
    }
}
