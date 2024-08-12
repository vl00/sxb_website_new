using Microsoft.Extensions.DependencyInjection.Extensions;
using Sxb.ArticleMajor.Query.SQL;
using System.Data.SqlClient;

namespace Sxb.ArticleMajor.AdminAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerDataBase(this IServiceCollection services, IConfigurationSection connSection)
        {
            var connStr = connSection.GetValue<string>("Master");
            var slaveConnStr = connSection.GetValue<string>("Slavers");
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                services.TryAddScoped(sp =>
                {
                    return new ArticleMajorDB() { Connection = new SqlConnection(connStr), SlaveConnection = new SqlConnection(slaveConnStr) };
                });
            }
            return services;
        }
    }
}
