using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.Utils;
using Sxb.Framework.Cache.Redis.Configuration;
using Sxb.Framework.Cache.RedisProfiler;
using Sxb.Framework.Cache.Redis;
using Sxb.WenDa.API;
using Sxb.WenDa.Infrastructure;
using Sxb.WenDa.Query.SQL;
using System;
using System.Data.SqlClient;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;
using Sxb.WenDa.API.Application.IntegrationEventHandlers;

namespace Sxb.WenDa.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LocalContextTransactionBehavior<,>));
            return services.AddMediatR(typeof(Question).Assembly, typeof(Program).Assembly);
        }

        public static IServiceCollection AddDomainContext(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            return services.AddDbContext<LocalDbContext>(optionsAction);
        }

        public static IServiceCollection AddInMemoryDomainContext(this IServiceCollection services)
        {
            return services.AddDomainContext(builder => builder.UseInMemoryDatabase("domanContextDatabase"));
        }

        public static IServiceCollection AddMsSqlDomainContext(this IServiceCollection services, string connectionString)
        {
            return services.AddDomainContext(builder =>
            {
                builder.UseSqlServer(connectionString);
            });
        }

        public static IServiceCollection AddSqlServerDataBase(this IServiceCollection services, IConfigurationSection connSection)
        {
            var connStr = connSection.GetValue<string>("Master");
            var slaveConnStr = connSection.GetValue<string>("Slavers");
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                services.TryAddScoped(sp =>
                {
                    return new LocalQueryDB() { Connection = new SqlConnection(connStr), SlaveConnection = new SqlConnection(slaveConnStr) };
                });
            }
            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<INotifyIntegrationEventHandler, NotifyIntegrationEventHandler>();

            services.AddCap(options =>
            {
                //EventBus和Context共享数据库连接，以便在同一事务中。
                //options.UseEntityFramework<UserContext>();
                options.UseSqlServer(configuration.GetSection("ConnectionString").GetValue<string>("Master"));
                options.UseRabbitMQ(options =>
                {
                    configuration.GetSection("RabbitMQ").Bind(options);
                });
#if DEBUG
                options.UseDashboard();
#endif
            });

            return services;
        }


        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            //Redis
            var redisConfig = configuration.GetSection("RedisConfig").Get<RedisConfig>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<MyRedisProfiler>();
            services.AddSingletonCacheRedis(config =>
            {
                config.Database = redisConfig.Database;
                config.RedisConnect = redisConfig.RedisConnect;
                config.CloseRedis = redisConfig.CloseRedis;
                config.HaveLog = redisConfig.HaveLog;
            }, new Sxb.Framework.Cache.Redis.Serializer.NewtonsoftSerializer(new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            }));
            return services;
        }


        public static IServiceCollection AddHttpClientBase(this IServiceCollection services, IConfigurationSection connSection)
        {
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("FinanceCenterAddress")))
            {
                services.AddHttpClient("FinanceCenterAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("FinanceCenterAddress")));
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("MarketingAddress")))
            {
                services.AddHttpClient("MarketingAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("MarketingAddress")));
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("WeChatAddress")))
            {
                services.AddHttpClient("WxChatAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("WeChatAddress")));
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("OperationAddress")))
            {
                services.AddHttpClient("OperationAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("OperationAddress")));
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("OrganizationAddress")))
            {
                services.AddHttpClient("OrgAPI", http =>
                {
                    http.BaseAddress = new Uri(connSection.GetValue<string>("OrganizationAddress"));
                    http.Timeout = TimeSpan.Parse("00:00:30");
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var ch = new HttpClientHandler();
                    ch.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                    return ch;
                });
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("Sxb-OpenApi")))
            {
                services.AddHttpClient("Sxb-OpenApi", http =>
                {
                    http.BaseAddress = new Uri(connSection.GetValue<string>("Sxb-OpenApi"));
                    http.Timeout = TimeSpan.Parse("00:00:30");
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var ch = new HttpClientHandler();
                    ch.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                    return ch;
                });
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("WxWorkApi")))
            {
                services.AddHttpClient("WxWorkApi", http =>
                {
                    http.BaseAddress = new Uri(connSection.GetValue<string>("WxWorkApi"));
                    http.Timeout = TimeSpan.Parse("00:00:30");
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var ch = new HttpClientHandler();
                    ch.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                    return ch;
                });
            }

            return services;
        }
    }
}
