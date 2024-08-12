using DotNetCore.CAP.Dashboard.NodeDiscovery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Polly;
using Sxb.Framework.Cache.Redis.Configuration;
using Sxb.Settlement.API.Application.Queries;
using Sxb.Settlement.API.GaoDeng;
using Sxb.Settlement.API.HttpDelegationHandler;
using Sxb.Settlement.API.Infrastucture.Repositories;
using Sxb.Settlement.API.Middlewares;
using Sxb.Settlement.API.Model;
using Sxb.Settlement.API.Services;
using Sxb.Settlement.API.Services.Aliyun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sxb.Framework.Cache.Redis;

namespace Sxb.Settlement.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Redis
            var redisConfig = Configuration.GetSection("RedisConfig").Get<RedisConfig>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingletonCacheRedis(config =>
            {
                config.Database = redisConfig.Database;
                config.RedisConnect = redisConfig.RedisConnect;
                config.CloseRedis = redisConfig.CloseRedis;
                config.HaveLog = redisConfig.HaveLog;
            }, new Sxb.Framework.Cache.Redis.Serializer.NewtonsoftSerializer());

            services.Configure<GaoDengOption>(Configuration.GetSection(GaoDengOption.GaoDengConfig));
            services
                .AddHttpClient<IGaoDengService, GaoDengService>()
                .AddHttpMessageHandler<LogDelegationHandler>();

            services.Configure<AliyunOption>(Configuration.GetSection(AliyunOption.AliyunConfig));
            services
                .AddHttpClient<IAliyunService, AliyunService>()
                .AddHttpMessageHandler<LogDelegationHandler>();

            //策略：不断地重试，重试间隔2的重试次数次方秒。
            services.AddTransient<LogDelegationHandler>();
            services
                .AddHttpClient<IHttpCallBackNotifyService, HttpCallBackNotifyService>()
                .AddHttpMessageHandler<LogDelegationHandler>()
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));



            services.AddScoped<IIDCardRepository>(sp =>
            {
                string constr =  sp.GetService<IConfiguration>().GetConnectionString("dbo.iSchoolUsers");
                return new IDCardRepository(constr);
            });
            services.AddScoped<IUserRepository>(sp =>
            {
                string constr = sp.GetService<IConfiguration>().GetConnectionString("dbo.iSchoolUsers");
                return new UserRepository(constr);
            });
            services.AddScoped<ISettlementReposiroty>(sp =>
            {
                string constr = sp.GetService<IConfiguration>().GetConnectionString("dbo.iSchoolSettlement");
                return new SettlementReposiroty(constr);
            });
            services.AddScoped<IUserIDCardQueries>(sp =>
            {
                string constr = sp.GetService<IConfiguration>().GetConnectionString("dbo.iSchoolUsers");
                return new UserIDCardQueries(constr);
            });

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options=> {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Name = "iSchoolAuth";
                    options.Cookie.Domain = ".sxkid.com";
                    options.Cookie.Path = "/";
                    options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo("shared-auth-ticket-keys"));

                });


            services.AddCap(x =>
            {
                // 注册 Dashboard
                x.UseDashboard();

                // 注册节点到 Consul
                //x.UseDiscovery(d =>
                //{
                //    d.DiscoveryServerHostName = "localhost";
                //    d.DiscoveryServerPort = 8500;
                //    d.CurrentNodeHostName = "james.sxkid.com";
                //    d.CurrentNodePort = 5001;
                //    d.NodeId = "Sxb.Settlement.API_1";
                //    d.NodeName = "CAP Sxb.Settlement.API No.1";
                //});

                x.DefaultGroupName = "cap-settlement.api_default";
                x.GroupNamePrefix = "cap-settlement.api";
                //如果你使用的ADO.NET，根据数据库选择进行配置：
                x.UseSqlServer(Configuration.GetConnectionString("dbo.iSchoolSettlement"));

                //CAP支持 RabbitMQ、Kafka、AzureServiceBus、AmazonSQS 等作为MQ，根据使用选择配置：
                x.UseRabbitMQ(options=> {
                    Configuration.GetSection("RabbitMQ").Bind(options);
                });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sxb.Settlement.API", Version = "v1" });
            });
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sxb.Settlement.API v1"));
            }

            app.UseCustomExceptionCatch();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
