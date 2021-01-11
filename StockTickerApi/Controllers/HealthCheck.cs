using Microsoft.AspNetCore.Mvc;

namespace StockTickerApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class HealthCheck : ControllerBase
    {
        public IActionResult HealthChecker()
        {
            return Ok();
        }
    }
}