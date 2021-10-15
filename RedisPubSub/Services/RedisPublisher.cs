using RedisPubSub.Constants;
using RedisPubSub.Models;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace RedisPubSub.Services
{
    public interface IRedisPublisher
    {
        Task PublishMessage();
    }

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
}
