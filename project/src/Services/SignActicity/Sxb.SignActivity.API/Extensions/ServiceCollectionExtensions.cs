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
using Sxb.SignActivity.API;
using Sxb.SignActivity.API.Application.IntegrationEvents;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using Sxb.SignActivity.Infrastructure;
using Sxb.SignActivity.Query.SQL;
using System;
using System.Data.SqlClient;
using Sxb.SignActivity.Common.OtherAPIClient.Marketing;

namespace Sxb.Article.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OrganizationContextTransactionBehavior<,>));
            return services.AddMediatR(typeof(SignIn).Assembly, typeof(Program).Assembly);
        }

        public static IServiceCollection AddDomainContext(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            return services.AddDbContext<OrganizationContext>(optionsAction);
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
                    return new OrgDB() { Connection = new SqlConnection(connStr), SlaveConnection = new SqlConnection(slaveConnStr) };
                });
            }
            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
#if !DEBUG
            services.AddTransient<IOrderPayIntegrationEventHandler, OrderPayIntegrationEventHandler>();
            services.AddTransient<IOrderRefundIntegrationEventHandler, OrderRefundIntegrationEventHandler>();
            services.AddTransient<IOrderShippedIntegrationEventHandler, OrderShippedIntegrationEventHandler>();
            services.AddTransient<IWeChatMsgIntegrationEventHandler, WeChatMsgIntegrationEventHandler>();
#endif

            services.AddCap(options =>
            {
                //EventBus和Context共享数据库连接，以便在同一事务中。
                //options.UseEntityFramework<UserContext>();
                options.UseSqlServer(configuration.GetSection("ConnectionString").GetValue<string>("Master"));
                options.UseRabbitMQ(options =>
                {
                    configuration.GetSection("RabbitMQ").Bind(options);
                });
                //options.UseDashboard();
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
            return services;
        }
    }
}
