using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sxb.Recommend.Application.DomainEventHandler;
using Sxb.Recommend.Application.Services;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Recommend.Assist
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
             .ConfigureHostConfiguration(configHost =>
             {
                 configHost.AddEnvironmentVariables(prefix: "SxbRecommend_");
             })
            .ConfigureServices((hostBuilder,services)=> {
                //在此注入
                services.AddHostedService<SxbRecommendHostedService>();
                services.TryAddCSVFileRepository(hostBuilder.Configuration);
                services.TryAddRepository();
                services.TryAddServices();
                services.TryAddMapFeatureComputeRules();
                services.AddDomainEvent();

            });

    }
   
}
