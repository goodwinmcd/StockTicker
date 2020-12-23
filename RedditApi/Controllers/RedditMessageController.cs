using System;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedditApi.Logic;

namespace RedditApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RedditMessageController : ControllerBase
    {
        private readonly ILogger<RedditMessageController> _logger;
        private readonly IRedditMessageService _redditMessageService;

        public RedditMessageController(
            ILogger<RedditMessageController> logger,
            IRedditMessageService redditMessageService)
        {
            _logger = logger;
            _redditMessageService = redditMessageService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRedditMessage(RedditMessage message)
        {
            try
            {
                var createdMessageId = await _redditMessageService.InsertRedditMessage(message);
                if (createdMessageId == -1)
                    return StatusCode(409, "Resource Already Exists");
                else
                    return StatusCode(201, createdMessageId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

    }
}