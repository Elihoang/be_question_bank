using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BEQuestionBank.Shared.DTOs.Khoa;
using BEQuestionBank.Shared.Helpers;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Thêmn một Khoa")]
        public async Task<IActionResult> Create([FromBody] KhoaCreateDto model)
        {
            var existingKhoa = await _service.GetByTenKhoaAsync(model.TenKhoa);

            if (existingKhoa != null)
            {
                if(existingKhoa.XoaTam)
                {
                    existingKhoa.XoaTam = false;
                    await _service.UpdateAsync(existingKhoa);
                    return Ok(existingKhoa); 
                }
                throw new ArgumentException($"Khoa with name {model.TenKhoa} already exists");
            }

            var newKhoa = new Khoa()
            {
                TenKhoa = model.TenKhoa,
                XoaTam = false
            };
            await _service.AddAsync(newKhoa);
            
            return Ok(newKhoa);
        }
        
        // GET: api/Khoa
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách Khoa")]
        public async Task<IEnumerable<Khoa>> GetAll()
        {
            return await _service.GetAllAsync();
        }
    }
}