using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Sxb.WeWorkFinance.API;
using System;
using System.IO;

try
{
    IConfiguration Configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
#if DEBUG
        .WriteTo.Console()
#else
        .WriteTo.Console(new ElasticsearchJsonFormatter())
#endif
    .WriteTo.Fluentd("localhost", 30011, tag: "sxb-user-api", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
    .WriteTo.Map(
         evt => evt.Level,
         (level, wt) => wt.RollingFile("Logs\\" + level + "\\" + level + "-{Date}.log"))
    .CreateLogger();
    Configuration.GetSection("exceptionless").Bind(Exceptionless.ExceptionlessClient.Default.Configuration);

    Log.Information("Starting web host");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.WebHost.UseSerilog();

    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>(startup.ConfigureContainer);
    var app = builder.Build();
    startup.Configure(app, app.Environment);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
