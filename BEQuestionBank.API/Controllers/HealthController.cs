using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BEQuestionBank.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private static readonly Stopwatch _uptime = Stopwatch.StartNew();
        
        [HttpGet]
        public IActionResult GetAppInfo()
        {
            var info = new
            {
                Name = "BEQuestionBank.API",
                Version = "1.0.0",
                Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            };
            return Ok(info);
        }
        
        [HttpGet("health")]
        [SwaggerOperation(
            Summary = "Health check endpoint",
            Description = "Trả về trạng thái sức khỏe hiện tại của hệ thống."
        )]
        public IActionResult HealthCheck()
        {
            var response = new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow.ToString("o"), // ISO 8601 format
                uptime = Math.Round(_uptime.Elapsed.TotalSeconds, 9),
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown",
                version = "1.0.0"
            };

            return Ok(response);
        }

        [HttpGet("ready")]
        public IActionResult Ready()
        {
            var response = new
            {
                status = "ready",
                timestamp = DateTime.UtcNow.ToString("o"), // ISO 8601 format
                uptime = Math.Round(_uptime.Elapsed.TotalSeconds, 9),
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown",
                version = "1.0.0"
            };

            return Ok(response);
        }

        [HttpGet("live")]
        public IActionResult Live()
        {
            var response = new
            {
                status = "live",
                timestamp = DateTime.UtcNow.ToString("o"), // ISO 8601 format
                uptime = Math.Round(_uptime.Elapsed.TotalSeconds, 9),
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown",
                version = "1.0.0"
            };

            return Ok(response);
        }
    }
}
