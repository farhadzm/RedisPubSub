# RedisPubSub
In this case when **MemoryCache** updated in one instance, a message send to **Redis** channel, then a **HostedService** recieve data from **Redis** channel and update **MemoryCache** of Application.
```csharp
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

```
**PublisherService**:
```csharp
public class RedisPublisher : IRedisPublisher
{
    private readonly IDatabaseAsync _databaseAsync;
    public RedisPublisher(IConnectionMultiplexer redisCachingProvider)
    {
        _databaseAsync = redisCachingProvider.GetDatabase();
    }
    public async Task PublishMessage()
    {
        var updatedData = new MemoryCacheDataDto
        {
            CacheKey = "user_information",
            Data = 20
        };
        var redisChannelData = System.Text.Json.JsonSerializer.Serialize(updatedData);
        await _databaseAsync.PublishAsync(RedisChannelConstant.MemoryCache, redisChannelData);
    }
}
```
