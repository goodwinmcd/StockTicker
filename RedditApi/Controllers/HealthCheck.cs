using Microsoft.AspNetCore.Mvc;

namespace RedditApi.Controllers
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