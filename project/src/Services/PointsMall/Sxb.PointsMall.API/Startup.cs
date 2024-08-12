using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.Middleware;
using Sxb.Framework.AspNetCoreHelper.Utils;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Cache.Redis.Configuration;
using Sxb.Framework.Cache.RedisProfiler;
using Sxb.PointsMall.API.Application.Behaviors;
using Sxb.PointsMall.API.Application.Queries.AccountPoints;
using Sxb.PointsMall.API.Application.Queries.AccountPointsItem;
using Sxb.PointsMall.API.Application.Queries.UserPointsTask;
using Sxb.PointsMall.API.Application.Queries.UserSignInInfo;
using Sxb.PointsMall.API.Infrastructure.DistributedLock;
using Sxb.PointsMall.API.Infrastructure.Middlewares;
using Sxb.PointsMall.API.Infrastructure.Services;
using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using Sxb.PointsMall.Domain.AggregatesModel.UserSignInInfoAggregate;
using Sxb.PointsMall.Infrastructure;
using Sxb.PointsMall.Infrastructure.Repositories;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API
{
    public class Startup
    {
        IConfiguration Configuration;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerDocument(); // add OpenAPI v3 document
            services.AddPointsMallDbContext();
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            services.AddBehaviors();
            services.AddRepositories();
            services.AddQueries();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = context =>
                        {
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                    };
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Name = "iSchoolAuth";
                    options.Cookie.Domain = ".sxkid.com";
                    options.Cookie.Path = "/";
                    options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo("shared-auth-ticket-keys"));

                });
            services.AddRedis(Configuration);
            services.AddEventBus(Configuration);
            services.AddHttpService(Configuration.GetSection("ExternalInterface"));

            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCustomExceptionCatch();

            if (env.IsDevelopment())
            {
#if DEBUG
                app.UseSwaggerUi3(p =>
                {
                    p.DocumentPath = "/swagger/{documentName}/swagger.json";//手动设置一下默认,开启explore功能。
                });
                app.UseOpenApi();
#else
                app.UseSwaggerUi3(p =>
                {
                    p.ServerUrl = "/pointsmall";
                    p.DocumentPath = "/pointsmall/swagger/{documentName}/swagger.json";
                });
                app.UseOpenApi(p =>
                {
                    p.PostProcess = (d, r) =>
                    {
                        var documentBaseUrl = d.BaseUrl;
                        d.Servers.Clear();
                        d.Servers.Add(new NSwag.OpenApiServer()
                        {
                            Description = "Lonlykids",
                            Url = $"/pointsmall"
                        }) ;
                    };
                });
#endif
            }
            //app.UseReDoc(); // serve ReDoc UI
            app.UseStatusCodeMiddleware();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }


    }

    public static class CustomDIExtesionMethods
    {
        public static IServiceCollection AddHttpService(this IServiceCollection services, IConfigurationSection connSection)
        {
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("UserAddress")))
            {
                services.AddHttpClient("UserAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("UserAddress")));
            }


            services.AddScoped<IUserService, UserService>();
            return services;
        }

        public static IServiceCollection AddPointsMallDbContext(this IServiceCollection services)
        {
            services.AddScoped<PointsMallDbContext>(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                string iSchoolPointsMall_Constr = configuration.GetConnectionString("iSchoolPointsMall");
                return PointsMallDbContext.CreateSqlConnectionFrom(iSchoolPointsMall_Constr);

            });
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {

            services.AddScoped<IAccountPointsRepository, AccountPointsRepository>();
            services.AddScoped<IUserSignInInfoRepository, UserSignInInfoRepository>();
            services.AddScoped<IUserPointsTaskRepository, UserPointsTaskRepository>();
            services.AddScoped<IAccountPointsItemRepository, AccountPointsItemRepository>();
            return services;

        }


        public static IServiceCollection AddQueries(this IServiceCollection services)
        {

            services.AddScoped<IAccountPointsQueries, AccountPointsQueries>(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                string iSchoolPointsMall_Read_Constr = configuration.GetConnectionString("iSchoolPointsMall.Read");
                return new AccountPointsQueries(iSchoolPointsMall_Read_Constr);
            });
            services.AddScoped<IUserSignInInfoQueries, UserSignInInfoQueries>(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                string iSchoolPointsMall_Read_Constr = configuration.GetConnectionString("iSchoolPointsMall.Read");
                return new UserSignInInfoQueries(iSchoolPointsMall_Read_Constr);
            });
            services.AddScoped<IUserPointsTaskQueries, UserPointsTaskQueries>(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                string iSchoolPointsMall_Read_Constr = configuration.GetConnectionString("iSchoolPointsMall.Read");
                return new UserPointsTaskQueries(iSchoolPointsMall_Read_Constr);
            });
            services.AddScoped<IAccountPointsItemQueries, AccountPointsItemQueries>(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                string iSchoolPointsMall_Read_Constr = configuration.GetConnectionString("iSchoolPointsMall.Read");
                return new AccountPointsItemQueries(iSchoolPointsMall_Read_Constr);
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
            }, new Framework.Cache.Redis.Serializer.NewtonsoftSerializer(new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            }));

            services.AddScoped<IDistributedLockFactory, DistributedLockFactory>(sp =>
            {
                var easyRedisClient = sp.GetRequiredService<IEasyRedisClient>();
                return new DistributedLockFactory(easyRedisClient, 60);
            });
            return services;
        }


        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
#if !DEBUG
            services.AddTransient<Application.IntegrationEvents.IAddUserPointsTaskIntegrationEventHandler, Application.IntegrationEvents.AddUserPointsTaskIntegrationEventHandler>();
#endif
            services.AddCap(options =>
            {
                options.UseSqlServer(configuration.GetSection("ConnectionStrings").GetValue<string>("iSchoolPointsMall"));
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

        public static IServiceCollection AddBehaviors(this IServiceCollection services)
        {
            //注入顺序决定管道执行顺序，先注入先执行。
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UserExcuteOnlyBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            return services;
        }
    }
}
