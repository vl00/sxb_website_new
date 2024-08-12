using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Elasticsearch;

namespace Sxb.ArticleMajor.API.Extensions
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
                   .WriteTo.Map(
                       evt => evt.Level,
                       (level, wt) => wt.RollingFile("Logs\\" + level + "\\" + level + "-{Date}.log"))
            #else
                   .WriteTo.Console(new ElasticsearchJsonFormatter())
            #endif
                .CreateLogger();
            Configuration.GetSection("exceptionless").Bind(Exceptionless.ExceptionlessClient.Default.Configuration);
            return services;
        }
    }
}
