using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Globalization;
using System.Text;

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
                     .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {CorrelationId} {Level:u3} {ClientIp}] {Message:l}{NewLine}{Exception}", restrictedToMinimumLevel: LogEventLevel.Information, theme: AnsiConsoleTheme.Code)
                     //.WriteTo.File("./logs/myapp.txt", rollingInterval: RollingInterval.Day)

                     /* AWS S3 에 file direct write 가 지원되지 않으므로, 앱 내에 로그를 쌓은 후 hangfire 등을 이용해 옮기거나 s3 sink를 사용하지 않는 편이 좋다.
                      * 파일 갯수가 대단히 많이 생성됨
                      */
                     //.WriteTo.AmazonS3(
                     //  path: "myapp.txt",
                     //  bucketName: "ironpot42-logs",
                     //  endpoint: Amazon.RegionEndpoint.APNortheast2,
                     //  awsAccessKeyId: "AKIAWGFHN7YNQ5JOELMQ",
                     //  awsSecretAccessKey: "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",
                     //  restrictedToMinimumLevel: LogEventLevel.Verbose,
                     //  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                     //  formatProvider: new CultureInfo("ko-KR"),
                     //  rollingInterval: Serilog.Sinks.AmazonS3.RollingInterval.Day,
                     //  encoding: Encoding.UTF8,
                     //  failureCallback: e => Console.WriteLine($"An error occured in my sink: {e.Message}"))
                     .WriteTo.Seq("https://log.ironpot42.com", apiKey: "ENkxKUNEGWkJGYLh6H9j")
                     .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
                       new Uri("https://elasticsearch.shockz.io"))
                       {
                         AutoRegisterTemplate = true,
                         IndexFormat = $"applogs-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                         NumberOfReplicas = 0,
                         AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7
                       });
      };
  }
}