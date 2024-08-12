using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sxb.PointsMall.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.FuntionTest
{
   public class AccountPointsTestStartup:Startup
    {
        IConfiguration Configuration;
        public AccountPointsTestStartup(IConfiguration configuration):base(configuration)
        {
            Configuration = configuration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddApplicationPart(typeof(Startup).Assembly);
            base.ConfigureServices(services);
        }
    }
}
