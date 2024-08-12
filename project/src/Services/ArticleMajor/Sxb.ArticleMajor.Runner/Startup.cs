using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using ProductManagement.Framework.MongoDb;
using Sxb.ArticleMajor.Query.Mongodb;
using Sxb.ArticleMajor.Runner.DynamicImage;
using Sxb.GenerateNo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Runner
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

        public virtual void ServiceBuilder(ServiceCollection serviceCollection)
        {
            AddMongodb(serviceCollection);

            serviceCollection.AddHttpClient();
            serviceCollection.AddSingleton<UploadService>();
            serviceCollection.AddSingleton(ConfigurationRoot.Get<AppSettingsData>());
        }

        private void AddMongodb(ServiceCollection serviceCollection)
        {
            //Standard标准模式下, 数据库中显示是uuid串,
            //CSharpLegacy模式下, 显示是binary串
            //方法1.BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            //方法2.
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
            //V3模式下, 必须给Guid配置序列化方式(全局或者在具体类里指定)
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            serviceCollection.AddMongoDbAccessor(config =>
            {
                var dbs = ConfigurationRoot.GetSection("MongodbConfig").Get<List<MongoDbConfig>>();

                //var dbs = new List<MongoDbConfig>() { new MongoDbConfig() {
                //    ConfigName = "Mongo",
                //    Database = "ischool",
                //    ConnectionString = "mongodb://shenhao:19871016@132.232.125.113:27017/?authSource=ischool",
                //    //Database = "ArticleMajor",
                //    //ConnectionString = "mongodb://localhost:27020,localhost:27018/?replicaSet=rs0&readPreference=primary&ssl=false",
                //    WriteCountersign = "majority"
                //} };
                config.ConfigName = dbs[0].ConfigName;
                config.Database = dbs[0].Database;
                config.ConnectionString = dbs[0].ConnectionString;
                config.WriteCountersign = dbs[0].WriteCountersign;
            });
        }

        public virtual void ContainerBuilder(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SxbGenerateNo>().As<ISxbGenerateNo>();
            containerBuilder.RegisterAssemblyTypes(typeof(RepositoryBase<>).Assembly).AsImplementedInterfaces();

            var types = typeof(Program).Assembly
                .GetTypes()
                .Where(s => s.BaseType?.Name?.StartsWith("BaseRunner") == true)
                .ToArray();
            containerBuilder.RegisterTypes(types);
        }
    }
}
