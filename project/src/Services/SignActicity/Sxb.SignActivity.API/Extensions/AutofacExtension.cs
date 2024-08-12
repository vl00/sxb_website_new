using Autofac;
using Microsoft.Extensions.DependencyModel;
using Sxb.SignActivity.Infrastructure.Repositories;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Sxb.SignActivity.API.Extensions
{
    public class AutofacExtension : Autofac.Module
    {
        public string QueriesConnectionString { get; }

        public AutofacExtension(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SignInRepository>().As<ISignInRepository>();
            builder.RegisterType<SignInHistoryRepository>().As<ISignInHistoryRepository>();
            builder.RegisterType<SignInParentHistoryRepository>().As<ISignInParentHistoryRepository>();
            builder.RegisterType<SignConfigRepository>().As<ISignConfigRepository>();
            builder.RegisterType<OrderRepository>().As<IOrderRepository>();



            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project");
            foreach (var lib in libs)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));

                #region DI
                if (lib.Name.Contains("Sxb.SignActivity.API"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                if (lib.Name.Contains("Sxb.SignActivity.Query.SQL"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                if (lib.Name.Contains("Sxb.SignActivity.Common"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                #endregion
            }
        }
    }
}
