using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sxb.School.API
{
    public partial class Program
    {
        public static async Task<int> Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                //.WriteTo.Console(new RenderedCompactJsonFormatter())
                //.WriteTo.Fluentd("localhost", 30011, tag: "sxb-user-api", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
#if DEBUG
                .WriteTo.Console()
                .WriteTo.Map(
                    evt => evt.Level,
                    (level, wt) => wt.RollingFile("Logs\\" + level + "\\" + level + "-{Date}.log"))
#else
                .WriteTo.Console(new ElasticsearchJsonFormatter())
#endif
                .CreateLogger();

            Configuration.GetSection("exceptionless").Bind(Exceptionless.ExceptionlessClient.Default.Configuration);
            try
            {
                Log.Information("Starting web host");
                var builder = WebApplication.CreateBuilder(args);
                builder.WebHost.UseSerilog();
                var startup = new Startup(builder.Configuration);
                startup.ConfigureServices(builder.Services);
                builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
                builder.Host.ConfigureContainer<ContainerBuilder>(startup.ConfigureContainer);
                var app = builder.Build();
                startup.Configure(app, app.Environment);
                await startup.ConfigureAppBaseInfo(app);
                app.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}