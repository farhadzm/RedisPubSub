using RedisPubSub.Common.Constants;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace RedisPubSub.Publisher.Services
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
            await _databaseAsync.PublishAsync(RedisChannelConstant.MemoryCache, "keyForDelete");
        }
    }
}
