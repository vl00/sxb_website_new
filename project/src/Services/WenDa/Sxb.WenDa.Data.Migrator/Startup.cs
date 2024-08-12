using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sxb.WenDa.Query.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Data.Migrator
{

    public class Startup
    {
        private IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider => _serviceProvider ?? GetServiceProvider();

        public IConfigurationRoot ConfigurationRoot => BuildConfiguration();

        public bool IsProduction()
        {
            var isProduction = Environment.GetEnvironmentVariable("Production");
            return isProduction != null && isProduction == "true";
        }

        IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json", optional: false);

            if (IsProduction())
            {
                builder.AddJsonFile("appsettings.Production.json", optional: false);
            }
            else
            {
                builder.AddJsonFile("appsettings.Development.json", optional: false);
            }
            return builder.Build();
        }

        public virtual IServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            ServiceBuilder(serviceCollection);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            ContainerBuilder(containerBuilder);

            var container = containerBuilder.Build();
            _serviceProvider = new AutofacServiceProvider(container);
            return _serviceProvider;
        }

        public virtual void ServiceBuilder(ServiceCollection services)
        {
            services.AddElasticSearch(ConfigurationRoot);
        }


        public virtual void ContainerBuilder(ContainerBuilder containerBuilder)
        {
        }
    }
}
