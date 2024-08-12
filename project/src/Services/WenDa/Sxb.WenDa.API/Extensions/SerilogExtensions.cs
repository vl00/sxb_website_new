using Serilog;
using Serilog.Formatting.Elasticsearch;

namespace Sxb.WenDa.API.Extensions
{
    public static class SerilogExtensions
    {

        public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration Configuration)
        {
            Log.Logger = new LoggerConfiguration()
                   .ReadFrom
                   .Configuration(Configuration)
                   .Enrich
                   .FromLogContext()
#if DEBUG
                   .WriteTo.Console()
                   .WriteTo.File("log.txt",
                       rollingInterval: RollingInterval.Day,
                       rollOnFileSizeLimit: true)
#else
                   .WriteTo.Console(new ElasticsearchJsonFormatter())
#endif
                .CreateLogger();
            Configuration.GetSection("exceptionless").Bind(Exceptionless.ExceptionlessClient.Default.Configuration);
            return services;
        }
    }
}
