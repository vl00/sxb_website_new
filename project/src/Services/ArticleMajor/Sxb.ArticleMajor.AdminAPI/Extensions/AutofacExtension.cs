using Autofac;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;
using System.Runtime.Loader;

namespace Sxb.ArticleMajor.AdminAPI.Extensions
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
                if (lib.Name.Contains("Sxb.ArticleMajor.AdminAPI"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                if (lib.Name.Contains("Sxb.ArticleMajor.Query.SQL"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                if (lib.Name.Contains("Sxb.ArticleMajor.Common"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                #endregion
            }
        }
    }
}
