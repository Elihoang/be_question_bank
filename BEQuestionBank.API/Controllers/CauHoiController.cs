using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using BEQuestionBank.Application.Services;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.CauHoi;
using BEQuestionBank.Shared.DTOs.CauTraLoi;

namespace BEQuestionBank.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CauHoiController : ControllerBase
    {
        private readonly ICauHoiService _cauHoiService;
        private readonly ILogger<CauHoiController> _logger;

        public CauHoiController(ICauHoiService cauHoiService, ILogger<CauHoiController> logger)
        {
            _cauHoiService = cauHoiService;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả câu hỏi")]
        public async Task<ActionResult> GetAllCauHois()
        {
            try
            {
                var cauHois = await _cauHoiService.GetAllAsync();
                _logger.LogInformation("Lấy danh sách câu hỏi thành công. Số lượng: {Count}", cauHois.Count());
                return Ok(new { Message = "Lấy danh sách câu hỏi thành công", Data = cauHois });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách câu hỏi: {Message}", ex.Message);
                return StatusCode(500, new { Message = $"Lỗi khi lấy danh sách câu hỏi: {ex.Message}", Data = (IEnumerable<CauHoiDto>)null });
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy thông tin câu hỏi theo ID")]
        public async Task<ActionResult> GetCauHoiById(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out _))
                {
                    _logger.LogError("ID câu hỏi không đúng định dạng GUID: {Id}", id);
                    return BadRequest(new { Message = "ID câu hỏi không đúng định dạng GUID.", Data = (CauHoiDto)null });
                }

                var cauHoi = await _cauHoiService.GetByIdAsync(Guid.Parse(id));
                if (cauHoi == null)
                {
                    _logger.LogError("Không tìm thấy câu hỏi với ID: {Id}", id);
                    return NotFound(new { Message = $"Không tìm thấy câu hỏi với ID '{id}'.", Data = (CauHoiDto)null });
                }

                _logger.LogInformation("Lấy thông tin câu hỏi thành công với ID: {Id}", id);
                return Ok(new { Message = $"Lấy thông tin câu hỏi với ID '{id}' thành công", Data = cauHoi });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Lỗi tham số khi lấy câu hỏi với ID {Id}: {Message}", id, ex.Message);
                return BadRequest(new { Message = ex.Message, Data = (CauHoiDto)null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy câu hỏi với ID {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new { Message = $"Lỗi khi lấy câu hỏi: {ex.Message}", Data = (CauHoiDto)null });
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo một câu hỏi mới")]
        public async Task<ActionResult> CreateCauHoi([FromBody] CreateCauHoiDto cauHoiDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Dữ liệu đầu vào không hợp lệ khi tạo câu hỏi.");
                    return BadRequest(new { Message = "Dữ liệu đầu vào không hợp lệ.", Data = (CauHoiDto)null });
                }
                
                var newCauHoi = new CauHoi
                {
                    MaPhan = cauHoiDto.MaPhan,
                    MaSoCauHoi = cauHoiDto.MaSoCauHoi,
                    NoiDung = cauHoiDto.NoiDung,
                    HoanVi = cauHoiDto.HoanVi,
                    CapDo = cauHoiDto.CapDo,
                    SoCauHoiCon = cauHoiDto.SoCauHoiCon,
                    DoPhanCach = cauHoiDto.DoPhanCach,
                    MaCauHoiCha = cauHoiDto.MaCauHoiCha,
                    XoaTam = cauHoiDto.XoaTam,
                    SoLanDuocThi = cauHoiDto.SoLanDuocThi,
                    SoLanDung = cauHoiDto.SoLanDung,
                    NgayTao = DateTime.UtcNow,
                    NgaySua = DateTime.UtcNow,
                    CLO = cauHoiDto.CLO
                };
                await _cauHoiService.AddAsync(newCauHoi);
                return StatusCode(StatusCodes.Status201Created, new { Message = "Tạo câu hỏi thành công", Data = MapToCauHoiDto(newCauHoi) });
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi tạo câu hỏi: {Message}", ex.Message);
                return StatusCode(500, new { Message = $"Lỗi khi tạo câu hỏi: {ex.Message}"});
            }
        }

       
       
        
        private static CauHoiDto MapToCauHoiDto(CauHoi cauHoi)
        {
            return new CauHoiDto
            {
                MaCauHoi = cauHoi.MaCauHoi,
                MaPhan = cauHoi.MaPhan,
                MaSoCauHoi = cauHoi.MaSoCauHoi,
                NoiDung = cauHoi.NoiDung,
                HoanVi = cauHoi.HoanVi,
                CapDo = cauHoi.CapDo,
                SoCauHoiCon = cauHoi.SoCauHoiCon,
                DoPhanCach = cauHoi.DoPhanCach,
                MaCauHoiCha = cauHoi.MaCauHoiCha,
                XoaTam = cauHoi.XoaTam,
                SoLanDuocThi = cauHoi.SoLanDuocThi,
                SoLanDung = cauHoi.SoLanDung,
                NgayTao = cauHoi.NgayTao,
                NgaySua = cauHoi.NgaySua,
                CLO = cauHoi.CLO,
                CauTraLois = cauHoi.CauTraLois?.Select(ctl => new CauTraLoiDto
                {
                    MaCauTraLoi = ctl.MaCauTraLoi,
                    MaCauHoi = ctl.MaCauHoi,
                    NoiDung = ctl.NoiDung,
                    ThuTu = ctl.ThuTu,
                    LaDapAn = ctl.LaDapAn,
                    HoanVi = ctl.HoanVi
                }).ToList() ?? new List<CauTraLoiDto>()
            };
        }
    }
}