using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sxb.Comment.Common.DB;
using System.Data.SqlClient;

namespace Sxb.Comment.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerDataBase(this IServiceCollection services, IConfigurationSection connSection)
        {
            var connStr = connSection.GetValue<string>("Master");
            var slaveConnStr = connSection.GetValue<string>("Slavers");
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                services.AddTransient(sp =>
                {
                    return new SchoolProductDB() { Connection = new SqlConnection(connStr), SlaveConnection = new SqlConnection(slaveConnStr) };
                });
            }
            return services;
        }
        public static IServiceCollection AddHttpClientBase(this IServiceCollection services, IConfigurationSection connSection)
        {
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("UserAddress")))
            {
                services.AddHttpClient("UserAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("UserAddress")));
            }
            if (!string.IsNullOrWhiteSpace(connSection.GetValue<string>("SchoolAddress")))
            {
                services.AddHttpClient("SchoolAPI", p => p.BaseAddress = new System.Uri(connSection.GetValue<string>("SchoolAddress")));
            }
            return services;
        }
    }
}
