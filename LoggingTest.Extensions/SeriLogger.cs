using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;

namespace LoggingTest.Extensions
{
  public static class SeriLogger
  {
    public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
      (context, configuration) =>
      {
        configuration.Enrich.FromLogContext()
                     .Enrich.WithCorrelationId()
                     .Enrich.WithClientIp()
                     .Enrich.WithClientAgent()
                     .Enrich.WithMachineName()
                     .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                     .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)

                     .WriteTo.Debug()
                     .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {CorrelationId} {Level:u3} {ClientIp}] {Message} (at {Caller}){NewLine}{Exception}")
                     .WriteTo.File("./logs/myapp.txt", rollingInterval: RollingInterval.Day)
                     .WriteTo.Seq("", apiKey: "")
                     .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
                       new Uri("http://localhost:9200"))
                     {
                       AutoRegisterTemplate = true,
                       IndexFormat = $"applogs-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                       NumberOfReplicas = 0,
                       AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7
                     });
      };
  }
}