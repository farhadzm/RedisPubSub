using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedisPubSub.Common.Constants;
using RedisPubSub.Common.Models;
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
            await _subscriber.SubscribeAsync(RedisChannelConstant.MemoryCache, (a, updatedData) =>
             {
                 var data = System.Text.Json.JsonSerializer.Deserialize<MemoryCacheDataDto>(updatedData);
                 _memoryCache.Remove(data.CacheKey);
                 _memoryCache.Set(data.CacheKey, data.Data);
                 _logger.LogInformation($"MemoryCache update. Key:{data.CacheKey}");
             });
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _subscriber.UnsubscribeAsync(RedisChannelConstant.MemoryCache);
            await base.StopAsync(cancellationToken);
        }
    }
}
