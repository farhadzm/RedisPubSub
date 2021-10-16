using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Filters;
using Serilog.Formatting.Elasticsearch;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.RollingFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisPubSub.Extensions
{
    public static class ProgramExtensions
    {
        public static void ConfigureLogging()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Filter.ByExcluding(Matching.FromSource("System"))
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .WriteTo.Console()
                .Enrich.WithProperty("Environment", environment)
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"].ToString()))
                {
                    FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                       EmitEventFailureHandling.WriteToFailureSink |
                                       EmitEventFailureHandling.RaiseCallback,
                    FailureSink = new RollingFileSink("./failures.txt", new JsonFormatter(), null, null),
                    IndexFormat = $"{configuration["ElasticConfiguration:IndexFormat"]}-{DateTime.UtcNow:yyyy-MM}",
                    CustomFormatter = new ElasticsearchJsonFormatter(),
                    AutoRegisterTemplate = true
                })
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
