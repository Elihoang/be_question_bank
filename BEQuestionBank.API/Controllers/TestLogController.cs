using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BEQuestionBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestLogController(ILogger<TestLogController> logger) : ControllerBase
    {
        private readonly ILogger<TestLogController> _logger = logger;
        
        [HttpGet("log")]
        public IActionResult CreateTestLog()
        {
            _logger.LogInformation("🟢 Đây là log Information");
            _logger.LogWarning("🟡 Đây là log Warning");
            _logger.LogError("🔴 Đây là log Error");

            // 🧪 Log thông tin biến động
            string name = "Ngô Mạnh Hùng";
            _logger.LogInformation("📌 Tên người dùng là: {Name}", name);

            // 🧪 Log exception
            try
            {
                int a = 0;
                int b = 10 / a; // Lỗi chia cho 0
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Có lỗi xảy ra khi chia số");
            }

            return Ok("✅ Đã ghi log đầy đủ (Info, Warning, Error, Biến, Exception)");
        }
    }
}
