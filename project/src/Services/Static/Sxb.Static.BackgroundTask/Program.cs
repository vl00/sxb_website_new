
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NLog;
using NLog.Extensions.Logging;
using Sxb.Static.BackgroundTask.Application.Queries;
using Sxb.Static.BackgroundTask.Infrastucture;
using System;
using System.IO;

namespace Sxb.Static.BackgroundTask
{
    public class Program
    {
        public static readonly string AppName = typeof(Program).Assembly.GetName().Name;

        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Main(string[] args)
        {

            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var host = CreateHostBuilder(args).Build();

                ServiceProvider = host.Services;
                host.Run();
            }
            catch (Exception ex)
            {
                // NLog: catch any exception and log it.
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
               Host.CreateDefaultBuilder(args)
               .ConfigureHostConfiguration(configHost =>
               {
                   configHost.AddEnvironmentVariables(prefix: "SxbStatic");
               })
               .UseWindowsService(options =>
               {
                   options.ServiceName = ".NET Static SynclogService";
               })
              .ConfigureServices((hostBuilder, services) =>
              {

                  services.AddHostedService<SynclogService>();
                  services.AddLogging(loggingBuilder =>
                  {
                      // configure Logging with NLog
                      loggingBuilder.ClearProviders();
                      loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                      loggingBuilder.AddNLog(hostBuilder.Configuration);
                  });
                  services.AddScoped<ISchoolQueries>(sp =>
                  {
                      return new SchoolQueries(Config.SqlServerConstr_School);
                  });
                  services.AddScoped<IArticleQueries>(sp =>
                  {
                      return new ArticleQueries(Config.SqlServerConstr_Article);
                  });
                  services.AddScoped<ISchoolRankQueries>(sp =>
                  {
                      return new SchoolRankQueries(Config.SqlServerConstr_Article);
                  });
                  services.AddScoped<IQuestionQueries>(sp =>
                  {
                      return new QuestionQueries(Config.SqlServerConstr_Product);
                  });
                  services.AddScoped<ICommentQueries>(sp =>
                  {
                      return new CommentQueries(Config.SqlServerConstr_Product);
                  });
                  services.AddScoped<ITalentQueries>(sp =>
                  {
                      return new TalentQueries(Config.SqlServerConstr_User);
                  });
                  services.AddScoped<ILiveQueries>(sp =>
                  {
                      return new LiveQueries(Config.SqlServerConstr_Live);
                  });
                  services.AddScoped<IEvaluationQueries>(sp =>
                  {
                      return new EvaluationQueries(Config.SqlServerConstr_Org);
                  });
                  services.AddScoped<ICourseQueries>(sp =>
                  {
                      return new CourseQueries(Config.SqlServerConstr_Org);
                  });
                  services.AddSingleton<IMongoClient>(s =>
                  {
                      return new MongoClient(Config.MongoConstr);
                  });
                  services.AddScoped((sp) =>
                  {
                      var client = sp.GetService<IMongoClient>();
                      return new StaticDbContext(client, "SxbStatic");
                  });
              })
            ;

    }
}
