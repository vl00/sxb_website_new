using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.Utils;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Cache.Redis.Configuration;
using Sxb.Framework.Cache.RedisProfiler;
using Sxb.School.API.Application.Behaviors;
using Sxb.School.API.Application.Queries.DgAyOrder;
using Sxb.School.API.Application.Queries.DegreeAnalyze;
using Sxb.School.API.Application.Queries.SchoolViewOrder;
using Sxb.School.API.Application.Queries.UserInfo;
using Sxb.School.API.Infrastructures.Services;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using Sxb.School.Domain.AggregateModels.ViewOrder;
using Sxb.School.Domain.AggregateModels.ViewPermission;
using Sxb.School.Infrastructure;
using Sxb.School.Infrastructure.Repositories;
using Sxb.School.Query.SQL.DB;
using System.Data.SqlClient;
using System.Net.Http;
using Sxb.School.API.Application.Queries.DgAyUserQaPaper;
using Sxb.School.Domain.AggregateModels.DgAyUserQaPaperAggregate;
using Sxb.School.API.Application.Queries.DgAyAddCustomerUser;

namespace Sxb.School.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            return services.AddMediatR(typeof(Program).Assembly);
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
            }, new Framework.Cache.Redis.Serializer.NewtonsoftSerializer(new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            }));
            return services;
        }
        public static IServiceCollection AddSqlServerDataBase(this IServiceCollection services, IConfigurationSection connSection)
        {
            var connStr = connSection.GetValue<string>("Master");
            var slaveConnStr = connSection.GetValue<string>("Slavers");
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                services.TryAddScoped(sp =>
                {
                    return new SchoolDataDB() { Connection = new SqlConnection(connStr), SlaveConnection = new SqlConnection(slaveConnStr) };
                });
            }
            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
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

        public static IServiceCollection AddHttpClientBase(this IServiceCollection services, IConfigurationSection connSection)
        {
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("CommentAddress")))
            {
                services.AddHttpClient("CommentAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("CommentAddress")));
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("ArticleAddress")))
            {
                services.AddHttpClient("ArticleAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("ArticleAddress")));
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("UserAddress")))
            {
                services.AddHttpClient("UserAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("UserAddress")));
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("PaidQAAddress")))
            {
                services.AddHttpClient("PaidQAAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("PaidQAAddress")));
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("OrganizationAddress")))
            {
                services.AddHttpClient("OrganizationAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("OrganizationAddress")));
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("WxWorkAddress")))
            {
                services.AddHttpClient("WxWorkAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("WxWorkAddress")));
            }
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                return new SchoolDataDbContext(new SqlConnection(configuration["ConnectionString:Master"]));
            });
            services.AddScoped<ISchoolViewOrderRepository, SchoolViewOrderRepository>();
            services.AddScoped<ISchoolViewPermissionRepository, SchoolViewPermissionRepository>();
            services.AddScoped<IDgAyOrderRepository, DgAyOrderRepository>();
            services.AddScoped<IDgAyUserQaPaperRepository, DgAyUserQaPaperRepository>();
            return services;

        }

        public static IServiceCollection AddQueries(this IServiceCollection services)
        {
            services.AddScoped<ISchoolViewOrderQueries, SchoolViewOrderQueries>(sp =>
             {
                 var configuration = sp.GetService<IConfiguration>();
                 return new SchoolViewOrderQueries(configuration["ConnectionString:Slavers"]);
             });

            services.AddScoped<IDgAyOrderQueries, DgAyOrderQueries>(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                return new DgAyOrderQueries(configuration["ConnectionString:Slavers"]);
            });
            services.AddScoped<IDgAyUserQaPaperQueries, DgAyUserQaPaperQueries>(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                return new DgAyUserQaPaperQueries(configuration["ConnectionString:Slavers"]);
            });

            services.AddScoped<IUserInfoQueries, UserInfoQueries>(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                return new UserInfoQueries(configuration["ConnectionString:Slavers"]);
            });
            services.AddScoped<IDgAyAddCustomerUserQueries, DgAyAddCustomerUserQueries>(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                return new DgAyAddCustomerUserQueries(configuration["ConnectionString:Slavers"]);
            });


            services.AddScoped<IDegreeAnalyzeQueries, DegreeAnalyzeQueries>();
            return services;

        }


        public static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddHttpClient("InnerClient", async (sp, client) =>
            {
                var redisClient = sp.GetService<IEasyRedisClient>();
                string secret = await redisClient.GetStringAsync(RedisCacheKeys.AppIDKey);
                client.DefaultRequestHeaders.TryAddWithoutValidation("appid", Configs.AppID);
                client.DefaultRequestHeaders.TryAddWithoutValidation("appsecret", secret);
            });
            services.AddScoped<IWeChatGatewayService, WeChatGatewayService>();
            services.AddScoped<IUserCenterService, UserCenterService>();
            services.AddScoped<IPayGatewayService, PayGatewayService>();
            services.AddScoped<IWxWorkService, WxWorkService>();
            return services;

        }
        public static IServiceCollection AddBehaviors(this IServiceCollection services)
        {
            //注入顺序决定管道执行顺序，先注入先执行。
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            return services;
        }
    }
}
