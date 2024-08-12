//using Autofac;
//using Exceptionless;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.HttpOverrides;
//using Microsoft.AspNetCore.HttpsPolicy;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Microsoft.OpenApi.Models;
//using Sxb.School.API.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Sxb.School.API
//{
//    public class Startup
//    {
//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }

//        public IConfiguration Configuration { get; }

//        // This method gets called by the runtime. Use this method to add services to the container.
//        public void ConfigureServices(IServiceCollection services)
//        {
//            services.AddControllers().AddNewtonsoftJson().AddXmlSerializerFormatters();
//            services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
//            });

//            //services.AddMediatRServices();
//            services.AddRedis(Configuration);
//            //services.AddMsSqlDomainContext(Configuration.GetSection("ConnectionString").GetValue<string>("Master"));
//            //services.AddEventBus(Configuration);
//            services.Configure<ForwardedHeadersOptions>(options =>
//            {
//                options.KnownNetworks.Clear();
//                options.KnownProxies.Clear();
//                options.ForwardedHeaders = ForwardedHeaders.All;
//            });
//        }

//        //Autofac×¢Èë
//        public void ConfigureContainer(ContainerBuilder builder)
//        {
//            builder.RegisterModule(new AutofacExtension(Configuration.GetSection("ConnectionString").GetValue<string>("Slavers")));
//        }

//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//        {
//            app.UseExceptionless();
//            if (Configuration.GetValue("USE_Forwarded_Headers", false))
//            {
//                app.UseForwardedHeaders();
//            }
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//                app.UseSwagger();
//                app.UseSwaggerUI(c =>
//                {
//                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
//                });
//            }

//            //app.UseHttpsRedirection();

//            app.UseRouting();

//            app.UseAuthorization();

//            app.UseEndpoints(endpoints =>
//            {
//                endpoints.MapControllers();
//            });
//        }
//    }
//}
