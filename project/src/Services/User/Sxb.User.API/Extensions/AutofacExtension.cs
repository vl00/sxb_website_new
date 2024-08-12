using Autofac;
using Microsoft.Extensions.DependencyModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Sxb.User.API.Extensions
{
    public class AutofacExtension : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //获取当前应用程序加载程序集
            //var assemblys = AppDomain.CurrentDomain.GetAssemblies().ToArray();
            //builder.RegisterAssemblyTypes(assembly)
            //    .InNamespaceOf<MyTalentQueries>()
            //    .InstancePerLifetimeScope();
            //var assemblys = AppDomain.CurrentDomain.GetAssemblies().ToArray();
            //var perRequestType = typeof(MyTalentQueries);
            //builder.RegisterAssemblyTypes(assemblys)
            //    .Where(t => perRequestType.IsAssignableFrom(t) && t != perRequestType)
            //    .PropertiesAutowired()
            //    .AsImplementedInterfaces()
            //    .InstancePerLifetimeScope();

            //builder.RegisterType<TalentRepository>().As<ITalentRepository>();

            //builder.RegisterType<MyTalentQueries>().As<IMyTalentQueries>();

            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project");
            foreach (var lib in libs)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));

                #region DI
                if (lib.Name.Contains("Sxb.User.API"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                if (lib.Name.Contains("Sxb.User.Query.SQL"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                #endregion
            }
        }
    }
}
