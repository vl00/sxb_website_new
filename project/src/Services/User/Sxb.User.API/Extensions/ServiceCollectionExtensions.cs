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
using Sxb.User.API.Application.IntegrationEvents;
using Sxb.User.Domain.AggregatesModel.TalentAggregate;
using Sxb.User.Infrastructure;
using Sxb.User.Query.SQL.DB;
using System;
using System.Data.SqlClient;
using Sxb.Framework.Cache.Redis;

namespace Sxb.User.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
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
            }, new Framework.Cache.Redis.Serializer.NewtonsoftSerializer(new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            }));
            return services;
        }

        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UserContextTransactionBehavior<,>));
            return services.AddMediatR(typeof(Talent).Assembly, typeof(Program).Assembly);
        }

        public static IServiceCollection AddDomainContext(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            return services.AddDbContext<UserContext>(optionsAction);
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
            //return services.AddDbContext<UserContext>(options =>
            //    {
            //        options.UseSqlServer(connectionString,
            //            sqlServerOptionsAction: sqlOptions =>
            //            {
            //                sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            //                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            //            });
            //    },
            //    ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            //);
        }

        public static IServiceCollection AddSqlServerDataBase(this IServiceCollection services, IConfigurationSection connSection)
        {
            var connStr = connSection.GetValue<string>("Master");
            var slaveConnStr = connSection.GetValue<string>("Slavers");
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                services.TryAddScoped(sp =>
                {
                    return new SchoolUserDB() { Connection = new SqlConnection(connStr), SlaveConnection = new SqlConnection(slaveConnStr) };
                });
            }
            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISubscriberService, SubscriberService>();
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
    }
}
