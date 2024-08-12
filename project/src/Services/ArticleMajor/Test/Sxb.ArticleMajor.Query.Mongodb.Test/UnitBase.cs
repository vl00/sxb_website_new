using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using ProductManagement.Framework.MongoDb;
using Sxb.GenerateNo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb.Test
{
    public class UnitBase
    {
        private IServiceProvider _serviceProvider;
        protected IServiceProvider ServiceProvider => _serviceProvider ?? GetServiceProvider();

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
            //Standard标准模式下, 数据库中显示是uuid串,
            //CSharpLegacy模式下, 显示是binary串
            //方法1.BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            //方法2.
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
            //V3模式下, 必须给Guid配置序列化方式(全局或者在具体类里指定)
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            
            serviceCollection.AddMongoDbAccessor(config =>
            {
                var dbs = new List<MongoDbConfig>() { new MongoDbConfig() {
                    ConfigName = "Mongo",
                    Database = "ischool",
                    ConnectionString = "mongodb://shenhao:19871016@132.232.125.113:27017/?authSource=ischool",
                    //Database = "ArticleMajor",
                    //ConnectionString = "mongodb://localhost:27020,localhost:27018/?replicaSet=rs0&readPreference=primary&ssl=false",
                    WriteCountersign = "majority"
                } };
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
        }

        public static void WriteLine<T>(T t)
        {
            var msg = JsonConvert.SerializeObject(t);
            WriteLine(msg);
        }

        public static void WriteLine<T>(IEnumerable<T> data)
        {
            //var msg = JsonConvert.SerializeObject(data);
            //WriteLine(msg)
            foreach (var item in data)
            {
                WriteLine(JsonConvert.SerializeObject(item));
            };
        }

        public static void WriteLine(string msg)
        {
            //Debug.WriteLine(msg);
            //Trace.WriteLine(msg);
            Console.WriteLine(msg);
        }
    }
}
