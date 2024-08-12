using Autofac;
using Microsoft.Extensions.DependencyModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Sxb.School.API.Extensions
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
            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project");
            foreach (var lib in libs)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));

                #region DI
                if (lib.Name.Contains("Sxb.School.API"))
                {
                   
                    builder.RegisterTypes(assembly.GetTypes().Where(t => 
                    t.Namespace?.StartsWith("Sxb.School.API.Application.Queries") == false
                    &&
                    t.Namespace?.StartsWith("Sxb.School.API.Application.DomainEventHandlers") == false
                    ).ToArray()).AsImplementedInterfaces();
                }
                if (lib.Name.Contains("Sxb.School.Query.SQL"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                if (lib.Name.Contains("Sxb.School.Common"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                #endregion
            }
        }
    }
}
