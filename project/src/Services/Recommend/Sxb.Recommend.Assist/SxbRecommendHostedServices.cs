using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sxb.Recommend.Application.Services;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Sxb.Recommend.Assist
{

    /// <summary>
    /// 自定义主机服务，里面有启动和结束两个时间
    /// </summary>
    public class SxbRecommendHostedService : BackgroundService
    {
        IHostApplicationLifetime _appLifetime;
        IHostLifetime _hostLifetime;
        IHostEnvironment _hostEnvironment;
        IServiceProvider _sp;
        public SxbRecommendHostedService(
            IHostApplicationLifetime appLifetime
            , IHostLifetime hostLifetime
            , IServiceProvider serviceProvider
            , IHostEnvironment hostEnvironment
            )
        {
            _appLifetime = appLifetime;
            _hostLifetime = hostLifetime;
            _sp = serviceProvider;
            _hostEnvironment = hostEnvironment;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _sp.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<SxbRecommendHostedService>>();
                var recommendService = scope.ServiceProvider.GetService<IRecommendService>();
                var mapFeatureService = scope.ServiceProvider.GetService<IMapFeatureService>();
                var schoolRedirectFrequencyService = scope.ServiceProvider.GetService<ISchoolRedirectFrequencyService>();
                Console.WriteLine("=================请选择你的操作=================");
                Console.WriteLine("0->结束;1->初始化特性;2->同步学校副本;3->刷新学校跳转频率;4->生成学校分数冷启动数据");
                string readline;
                while ((readline = Console.ReadLine()) != "0")
                {
                    switch (readline)
                    {

                        case "1":
                            Console.WriteLine("========================= 初始化特性 ============================");
                            await mapFeatureService.InitialFeatures();
                            Console.WriteLine("========================= 初始化特性结束 ============================");
                            break;
                        case "2":
                            Console.WriteLine("========================= 同步学校副本 ============================");
                            recommendService();
                            Console.WriteLine("========================= 同步学校结束 ============================");
                            break;
                        case "3":
                            Console.WriteLine("========================= 刷新学校跳转频率 ============================");
                            await schoolRedirectFrequencyService.FlushFrequency();
                            Console.WriteLine("========================= 刷新学校跳转频率结束 ============================");
                            break;
                        case "4":
                            Console.WriteLine("========================= 生成学校分数冷启动数据 ============================");
                            await recommendService.InsertSchoolMaps();
                            Console.WriteLine("========================= 生成学校分数冷启动数据结束 ============================");
                            break;
                    }
                    Console.WriteLine("=================请选择你的操作=================");
                    Console.WriteLine("0->结束;1->初始化特性;2->同步学校副本;3->刷新学校跳转频率;4->生成学校分数冷启动数据");

                }

                _appLifetime.StopApplication();
            }

        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
