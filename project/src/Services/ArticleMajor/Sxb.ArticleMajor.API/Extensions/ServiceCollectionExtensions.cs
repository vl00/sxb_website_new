using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.Utils;
using Sxb.Framework.Cache.Redis.Configuration;
using Sxb.Framework.Cache.Redis;
using System;
using System.Data.SqlClient;
using Sxb.Framework.Cache.RedisProfiler;
using ProductManagement.Framework.MongoDb;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Sxb.ArticleMajor.Query.SQL;
using Sxb.ArticleMajor.API.Application.IntegrationEvents;

namespace Sxb.ArticleMajor.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            return services.AddMediatR(typeof(Program).Assembly);
        }

        public static IServiceCollection AddSqlServerDataBase(this IServiceCollection services, IConfigurationSection connSection)
        {
            var connStr = connSection.GetValue<string>("Master");
            var slaveConnStr = connSection.GetValue<string>("Slavers");
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                services.TryAddScoped(sp =>
                {
                    return new ArticleMajorDB() { Connection = new SqlConnection(connStr), SlaveConnection = new SqlConnection(slaveConnStr) };
                });
            }
            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IViewArticleIntegrationEventHandler, ViewArticleIntegrationEventHandler>();
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
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("OrganizationAddress")))
            {
                services.AddHttpClient("OrganizationAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("OrganizationAddress")));
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("EasemobAddress")))
            {
                services.AddHttpClient("EasemobAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("EasemobAddress")));
            }
            return services;
        }


        public static IServiceCollection AddMongodb(this IServiceCollection services, IConfigurationSection connSection)
        {
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
            //V3模式下, 必须给Guid配置序列化方式(全局或者在具体类里指定)
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            //Mongo
            return services.AddMongoDbAccessor(config =>
            {
                var dbs = connSection.Get<List<MongoDbConfig>>();
                config.ConfigName = dbs[0].ConfigName;
                config.Database = dbs[0].Database;
                config.ConnectionString = dbs[0].ConnectionString;
                config.WriteCountersign = dbs[0].WriteCountersign;
            });
        }
    }
}
