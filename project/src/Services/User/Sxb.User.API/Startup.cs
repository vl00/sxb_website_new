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
using Prometheus;
using Serilog;
using Sxb.Framework.AspNetCoreHelper.Filters;
using Sxb.Framework.AspNetCoreHelper.Middleware;
using Sxb.Framework.AspNetCoreHelper.Utils;
using Sxb.User.API.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sxb.User.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        internal static bool Ready { get; set; } = true;
        internal static bool Live { get; set; } = true;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddGrpc();
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidateModelResultFilter>();
            });
            services.AddRedis(Configuration);
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

            services.AddControllers().AddNewtonsoftJson();//支持构造函数序列化
            services.AddSwaggerDocument();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
                //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
            services.AddMediatRServices();
            services.AddMsSqlDomainContext(Configuration.GetSection("ConnectionString").GetValue<string>("Master"));
            services.AddSqlServerDataBase(Configuration.GetSection("ConnectionString"));
            services.AddEventBus(Configuration);
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardedHeaders = ForwardedHeaders.All;
            });


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

        }

        // Autofac引用
        public void ConfigureContainer(ContainerBuilder builder) => builder.RegisterModule(new AutofacExtension());

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                    p.ServerUrl = "/user";
                    p.DocumentPath = "/user/swagger/{documentName}/swagger.json";
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
                            Url = $"/user"
                        }) ;
                    };
                });
#endif
            }
            else
            {
                app.UseStatusCodeMiddleware();
                app.UseExceptionMiddleware();
            }

            //app.UseHttpsRedirection();
            //app.UseSerilogRequestLogging(opts
            //    => opts.EnrichDiagnosticContext = EnrichFromRequest);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapHealthChecks("/live", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("live") });
                endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("ready") });
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                {
                    ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapControllers();
            });
        }

        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;

            // Set all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is object) // endpoint != null
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }
    }
}
