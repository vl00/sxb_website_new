using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Sxb.Recommend.Infrastructure;
using Sxb.Recommend.Infrastructure.IRepository;
using Sxb.Recommend.Infrastructure.Repository;
using Sxb.Recommend.Infrastructure.Repository.CSV;
using Sxb.Recommend.Infrastructure.Repository.MongoDB;
using Sxb.Recommend.Infrastructure.Repository.SQLServer;
using Sxb.Recommend.Infrastructure.Repository.SQLServer.DataBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtension
    {
        public static void TryAddISchoolDataDB(this IServiceCollection services)
        {
            services.TryAddTransient(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                var connectionString = configuration?.GetConnectionString("ISchoolDataDB");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException("找不到[ISchoolDataDB]的连接字符串，请尝试再配置文件中配置:ConnectionString:ISchoolDataDB。");
                }
                return new ISchoolDataDB() { Connection = new SqlConnection(connectionString) };
            });

        }

        public static void TryAddISchoolArticleDB(this IServiceCollection services)
        {
            services.TryAddTransient(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                var connectionString = configuration?.GetConnectionString("ISchoolArticleDB");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException("找不到[ISchoolArticleDB]的连接字符串，请尝试再配置文件中配置:ConnectionString:ISchoolArticleDB。");
                }
                return new ISchoolArticleDB() { Connection = new SqlConnection(connectionString) };
            });

        }


        public static void TryAddMongoDB(this IServiceCollection services)
        {

            services.TryAddScoped<IMongoClient>(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                var connectionString = configuration?.GetConnectionString("MongoDB");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException("找不到[MongoDB]的连接字符串，请尝试再配置文件中配置:ConnectionString:MongoDB。");
                }
                return new MongoClient(connectionString);
            });

        }

        public static void TryAddRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddISchoolDataDB();
            services.TryAddISchoolArticleDB();
            services.TryAddMongoDB();
            services.Configure<SchoolCSVOption>(configuration.GetSection(SchoolCSVOption.SchoolCSV));
            services.Configure<ArticleCSVOption>(configuration.GetSection(ArticleCSVOption.ArticleCSV));

            var interfaceTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
              .Where(t =>
                 t.IsPublic
                 &&
                 t.FullName.StartsWith("Sxb.Recommend.Infrastructure.IRepository", System.StringComparison.OrdinalIgnoreCase)
                 &&
                 t.IsInterface);

            var implementationTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
              .Where(t =>
                 t.IsPublic
                 &&
                 t.FullName.StartsWith("Sxb.Recommend.Infrastructure.Repository", System.StringComparison.OrdinalIgnoreCase)
                 &&
                 t.IsClass
                 &&
                 !t.IsAbstract)
              .ToList();
            foreach (var interfaceType in interfaceTypes)
            {
                foreach (var implementationType in implementationTypes)
                {
                    if (implementationType.GetInterfaces().Any(t => t == interfaceType))
                    {
                        services.TryAddScoped(interfaceType, implementationType);
                        break;
                    }
                }
            }

        }

    }
}
