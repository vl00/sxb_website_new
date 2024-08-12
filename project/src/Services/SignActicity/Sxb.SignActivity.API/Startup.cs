using Autofac;
using Exceptionless;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sxb.Article.API.Extensions;
using Sxb.Framework.AspNetCoreHelper.Filters;
using Sxb.Framework.AspNetCoreHelper.Middleware;
using Sxb.SignActivity.API.Extensions;
using Sxb.SignActivity.Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.SignActivity.API
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
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidateModelResultFilter>();
            });
            services.AddControllers();
            services.AddSwaggerDocument();

            services.Configure<WeChatConfig>(Configuration.GetSection("WeChatConfig"));
            //services.AddSingleton(Configuration.GetSection("WeChatConfig").Get<WeChatConfig>());

            services.AddEasyWeChat();
            services.AddRedis(Configuration);
            services.AddMediatRServices();
            services.AddMsSqlDomainContext(Configuration.GetSection("ConnectionString").GetValue<string>("Master"));
            services.AddSqlServerDataBase(Configuration.GetSection("ConnectionString"));
            services.AddEventBus(Configuration);

            services.AddHttpClientBase(Configuration.GetSection("ExternalInterface"));
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacExtension(Configuration.GetSection("ConnectionString").GetValue<string>("Master")));
        }

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

                app.UseStatusCodeMiddleware();
                //app.UseExceptionMiddleware();
            }
            else
            {
                app.UseStatusCodeMiddleware();
                app.UseExceptionMiddleware();
            }
            app.UseOpenApi();
            app.UseSwaggerUi3();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
