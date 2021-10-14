using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace RedisPubSub.Common.Extensions
{
  public static  class StartupExtension
    {
        public static void AddRedisService(this IServiceCollection services, IConfiguration configuration)
        {
            var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints =
                {
                    $"{configuration.GetValue<string>("RedisCache:Host")}:{configuration.GetValue<int>("RedisCache:Port")}"
                }
            });
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        }
    }
}
