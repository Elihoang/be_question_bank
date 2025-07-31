using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.File;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using File = BEQuestionBank.Domain.Models.File;
using IOFile = System.IO.File;
using System.ComponentModel.DataAnnotations;
using BEQuestionBank.Domain.Enums;

namespace BEQuestionBank.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;

        public FileController(IFileService fileService, ILogger<FileController> logger)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Tải lên một tệp mới liên quan đến câu hỏi hoặc câu trả lời.
        /// </summary>
        /// <param name="file">Tệp cần tải lên</param>
        /// <param name="maCauHoi">Mã câu hỏi (tùy chọn)</param>
        /// <param name="maCauTraLoi">Mã câu trả lời (tùy chọn)</param>
        /// <returns>DTO của tệp với thông tin chi tiết</returns>
        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadFile([Required] IFormFile file, [FromQuery] Guid? maCauHoi, [FromQuery] Guid? maCauTraLoi)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Yêu cầu tải lên tệp không hợp lệ: Tệp rỗng hoặc null");
                return BadRequest(new { Error = "Tệp là bắt buộc và không được rỗng" });
            }

            try
            {
                var fileDto = await _fileService.AddAsync(file, maCauHoi, maCauTraLoi);
                _logger.LogInformation("Tải lên thành công tệp {FileName} với ID {FileId}", fileDto.TenFile, fileDto.MaFile);
                return Ok(new { Data = fileDto, Message = "Tải lên tệp thành công" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Yêu cầu tải lên tệp không hợp lệ cho {FileName}", file.FileName);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải lên tệp {FileName}", file.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Đã xảy ra lỗi khi tải lên tệp" });
            }
        }

        /// <summary>
        /// Lấy thông tin tệp theo mã.
        /// </summary>
        /// <param name="maFile">Mã tệp</param>
        /// <returns>DTO của tệp với thông tin chi tiết</returns>
        [HttpGet("{maFile}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFile([Required] Guid maFile)
        {
            if (maFile == Guid.Empty)
            {
                _logger.LogWarning("Mã tệp không hợp lệ: {FileId}", maFile);
                return BadRequest(new { Error = "Mã tệp hợp lệ là bắt buộc" });
            }

            try
            {
                var file = await _fileService.GetByIdAsync(maFile);
                var fileDto = new FileDto
                {
                    MaFile = file.MaFile,
                    MaCauHoi = file.MaCauHoi,
                    MaCauTraLoi = file.MaCauTraLoi,
                    TenFile = file.TenFile,
                    LoaiFile = file.LoaiFile ?? 0
                };
                _logger.LogInformation("Lấy thành công thông tin tệp {FileId}", maFile);
                return Ok(new { Data = fileDto, Message = "Lấy thông tin tệp thành công" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy tệp: {FileId}", maFile);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin tệp {FileId}", maFile);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Đã xảy ra lỗi khi lấy thông tin tệp" });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả các tệp.
        /// </summary>
        /// <returns>Danh sách các DTO của tệp</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFiles()
        {
            try
            {
                var files = await _fileService.GetAllAsync();
                var fileDtos = files.Select(f => new FileDto
                {
                    MaFile = f.MaFile,
                    MaCauHoi = f.MaCauHoi,
                    MaCauTraLoi = f.MaCauTraLoi,
                    TenFile = f.TenFile,
                    LoaiFile = f.LoaiFile ?? 0
                }).ToList();
                _logger.LogInformation("Lấy thành công {FileCount} tệp", fileDtos.Count);
                return Ok(new { Data = fileDtos, Message = "Lấy danh sách tệp thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách tệp");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Đã xảy ra lỗi khi lấy danh sách tệp" });
            }
        }

        /// <summary>
        /// Cập nhật một tệp hiện có.
        /// </summary>
        /// <param name="maFile">Mã tệp</param>
        /// <param name="newFile">Tệp mới để thay thế tệp hiện tại</param>
        /// <returns>DTO của tệp đã cập nhật</returns>
        [HttpPut("{maFile}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateFile([Required] Guid maFile, [Required] IFormFile newFile)
        {
            if (maFile == Guid.Empty)
            {
                _logger.LogWarning("Mã tệp không hợp lệ: {FileId}", maFile);
                return BadRequest(new { Error = "Mã tệp hợp lệ là bắt buộc" });
            }

            if (newFile == null || newFile.Length == 0)
            {
                _logger.LogWarning("Tệp cập nhật không hợp lệ: Tệp rỗng hoặc null");
                return BadRequest(new { Error = "Tệp cập nhật là bắt buộc và không được rỗng" });
            }

            try
            {
                var existingFile = await _fileService.GetByIdAsync(maFile);
                var extension = Path.GetExtension(newFile.FileName).ToLower();
                var loaiFile = extension switch
                {
                    ".jpg" or ".png" or ".jpeg" => (int)FileType.Image,
                    ".mp3" => (int)FileType.Audio,
                    _ => throw new ArgumentException($"Định dạng tệp {extension} không được hỗ trợ")
                };

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(existingFile.TenFile.Substring(0, existingFile.TenFile.LastIndexOf(Path.DirectorySeparatorChar)), fileName);

                // Xóa tệp vật lý cũ
                if (IOFile.Exists(existingFile.TenFile))
                {
                    IOFile.Delete(existingFile.TenFile);
                    _logger.LogInformation("Xóa thành công tệp vật lý cũ {FileName}", existingFile.TenFile);
                }

                // Lưu tệp mới
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newFile.CopyToAsync(stream);
                }

                // Cập nhật thông tin tệp
                existingFile.TenFile = filePath;
                existingFile.LoaiFile = (FileType?)loaiFile;
                await _fileService.UpdateAsync(existingFile);

                var fileDto = new FileDto
                {
                    MaFile = existingFile.MaFile,
                    MaCauHoi = existingFile.MaCauHoi,
                    MaCauTraLoi = existingFile.MaCauTraLoi,
                    TenFile = existingFile.TenFile,
                    LoaiFile = existingFile.LoaiFile ?? 0
                };

                _logger.LogInformation("Cập nhật thành công tệp {FileId}", maFile);
                return Ok(new { Data = fileDto, Message = "Cập nhật tệp thành công" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Lỗi khi cập nhật tệp: {FileId}", maFile);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật tệp {FileId}", maFile);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Đã xảy ra lỗi khi cập nhật tệp" });
            }
        }

        /// <summary>
        /// Xóa một tệp theo mã.
        /// </summary>
        /// <param name="maFile">Mã tệp</param>
        /// <returns>Thông báo xóa thành công</returns>
        [HttpDelete("{maFile}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFile([Required] Guid maFile)
        {
            if (maFile == Guid.Empty)
            {
                _logger.LogWarning("Mã tệp không hợp lệ: {FileId}", maFile);
                return BadRequest(new { Error = "Mã tệp hợp lệ là bắt buộc" });
            }

            try
            {
                var file = await _fileService.GetByIdAsync(maFile);
                await _fileService.DeleteAsync(file);
                _logger.LogInformation("Xóa thành công tệp {FileId}", maFile);
                return Ok(new { Message = "Xóa tệp thành công" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy tệp để xóa: {FileId}", maFile);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa tệp {FileId}", maFile);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Đã xảy ra lỗi khi xóa tệp" });
            }
        }

        /// <summary>
        /// Tìm kiếm tệp theo mã câu hỏi.
        /// </summary>
        /// <param name="maCauHoi">Mã câu hỏi (tùy chọn)</param>
        /// <returns>Danh sách các DTO của tệp</returns>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindFiles([FromQuery] Guid? maCauHoi)
        {
            try
            {
                Expression<Func<File, bool>> predicate = f => maCauHoi == null || f.MaCauHoi == maCauHoi;
                var files = await _fileService.FindAsync(predicate);
                var fileDtos = files.Select(f => new FileDto
                {
                    MaFile = f.MaFile,
                    MaCauHoi = f.MaCauHoi,
                    MaCauTraLoi = f.MaCauTraLoi,
                    TenFile = f.TenFile,
                    LoaiFile = f.LoaiFile ?? 0
                }).ToList();
                _logger.LogInformation("Tìm kiếm thành công tệp với mã câu hỏi: {MaCauHoi}", maCauHoi);
                return Ok(new { Data = fileDtos, Message = "Tìm kiếm tệp thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm tệp với mã câu hỏi: {MaCauHoi}", maCauHoi);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Đã xảy ra lỗi khi tìm kiếm tệp" });
            }
        }
    }
}