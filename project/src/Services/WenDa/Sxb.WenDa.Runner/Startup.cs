using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sxb.WenDa.Common.Data;
using Sxb.WenDa.Query.SQL;
using System.Data.SqlClient;

namespace Sxb.WenDa.Runner
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
            services.AddHttpClient()
            ;
            AddSqlServerDataBase(services, ConfigurationRoot.GetSection("ConnectionString"));
            //services.AddSingleton<UploadService>();
            //services.AddSingleton(ConfigurationRoot.Get<AppSettingsData>());
        }


        public virtual void ContainerBuilder(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<AutofacExtension>();

            //containerBuilder.RegisterAssemblyTypes(typeof(RepositoryBase<>).Assembly).AsImplementedInterfaces();

            var types = typeof(Program).Assembly
                .GetTypes()
                .Where(s => s.BaseType?.Name?.StartsWith("BaseRunner") == true)
                .ToArray();
            containerBuilder.RegisterTypes(types);
        }

        public IServiceCollection AddSqlServerDataBase(IServiceCollection services, IConfigurationSection connSection)
        {
            var connStr = connSection.GetValue<string>("Master");
            var slaveConnStr = connSection.GetValue<string>("Slavers");
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                services.TryAddScoped(sp =>
                {
                    return new LocalQueryDB() { Connection = new SqlConnection(connStr), SlaveConnection = new SqlConnection(slaveConnStr) };
                });
            }
            return services;
        }
    }
}
