using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisPubSub.Publisher.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisPubSub.Publisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IRedisPublisher _redisPublisher;

        public PublisherController(IRedisPublisher redisPublisher)
        {
            _redisPublisher = redisPublisher;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _redisPublisher.PublishMessage();
            return Ok();
        }
    }
}
