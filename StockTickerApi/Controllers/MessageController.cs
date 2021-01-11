using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockTickerApi.Logic;
using StockTickerApi.Models;

namespace StockTickerApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IMessageService _redditMessageService;

        public MessageController(
            ILogger<MessageController> logger,
            IMessageService redditMessageService)
        {
            _logger = logger;
            _redditMessageService = redditMessageService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRedditMessage(FoundMessage message)
        {
            try
            {
                var createdMessageId = await _redditMessageService.InsertMessage(message);
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