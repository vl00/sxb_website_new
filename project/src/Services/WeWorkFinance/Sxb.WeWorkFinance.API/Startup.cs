using Autofac;
using Exceptionless;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Prometheus;
using Sxb.WeWorkFinance.API.Application.HttpClients;
using Sxb.WeWorkFinance.API.Config;
using Sxb.WeWorkFinance.API.Extensions;
using Sxb.WeWorkFinance.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sxb.Framework.SearchAccessor;

namespace Sxb.WeWorkFinance.API
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
            services.AddGrpc();

            //services.AddHealthChecks()
            //    .AddSqlServer(Configuration.GetSection("ConnectionString").GetValue<string>("Master"), "select 1;", tags: new string[] { "mssql", "live", "all" })
            //    .AddRabbitMQ(s =>
            //    {
            //        var connectionFactory = new RabbitMQ.Client.ConnectionFactory();
            //        Configuration.GetSection("RabbitMQ").Bind(connectionFactory);
            //        return connectionFactory;
            //    }, "rabbitmq", tags: new string[] { "rabbitmq", "live", "all" })
            //    .AddCheck("liveChecker", () =>
            //    {
            //        return Live ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
            //    }, new string[] { "live", "all" })
            //    .AddCheck("readyChecker", () =>
            //    {
            //        return Ready ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
            //    }, new string[] { "ready", "all" });


            services.Configure<WorkWeixinConfig>(Configuration.GetSection("WorkWeixinConfig"));
            services.Configure<InviteActivityConfig>(Configuration.GetSection("InviteActivityConfig"));
            
            services.AddHttpClient<OrgServerClient>(c=> {
                string url = this.Configuration.GetSection("OrgClientConfig").GetValue<string>("ServerUrl");
                c.BaseAddress = new Uri(url);
            });
            services.AddHttpClient<WxServerClient>(c => {
                string url = this.Configuration.GetSection("WxClientConfig").GetValue<string>("ServerUrl");
                c.BaseAddress = new Uri(url);
            });
            services.AddHttpClient<MarketingServerClient>(c => {
                string url = this.Configuration.GetSection("MarketingClientConfig").GetValue<string>("ServerUrl");
                c.BaseAddress = new Uri(url);
            });
            services.AddHttpClient<BgDataServerClient>(c => {
                string url = this.Configuration.GetSection("BgDataClientConfig").GetValue<string>("ServerUrl");
                c.BaseAddress = new Uri(url);
            });

            //elasticsearch
            services.AddScopedSearchAccessor(config =>
            {
                var searchConfig = Configuration.GetSection("SearchConfig").Get<SearchConfig>();
                config.ServerUrl = searchConfig.ServerUrl;
                config.DefultIndexName = searchConfig.DefultIndexName;
            });



            services.AddControllers().AddNewtonsoftJson().AddXmlSerializerFormatters();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
                //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
            services.AddEasyWeChat();
            services.AddMediatRServices();
            services.AddRedis(Configuration);
            services.AddMsSqlDomainContext(Configuration.GetSection("ConnectionString").GetValue<string>("Master"));
            services.AddEventBus(Configuration);
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardedHeaders = ForwardedHeaders.All;
            });


        }

        // Autofac����
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacExtension(Configuration));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionless();

            //自动创建数据库和表
            //using (var scope = app.ApplicationServices.CreateScope())
            //{
            //    var dc = scope.ServiceProvider.GetService<UserContext>();
            //    //dc.Database.EnsureDeleted();
            //    //dc.Database.EnsureCreated();
            //    //dc.Database.Migrate();
            //    RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)dc.Database.GetService<IDatabaseCreator>();
            //    databaseCreator.CreateTables();
            //}
            //
            if (Configuration.GetValue("USE_Forwarded_Headers", false))
            {
                app.UseForwardedHeaders();
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            else
            {
#if DEBUG
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
#endif
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                //endpoints.MapHealthChecks("/live", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("live") });
                //endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("ready") });
                //endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                //{
                //    ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
                //});
                endpoints.MapControllers();
            });
        }
    }
}
