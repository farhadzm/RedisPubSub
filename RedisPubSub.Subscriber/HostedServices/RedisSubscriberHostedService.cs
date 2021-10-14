using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using RedisPubSub.Common.Constants;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedisPubSub.Subscriber.HostedServices
{
    public class RedisSubscriberHostedService : BackgroundService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IMemoryCache _memoryCache;

        public RedisSubscriberHostedService(IConnectionMultiplexer connectionMultiplexer, IMemoryCache memoryCache)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _memoryCache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = _connectionMultiplexer.GetSubscriber();
            await subscriber.SubscribeAsync(RedisChannelConstant.MemoryCache, (a, cacheKey) =>
             {
                 _memoryCache.Remove(cacheKey);
             });
        }
    }
}
