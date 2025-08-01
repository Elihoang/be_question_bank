using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.CauHoi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BEQuestionBank.Core.Services;
using BEQuestionBank.Domain.Enums;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CauHoiController : ControllerBase
    {
        private readonly ICauHoiService _service;
        private readonly WordImportService _wordImportService;
        private readonly ILogger<CauHoiController> _logger;

        public CauHoiController(ICauHoiService service,WordImportService wordImportService, ILogger<CauHoiController> logger)
        {
            _service = service;
            _wordImportService = wordImportService;
            _logger = logger;
        }

        // GET: api/CauHoi
        [HttpGet]
        [SwaggerOperation("Lấy danh sách tất cả câu hỏi")]
        public async Task<IEnumerable<CauHoi>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }
        [HttpGet("WithAnswers")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả câu hỏi kèm câu trả lời")]
        public async Task<ActionResult<IEnumerable<CauHoiDto>>> GetAllWithAnswersAsync()
        {
            try
            {
                var cauHois = await _service.GetAllWithAnswersAsync();
                if (cauHois == null || !cauHois.Any())
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi nào kèm câu trả lời.");
                    return StatusCode(StatusCodes.Status404NotFound, "Không tìm thấy câu hỏi nào.");
                }

                _logger.LogInformation("Lấy danh sách câu hỏi kèm câu trả lời thành công.");
                return StatusCode(StatusCodes.Status200OK, cauHois);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách câu hỏi kèm câu trả lời.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }

        // GET: api/CauHoi/{id}/WithAnswers
        [HttpGet("{id}/WithAnswers")]
        [SwaggerOperation(Summary = "Lấy câu hỏi kèm câu trả lời theo ID")]
        public async Task<ActionResult<CauHoiDto>> GetByIdWithAnswersAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID câu hỏi không hợp lệ: {Id}", id);
                    return StatusCode(StatusCodes.Status400BadRequest, "ID câu hỏi không hợp lệ.");
                }

                var cauHoi = await _service.GetByIdWithAnswersAsync(guidId);
                if (cauHoi == null)
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi với ID: {Id}", id);
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy câu hỏi với ID: {id}");
                }

                _logger.LogInformation("Lấy câu hỏi kèm câu trả lời thành công với ID: {Id}", id);
                return StatusCode(StatusCodes.Status200OK, cauHoi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy câu hỏi kèm câu trả lời với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }
        // GET: api/CauHoi/{id}
        [HttpGet("{id}")]
        [SwaggerOperation("Lấy câu hỏi theo ID")]
        public async Task<ActionResult<CauHoiDto>> GetByIdAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID câu hỏi không hợp lệ: {Id}", id);
                    return StatusCode(StatusCodes.Status400BadRequest, "ID câu hỏi không hợp lệ.");
                }

                var cauHoi = await _service.GetByIdAsync(guidId);
                if (cauHoi == null)
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi với ID: {Id}", id);
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy câu hỏi với ID: {id}");
                }

                return StatusCode(StatusCodes.Status200OK, cauHoi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy câu hỏi theo ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }

        // POST: api/CauHoi
        [HttpPost]
        [SwaggerOperation("Thêm mới câu hỏi")]
        public async Task<ActionResult<CauHoiDto>> CreateAsync([FromBody] CreateCauHoiDto cauHoiDto)
        {
            if (cauHoiDto == null)
            {
                _logger.LogError("Yêu cầu thêm câu hỏi không hợp lệ.");
                return StatusCode(StatusCodes.Status400BadRequest, "Yêu cầu không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Dữ liệu đầu vào không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            try
            {
                var cauHoi = new CauHoi
                {
                    MaPhan = cauHoiDto.MaPhan,
                    MaSoCauHoi = cauHoiDto.MaSoCauHoi,
                    NoiDung = cauHoiDto.NoiDung,
                    HoanVi = cauHoiDto.HoanVi,
                    CapDo = cauHoiDto.CapDo,
                    SoCauHoiCon = cauHoiDto.SoCauHoiCon,
                    DoPhanCach = cauHoiDto.DoPhanCach,
                    MaCauHoiCha = cauHoiDto.MaCauHoiCha,
                    XoaTam = cauHoiDto.XoaTam ?? false,
                    SoLanDuocThi = cauHoiDto.SoLanDuocThi,
                    SoLanDung = cauHoiDto.SoLanDung,
                    CLO = cauHoiDto.CLO
                };

                await _service.AddAsync(cauHoi);
                _logger.LogInformation("Thêm mới câu hỏi thành công với MaSoCauHoi: {MaSoCauHoi}", cauHoiDto.MaSoCauHoi);
                return StatusCode(StatusCodes.Status201Created,
                    new { Message = "Thêm mới câu hỏi thành công", Data = cauHoi });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm mới câu hỏi.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi thêm mới câu hỏi.");
            }
        }

        // PUT: api/CauHoi/{id}
        [HttpPatch("{id}")]
        [SwaggerOperation("Cập nhật câu hỏi theo ID")]
        public async Task<ActionResult<CauHoiDto>> UpdateAsync(string id, [FromBody] UpdateCauHoiDto cauHoiDto)
        {
            if (cauHoiDto == null)
            {
                _logger.LogError("Yêu cầu cập nhật câu hỏi không hợp lệ.");
                return StatusCode(StatusCodes.Status400BadRequest, "Yêu cầu không hợp lệ.");
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
                    _logger.LogWarning("ID câu hỏi không hợp lệ: {Id}", id);
                    return BadRequest("ID câu hỏi không hợp lệ.");
                }

                var existingCauHoi = await _service.GetByIdAsync(guidId);
                if (existingCauHoi == null)
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi với ID: {Id}", id);
                    return NotFound($"Không tìm thấy câu hỏi với ID: {id}");
                }

                existingCauHoi.MaPhan = cauHoiDto.MaPhan;
                existingCauHoi.MaSoCauHoi = cauHoiDto.MaSoCauHoi;
                existingCauHoi.NoiDung = cauHoiDto.NoiDung;
                existingCauHoi.HoanVi = cauHoiDto.HoanVi;
                existingCauHoi.CapDo = cauHoiDto.CapDo;
                existingCauHoi.SoCauHoiCon = cauHoiDto.SoCauHoiCon;
                existingCauHoi.DoPhanCach = cauHoiDto.DoPhanCach;
                existingCauHoi.MaCauHoiCha = cauHoiDto.MaCauHoiCha;
                existingCauHoi.XoaTam = cauHoiDto.XoaTam ?? existingCauHoi.XoaTam;
                existingCauHoi.SoLanDuocThi = cauHoiDto.SoLanDuocThi;
                existingCauHoi.SoLanDung = cauHoiDto.SoLanDung;
                existingCauHoi.CLO = cauHoiDto.CLO;
                existingCauHoi.NgaySua = DateTime.UtcNow;

                 await _service.UpdateAsync(existingCauHoi);
                _logger.LogInformation("Cập nhật câu hỏi thành công với ID: {Id}, MaSoCauHoi: {MaSoCauHoi}", id, cauHoiDto.MaSoCauHoi);
                return StatusCode(StatusCodes.Status200OK,
                    new { Message = "Cập nhật câu hỏi thành công", Data = existingCauHoi  });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật câu hỏi với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi cập nhật câu hỏi.");
            }
        }

        // PATCH: api/CauHoi/{id}/XoaTam
        [HttpPatch("{id}/XoaTam")]
        [SwaggerOperation(Summary = "Xóa câu hỏi tạm thời")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID câu hỏi không hợp lệ: {Id}", id);
                    return StatusCode(StatusCodes.Status400BadRequest, "ID câu hỏi không hợp lệ.");
                }

                var existingCauHoi = await _service.GetByIdAsync(guidId);
                if (existingCauHoi == null)
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi với ID: {Id}", id);
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy câu hỏi với mã {id}");
                }

                existingCauHoi.XoaTam = true;
                await _service.UpdateAsync(existingCauHoi);
                _logger.LogInformation("Xóa tạm câu hỏi thành công với ID: {Id}", id);
                return StatusCode(StatusCodes.Status200OK, $"Đã xóa tạm câu hỏi với mã số {existingCauHoi.MaCauHoi} thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa tạm câu hỏi với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi xóa tạm câu hỏi.");
            }
        }

        // PATCH: api/CauHoi/{id}/KhoiPhuc
        [HttpPatch("{id}/KhoiPhuc")]
        [SwaggerOperation(Summary = "Khôi phục câu hỏi đã xóa tạm")]
        public async Task<IActionResult> Restore(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID câu hỏi không hợp lệ: {Id}", id);
                    return StatusCode(StatusCodes.Status400BadRequest, "ID câu hỏi không hợp lệ.");
                }

                var existingCauHoi = await _service.GetByIdAsync(guidId);
                if (existingCauHoi == null)
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi với ID: {Id}", id);
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy câu hỏi với mã {id}");
                }

                existingCauHoi.XoaTam = false;
                await _service.UpdateAsync(existingCauHoi);
                _logger.LogInformation("Khôi phục câu hỏi thành công với ID: {Id}", id);
                return StatusCode(StatusCodes.Status200OK, $"Đã khôi phục câu hỏi với mã số {existingCauHoi.MaCauHoi} thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi khôi phục câu hỏi với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi khôi phục câu hỏi.");
            }
        }

        // DELETE: api/CauHoi/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation("Xóa câu hỏi theo ID")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID câu hỏi không hợp lệ: {Id}", id);
                    return StatusCode(StatusCodes.Status400BadRequest, "ID câu hỏi không hợp lệ.");
                }

                var cauHoi = await _service.GetByIdAsync(guidId);
                if (cauHoi == null)
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi với ID: {Id}", id);
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy câu hỏi với ID: {id}");
                }

                await _service.DeleteAsync(cauHoi);
                _logger.LogInformation("Xóa câu hỏi thành công với ID: {Id}", id);
                return StatusCode(StatusCodes.Status200OK, new { Message = "Xóa câu hỏi thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa câu hỏi với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi xóa câu hỏi.");
            }
        }
        
        // GET: api/CauHoi/CLO/{maCLo}
        [HttpGet("CLO/{maCLo}")]
        [SwaggerOperation("Lấy danh sách câu hỏi theo mã CLO")]
        public async Task<ActionResult<IEnumerable<CauHoiDto>>> GetByCLoAsync(EnumCLO maCLo)
        {
            try
            {
                if (!Enum.IsDefined(typeof(EnumCLO), maCLo))
                {
                    _logger.LogWarning("Mã CLO không hợp lệ: {maCLo}", maCLo);
                    return StatusCode(StatusCodes.Status400BadRequest,"Mã CLO không hợp lệ.");
                }
                
                var cauHois = await _service.GetByCLoAsync(maCLo);

                if (cauHois == null || !cauHois.Any())
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi nào với mã CLO: {maCLo}", maCLo);
                    return StatusCode(StatusCodes.Status404NotFound,$"Không tìm thấy câu hỏi nào với mã CLO: {maCLo}");
                }

                _logger.LogInformation("Lấy danh sách câu hỏi thành công với mã CLO: {maCLo}", maCLo);
                return StatusCode(StatusCodes.Status200OK, cauHois);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách câu hỏi theo mã CLO: {maCLo}", maCLo);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }
        // GET: api/CauHoi/Phan/{maPhan}
        [HttpGet("Phan/{maPhan}")]
        [SwaggerOperation("Lấy danh sách câu hỏi theo mã phần")]
        public async Task<ActionResult<IEnumerable<CauHoiDto>>> GetByMaPhanAsync(Guid maPhan)
        {
            try
            {
                var cauHois = await _service.GetByMaPhanAsync(maPhan);

                if (cauHois == null || !cauHois.Any())
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi nào với mã phần: {maPhan}", maPhan);
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy câu hỏi nào với mã phần: {maPhan}");
                }

                _logger.LogInformation("Lấy danh sách câu hỏi thành công với mã phần: {maPhan}", maPhan);
                return StatusCode(StatusCodes.Status200OK, cauHois);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách câu hỏi theo mã phần: {maPhan}", maPhan);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }
        // GET: api/CauHoi/MonHoc/{maMonHoc}
        [HttpGet("MonHoc/{maMonHoc}")]
        [SwaggerOperation("Lấy danh sách câu hỏi theo mã môn học")]
        public async Task<ActionResult<IEnumerable<CauHoiDto>>> GetByMaMonHocAsync(Guid maMonHoc)
        {
            try
            {
                var cauHois = await _service.GetByMaMonHocAsync(maMonHoc);

                if (cauHois == null || !cauHois.Any())
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi nào với mã môn học: {maMonHoc}", maMonHoc);
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy câu hỏi nào với mã môn học: {maMonHoc}");
                }

                _logger.LogInformation("Lấy danh sách câu hỏi thành công với mã môn học: {maMonHoc}", maMonHoc);
                return StatusCode(StatusCodes.Status200OK, cauHois);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách câu hỏi theo mã môn học: {maMonHoc}", maMonHoc);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }
        // GET: api/CauHoi/DeThi/{maDeThi}
        [HttpGet("DeThi/{maDeThi}")]
        [SwaggerOperation("Lấy danh sách câu hỏi theo mã đề thi")]
        public async Task<ActionResult<IEnumerable<CauHoiDto>>> GetByMaDeThiAsync(Guid maDeThi)
        {
            try
            {
                var cauHois = await _service.GetByMaDeThiAsync(maDeThi);

                if (cauHois == null || !cauHois.Any())
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi nào với mã đề thi: {maDeThi}", maDeThi);
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy câu hỏi nào với mã đề thi: {maDeThi}");
                }

                _logger.LogInformation("Lấy danh sách câu hỏi thành công với mã đề thi: {maDeThi}", maDeThi);
                return StatusCode(StatusCodes.Status200OK, cauHois);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách câu hỏi theo mã đề thi: {maDeThi}", maDeThi);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }
        // GET: api/CauHoi/CauHoiCha/{maCHCha}
        [HttpGet("CauHoiCha/{maCHCha}")]
        [SwaggerOperation("Lấy danh sách câu hỏi con theo mã câu hỏi cha")]
        public async Task<ActionResult<IEnumerable<CauHoiDto>>> GetByMaCauHoiChaAsync(Guid maCHCha)
        {
            try
            {
                var cauHois = await _service.GetByMaCauHoiChasync(maCHCha);

                if (cauHois == null || !cauHois.Any())
                {
                    _logger.LogWarning("Không tìm thấy câu hỏi nào với mã câu hỏi cha: {maCHCha}", maCHCha);
                    return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy câu hỏi nào với mã câu hỏi cha: {maCHCha}");
                }

                _logger.LogInformation("Lấy danh sách câu hỏi thành công với mã câu hỏi cha: {maCHCha}", maCHCha);
                return StatusCode(StatusCodes.Status200OK, cauHois);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách câu hỏi theo mã câu hỏi cha: {maCHCha}", maCHCha);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }
        
        [HttpGet("Groups")]
        [SwaggerOperation(Summary = "Lấy tất cả các nhóm câu hỏi (câu hỏi cha và câu hỏi con)")]
        public async Task<ActionResult<IEnumerable<CauHoiDto>>> GetAllGroupsAsync()
        {
            try
            {
                var cauHoiGroups = await _service.GetAllGroupsAsync();
                _logger.LogInformation("Lấy danh sách tất cả nhóm câu hỏi thành công. Số lượng: {Count}", cauHoiGroups?.Count() ?? 0);
                return StatusCode(StatusCodes.Status200OK, cauHoiGroups ?? new List<CauHoiDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách tất cả nhóm câu hỏi.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi truy xuất dữ liệu.");
            }
        }
        [HttpPost("WithAnswers")]
        [SwaggerOperation(Summary = "Thêm mới câu hỏi kèm câu trả lời")]
        public async Task<ActionResult<CauHoiDto>> CreateWithAnswersAsync([FromBody] CreateCauHoiWithAnswersDto cauHoiDto)
        {
            if (cauHoiDto == null)
            {
                _logger.LogError("Yêu cầu thêm câu hỏi kèm câu trả lời không hợp lệ.");
                return StatusCode(StatusCodes.Status400BadRequest, "Yêu cầu không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Dữ liệu đầu vào không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            try
            {
                var result = await _service.AddWithAnswersAsync(cauHoiDto);
                _logger.LogInformation("Thêm mới câu hỏi kèm câu trả lời thành công với MaSoCauHoi: {MaSoCauHoi}", cauHoiDto.MaSoCauHoi);
                return StatusCode(StatusCodes.Status201Created,
                    new { Message = "Thêm mới câu hỏi kèm câu trả lời thành công", Data = result });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Dữ liệu câu hỏi không hợp lệ.");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Dữ liệu đầu vào không hợp lệ.");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm mới câu hỏi kèm câu trả lời.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi thêm mới câu hỏi và câu trả lời.");
            }
        }
        
        [HttpPatch("{id}/WithAnswers")]
        [SwaggerOperation(Summary = "Cập nhật câu hỏi kèm câu trả lời theo ID")]
        public async Task<ActionResult<CauHoiDto>> UpdateWithAnswersAsync(string id, [FromBody] UpdateCauHoiWithAnswersDto cauHoiDto)
        {
            if (cauHoiDto == null)
            {
                _logger.LogError("Yêu cầu cập nhật câu hỏi kèm câu trả lời không hợp lệ.");
                return StatusCode(StatusCodes.Status400BadRequest, "Yêu cầu không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Dữ liệu đầu vào không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    _logger.LogWarning("ID câu hỏi không hợp lệ: {Id}", id);
                    return StatusCode(StatusCodes.Status400BadRequest, "ID câu hỏi không hợp lệ.");
                }

                var result = await _service.UpdateWithAnswersAsync(guidId, cauHoiDto);
                _logger.LogInformation("Cập nhật câu hỏi kèm câu trả lời thành công với MaSoCauHoi: {MaSoCauHoi}", cauHoiDto.MaSoCauHoi);
                return StatusCode(StatusCodes.Status200OK,
                    new { Message = "Cập nhật câu hỏi kèm câu trả lời thành công", Data = result });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Dữ liệu câu hỏi không hợp lệ.");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Dữ liệu đầu vào không hợp lệ.");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật câu hỏi kèm câu trả lời với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi cập nhật câu hỏi và câu trả lời.");
            }
        }
        [HttpPost("ImportWord")]
        [SwaggerOperation(Summary = "Nhập câu hỏi từ tệp Word")]
        public async Task<ActionResult<ImportResult>> ImportWordAsync(IFormFile wordFile, [FromQuery] Guid maPhan, [FromQuery] string? mediaFolderPath = null)
        {
            if (wordFile == null || wordFile.Length == 0)
            {
                _logger.LogWarning("Tệp Word không hợp lệ.");
                return StatusCode(StatusCodes.Status400BadRequest, new ImportResult { Errors = new List<string> { "Tệp Word là bắt buộc và không được rỗng." } });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Dữ liệu đầu vào không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            try
            {
                var result = await _wordImportService.ImportQuestionsAsync(wordFile, maPhan, mediaFolderPath);
                _logger.LogInformation("Nhập {SuccessCount} câu hỏi thành công từ tệp Word.", result.SuccessCount);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi nhập câu hỏi từ tệp Word.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ImportResult { Errors = new List<string> { "Đã xảy ra lỗi khi nhập câu hỏi." } });
            }
        }
    }
}