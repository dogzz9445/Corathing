using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Corathing.Organizer.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController() : ControllerBase
{
    /// <summary>
    /// 애플리케이션이 살아있는지 확인하는 간단한 헬스체크
    /// </summary>
    /// <returns>OK 응답</returns>
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        try
        {
            return Ok(new { message = "pong", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Health ping check failed");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 애플리케이션 상태 정보를 반환
    /// </summary>
    /// <returns>애플리케이션 상태 정보</returns>
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        try
        {
            var status = new
            {
                applicationName = "Corathing Server",
                status = "healthy",
                version = "1.0.0", // 실제 버전으로 변경
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                serverTime = DateTime.UtcNow,
                uptime = GetUptime(),
                machineName = Environment.MachineName
            };

            return Ok(status);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get application status");
            return StatusCode(500, new
            {
                status = "unhealthy",
                error = "Internal server error",
                timestamp = DateTime.UtcNow
            });
        }
    }

    private string GetUptime()
    {
        var uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
        return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
    }
}
