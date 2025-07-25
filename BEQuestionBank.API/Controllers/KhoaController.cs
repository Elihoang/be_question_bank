using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BEQuestionBank.Shared.DTOs.Khoa;
using BEQuestionBank.Shared.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhoaController : ControllerBase
    {
        private readonly IKhoaService _service;

        public KhoaController(IKhoaService service)
        {
            _service = service;
        }

        // POST: api/Khoa
        [HttpPost]
        [SwaggerOperation(Summary = "Thêm một Khoa")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(KhoaCreateDto))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(KhoaDto))]
        public async Task<IActionResult> Create([FromBody] KhoaCreateDto model)
        {
            var existingKhoa = await _service.GetByTenKhoaAsync(model.TenKhoa);
            if (existingKhoa != null)
            {
                if (existingKhoa.XoaTam)
                {
                    existingKhoa.XoaTam = false;
                    await _service.UpdateAsync(existingKhoa);
                    return StatusCode(StatusCodes.Status409Conflict, new KhoaDto
                    {
                        MaKhoa = existingKhoa.MaKhoa,
                        TenKhoa = existingKhoa.TenKhoa,
                        XoaTam = existingKhoa.XoaTam
                    });
                }

                throw new Exception($"Khoa với tên khoa {model.TenKhoa} đã tồn tại");
            }

            var newKhoa = new Khoa
            {
                TenKhoa = model.TenKhoa,
                XoaTam = false
            };
            await _service.AddAsync(newKhoa);

            return StatusCode(StatusCodes.Status201Created, model);
        }

        // GET: api/Khoa
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách Khoa (Admin)")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<KhoaDto>))]
        public async Task<IEnumerable<KhoaDto>> GetAll()
        {
            var khoas = await _service.GetAllAsync();
            var khoaDtos = khoas.Select(k => new KhoaDto
            {
                MaKhoa = k.MaKhoa,
                TenKhoa = k.TenKhoa,
                XoaTam = k.XoaTam
            });
            return khoaDtos;
        }

        // GET: api/Khoa/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy thông tin Khoa theo mã")]
        public async Task<IActionResult> GetById(string id)
        {
            var khoa = await _service.GetByIdAsync(id);
            if (khoa == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy Khoa với mã {id}");
            }

            var khoaDto = new KhoaDto
            {
                MaKhoa = khoa.MaKhoa,
                TenKhoa = khoa.TenKhoa,
                XoaTam = khoa.XoaTam
            };
            return StatusCode(StatusCodes.Status200OK, khoaDto);
        }

        // PATCH: api/Khoa/{id}
        [HttpPatch("{id}")]
        [SwaggerOperation(Summary = "Cập nhật thông tin Khoa")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KhoaDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> Update(string id, [FromBody] KhoaUpdateDto model)
        {
            var existingKhoa = await _service.GetByIdAsync(id);
            if (existingKhoa == null)
            {
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
            return StatusCode(StatusCodes.Status200OK, khoaDto);
        }

        // DELETE: api/Khoa/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa Khoa vĩnh viễn")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> Delete(string id)
        {
            var existingKhoa = await _service.GetByIdAsync(id);
            if (existingKhoa == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy Khoa với mã {id}");
            }

            await _service.DeleteAsync(existingKhoa);
            return StatusCode(StatusCodes.Status200OK, $"Đã xóa Khoa với mã {id} thành công");
        }

        // PATCH: api/Khoa/{id}/XoaTam
        [HttpPatch("{id}/XoaTam")]
        [SwaggerOperation(Summary = "Xóa Khoa tạm thời")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KhoaDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var existingKhoa = await _service.GetByIdAsync(id);
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
            var existingKhoa = await _service.GetByIdAsync(id);
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