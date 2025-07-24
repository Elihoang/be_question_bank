using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BEQuestionBank.Shared.DTOs.Khoa;
using BEQuestionBank.Shared.Helpers;

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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KhoaCreateDto model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    message = "Khoa model is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid model data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var khoa = new Khoa
                {
                    MaKhoa = CodeGenerator.GenerateKhoaCode(), // Will be generated if null
                    TenKhoa = model.TenKhoa ?? throw new ArgumentException("TenKhoa is required"),
                    // Map other properties as needed
                };

                await _service.AddAsync(khoa);
                return Ok(new
                {
                    message = "Khoa created successfully",
                    data = model
                });
            }
            catch (Exception ex)
            {
                // Log the exception using your logging mechanism
                // e.g., _logger.LogError(ex, "Error creating Khoa");
                return StatusCode(500, new
                {
                    message = "An error occurred while creating Khoa.",
                    error = ex.Message
                });
            }
        }
    }
}