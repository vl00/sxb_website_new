using Autofac;
using Microsoft.Extensions.DependencyModel;
using Sxb.WenDa.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Sxb.WenDa.API.Extensions
{
    public class AutofacExtension : Autofac.Module
    {

        public AutofacExtension()
        {

        }
        protected override void Load(ContainerBuilder builder)
        {
            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project");
            foreach (var lib in libs)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));

                #region DI
                if (lib.Name.Contains("Sxb.WenDa.API"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                if (lib.Name.Contains("Sxb.WenDa.Query.SQL"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                if (lib.Name.Contains("Sxb.WenDa.Common"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                #endregion
            }


            //AddRepository
            builder.RegisterTypes(InfrastructureExtension.RegisterTypes().ToArray()).AsImplementedInterfaces();
        }
    }
}
