using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedisPubSub.Common.Constants;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedisPubSub.Subscriber.HostedServices
{
    public class RedisSubscriberHostedService : BackgroundService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<RedisSubscriberHostedService> _logger;
        private readonly ISubscriber _subscriber;
        public RedisSubscriberHostedService(IConnectionMultiplexer connectionMultiplexer, IMemoryCache memoryCache, ILogger<RedisSubscriberHostedService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _subscriber = connectionMultiplexer.GetSubscriber();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _subscriber.SubscribeAsync(RedisChannelConstant.MemoryCache, (a, cacheKey) =>
             {
                 _memoryCache.Remove(cacheKey);
                 _logger.LogInformation($"Cache deleted. Key:{cacheKey}");
             });
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _subscriber.UnsubscribeAsync(RedisChannelConstant.MemoryCache);
            await base.StopAsync(cancellationToken);
        }
    }
}
