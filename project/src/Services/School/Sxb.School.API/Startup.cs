using Autofac;
using Exceptionless;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Sxb.Framework.AspNetCoreHelper.Filters;
using Sxb.Framework.AspNetCoreHelper.Utils;
using Sxb.School.API.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sxb.School.API
{
    using MediatR;
    using Sxb.Framework.Cache.Redis;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        internal static bool Ready { get; set; } = true;
        internal static bool Live { get; set; } = true;
        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidateModelResultFilter>();
            });

            services.AddHealthChecks()
                .AddSqlServer(Configuration.GetSection("ConnectionString").GetValue<string>("Master"), "select 1;", tags: new string[] { "mssql", "live", "all" })
                .AddRabbitMQ(s =>
                {
                    var connectionFactory = new RabbitMQ.Client.ConnectionFactory();
                    Configuration.GetSection("RabbitMQ").Bind(connectionFactory);
                    return connectionFactory;
                }, "rabbitmq", tags: new string[] { "rabbitmq", "live", "all" })
                .AddCheck("liveChecker", () =>
                {
                    return Live ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
                }, new string[] { "live", "all" })
                .AddCheck("readyChecker", () =>
                {
                    return Ready ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
                }, new string[] { "ready", "all" });

            services.AddControllers().AddNewtonsoftJson().AddXmlSerializerFormatters();
            services.AddSwaggerDocument();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            //services.AddMediatRServices();
            services.AddRedis(Configuration);
            //services.AddMsSqlDomainContext(Configuration.GetSection("ConnectionString").GetValue<string>("Master"));
            services.AddEventBus(Configuration);
            services.AddSqlServerDataBase(Configuration.GetSection("ConnectionString"));
            services.AddHttpClientBase(Configuration.GetSection("ExternalInterface"));
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardedHeaders = ForwardedHeaders.All;
            });
            services.AddCors(p => p.AddPolicy("LimitRequests", x =>
            {
                x.SetIsOriginAllowed(o => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            }));
            services.AddRepositories();
            services.AddQueries();
            services.AddExternalServices();

            #region 登陆状态相关 (需增加 app.UseAuthentication();)
            string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/shared-auth-ticket-keys/";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        if (context.Request.IsAjax())
                        {
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                        var currentUrl = new UriBuilder(context.RedirectUri);
                        var returnUrl = new UriBuilder
                        {
                            Scheme = currentUrl.Scheme,
                            Host = currentUrl.Host,
                            Port = currentUrl.Port,
                            Path = context.Request.Path,
                            Query = string.Join("&", context.Request.Query.Select(q => q.Key + "=" + q.Value))
                        };
                        var userServiceUrl = (Configuration.GetSection("UserSystemConfig")
                        .GetValue<string>("ServerUrl") ?? "")
                        .Replace("https://", "")
                        .Replace("http://", "");
                        var redirectUrl = new UriBuilder
                        {
                            Scheme = "https",
                            Host = userServiceUrl,
                            Path = new PathString("/login/"),
                            Query = QueryString.Create(context.Options.ReturnUrlParameter, returnUrl.Uri.ToString()).Value
                        };
                        context.Response.Redirect(redirectUrl.ToString());
                        return Task.CompletedTask;
                    }
                };
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = "iSchoolAuth";
                options.Cookie.Domain = ".sxkid.com";
                options.Cookie.Path = "/";
                options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(filePath));
            });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardedHeaders = ForwardedHeaders.All;
            });
            #endregion
            services.AddMediatR(typeof(Startup));
            services.AddBehaviors();
        }

        //Autofac注入
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacExtension(Configuration.GetSection("ConnectionString").GetValue<string>("Slavers")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionless();
            if (Configuration.GetValue("USE_Forwarded_Headers", false))
            {
                app.UseForwardedHeaders();
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
#if DEBUG
                app.UseSwaggerUi3(p =>
                {
                    p.DocumentPath = "/swagger/{documentName}/swagger.json";//手动设置一下默认,开启explore功能。
                });
                app.UseOpenApi();
#else
                app.UseSwaggerUi3(p =>
                {
                    p.ServerUrl = "/school";
                    p.DocumentPath = "/school/swagger/{documentName}/swagger.json";
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
                            Url = $"/school"
                        }) ;
                    };
                });
#endif
            }

            //app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
        
            app.Use(next => context =>
            {
                context.Request.EnableBuffering();
                return next(context);
            });
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapMetrics();
                endpoints.MapHealthChecks("/live", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("live") });
                endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("ready") });
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                {
                    ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapControllers();
            });

        }

        public async Task ConfigureAppBaseInfo(IApplicationBuilder app)
        {
            var easyRedisClient = app.ApplicationServices.GetService<IEasyRedisClient>();
            string secret = Guid.NewGuid().ToString();
            await easyRedisClient.AddStringAsync(RedisCacheKeys.AppIDKey, secret);

        }
    }

}
