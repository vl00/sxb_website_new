using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Sxb.WenDa.Infrastructure;
using System.Reflection;

namespace Sxb.WenDa.Data.Migrator
{
    public class LocalDbContextFactory : IDesignTimeDbContextFactory<LocalDbContext>
    {
        public LocalDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();
            var connectionStr = configuration.GetSection("ConnectionString").GetValue<string>("Master");

            var builder = new DbContextOptionsBuilder<LocalDbContext>()
               .UseSqlServer(connectionStr, 
                b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName))
                //.UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion)
                ;
            return new LocalDbContext(builder.Options, null, null);
        }

        IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                //.AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: false)
                ;

            return builder.Build();
        }
    }
}