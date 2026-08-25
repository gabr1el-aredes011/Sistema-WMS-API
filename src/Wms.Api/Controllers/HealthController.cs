using Microsoft.AspNetCore.Mvc;

namespace Wms.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            service = "Sistema WMS API",
            version = "1.0.0",
            timestampUtc = DateTimeOffset.UtcNow
        });
    }
}