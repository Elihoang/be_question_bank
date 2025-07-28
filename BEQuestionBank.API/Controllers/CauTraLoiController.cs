using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.CauTraLoi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CauTraLoiController : ControllerBase
    {
        private readonly ICauTraLoiService _service;
        private readonly ICauHoiService _cauHoiService;
        private readonly ILogger<CauTraLoiController> _logger;

        public CauTraLoiController(ICauTraLoiService service, ICauHoiService cauHoiService,ILogger<CauTraLoiController> logger)
        {
            _service = service;
            _cauHoiService = cauHoiService;
            _logger = logger;
        }


        [HttpGet("{id}")]
        [SwaggerOperation("Tìm Câu Trả Lời theo mã")]
        public async Task<ActionResult<CauTraLoi>> GetByIdAsync(string id)
        {
            try
            {
                var cauTraLoi = await _service.GetByIdAsync(Guid.Parse(id));
                if (cauTraLoi == null)
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy môn học với mã {id}");

                return StatusCode(StatusCodes.Status200OK, cauTraLoi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm môn học");
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi máy chủ");
            }
        }

        [HttpGet]
        [SwaggerOperation("Lấy danh sách tất cả Câu Trả Lời")]
        public async Task<IEnumerable<CauTraLoi>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("cau-hoi/{maCauHoi}")]
        [SwaggerOperation("Lấy danh sách Câu Trả Lời theo Mã Câu Hỏi")]
        public async Task<IEnumerable<CauTraLoi>> GetByMaKhoaAsync(string maCauHoi)
        {
            var list = await _service.GetByMaCauHoi(Guid.Parse(maCauHoi));
            return list;
        }

        [HttpPost]
        [SwaggerOperation("Thêm mới Câu Trả Lời")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCauTraLoiDto model)
        {
            try
            {
                var existingCauHoi = await _cauHoiService.ExistsAsync( cH => cH.MaCauHoi == model.MaCauHoi);
                if(!existingCauHoi)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Không tìm thấy Mã Câu Hỏi");
                }

                var cauTraLoi = new CauTraLoi
                {
                    MaCauHoi = model.MaCauHoi,
                    NoiDung = model.NoiDung,
                    ThuTu = model.ThuTu,
                    LaDapAn = model.LaDapAn,
                    HoanVi = model.HoanVi
                };

                await _service.AddAsync(cauTraLoi);
                return StatusCode(StatusCodes.Status201Created, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm Câu Hỏi");
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi máy chủ");
            }
        }

        [HttpPatch("{id}")]
        [SwaggerOperation("Cập nhật thông tin Câu Trả Lời")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateCauTraLoiDto model)
        {
            var cautraLoi = await _service.GetByIdAsync(Guid.Parse(id));
            if (cautraLoi == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy Câu Trả Lời với mã {id}");

            cautraLoi.NoiDung = model.NoiDung;
            cautraLoi.ThuTu = model.ThuTu;
            cautraLoi.LaDapAn = model.LaDapAn;
            cautraLoi.HoanVi = model.HoanVi;

            await _service.UpdateAsync(cautraLoi);

            return StatusCode(StatusCodes.Status200OK, cautraLoi);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Xóa Câu Trả Lời")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var monHoc = await _service.GetByIdAsync(Guid.Parse(id));
            if (monHoc == null)
                return NotFound($"Không tìm thấy Câu Trả Lời với mã {id}");

            await _service.DeleteAsync(monHoc);
            return StatusCode(StatusCodes.Status200OK, $"Đã xóa Câu Trả Lời có mã {id}");
        }
        
    }
}



