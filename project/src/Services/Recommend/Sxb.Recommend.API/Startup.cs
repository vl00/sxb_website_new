using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sxb.Recommend.API.Middlewares;
using Sxb.Recommend.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Recommend.API
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
            services.AddMemoryCache(option => {
                option.SizeLimit = 1000000;//预估量学校加文章的数据条目有这么多。
            });

            services.TryAddRepository(Configuration);
            services.TryAddServices();
            services.TryAddMapFeatureComputeRules();
            services.AddDomainEvent();
            services.AddCors(option =>
                option.AddDefaultPolicy(policy =>
                    policy.SetIsOriginAllowed(origin => true)
                    .AllowAnyMethod().AllowAnyHeader().AllowCredentials()
            ));
            services.AddMvc().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseCustomExceptionCatch();
            }
            app.UseSxbRecommendMongoDB();

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseCors();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
