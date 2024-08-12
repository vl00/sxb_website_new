using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sxb.Static.BackgroundTask.Middleware;
using Sxb.Static.BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask
{
    public class SynclogService : BackgroundService
    {
        IServiceProvider _serviceProvider;
        ILogger<SynclogService> _logger;
        public SynclogService(IServiceProvider serviceProvider, ILogger<SynclogService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool cancelled = false;
            ThreadPool.SetMaxThreads(100, 1000);

            using (var consumer = new ConsumerBuilder<Ignore, string>(Config.KafkaConfig).Build())
            {
                consumer.Subscribe("statistic");
                while (!cancelled)
                {

                    var consumeResult = consumer.Consume(stoppingToken);
                    ThreadPool.QueueUserWorkItem(async (consumeResult) =>
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var context = new KafkaContext()
                            {
                                ConsumeResult = consumeResult,
                                Services = scope.ServiceProvider,
                                CancellationToken = stoppingToken
                            };
                            try
                            {

                                Pipline pipline = new Pipline()
                                    .AddMiddleware(new DefaultMiddleware())
                                    .AddMiddleware(new SchoolMiddleware())
                                    .AddMiddleware(new ArticleMiddleware())
                                    .AddMiddleware(new SchoolRankMiddleware())
                                    .AddMiddleware(new TopicCircleMiddleware())
                                    .AddMiddleware(new TalentMiddleware())
                                    .AddMiddleware(new CommentMiddleware())
                                    .AddMiddleware(new QAMiddleware())
                                    .AddMiddleware(new LiveMiddleware())
                                    .AddMiddleware(new EvaluationMiddleware())
                                    .AddMiddleware(new CourseMiddleware());
                                await pipline.Start(context);
                            }
                            catch(Exception ex){
                                _logger.LogError(ex, "管道发生异常： Url = {url}", context.AsKafkaLog()?.Url);

                            }

                        }

                    }
                    , consumeResult
                    , false);

                }

                consumer.Close();
            }
            return Task.CompletedTask;
        }
    }
}
