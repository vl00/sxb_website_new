using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sxb.Recommend.Application.Services;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtension
    {
        public static void TryAddServices(this IServiceCollection services)
        {

            var interfaceTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
              .Where(t =>
                 t.IsPublic
                 &&
                 t.FullName.StartsWith("Sxb.Recommend.Application.Services", System.StringComparison.OrdinalIgnoreCase)
                 &&
                 t.IsInterface)
              .ToList();

            var implementationTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
              .Where(t =>
                 t.IsPublic
                 &&
                 t.FullName.StartsWith("Sxb.Recommend.Application.Services", System.StringComparison.OrdinalIgnoreCase)
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

        public static void TryAddMapFeatureComputeRules(this IServiceCollection services)
        {
            var computeRuleTypes = Assembly.GetExecutingAssembly()
                 .GetTypes()
                 .Where(t =>
                    t.FullName.StartsWith("Sxb.Recommend.Application.MapFeatureComputeRules", System.StringComparison.OrdinalIgnoreCase)
                    &&
                    t.IsClass
                    &&
                    t.FullName.EndsWith("ComputeRule", System.StringComparison.OrdinalIgnoreCase)

                 )
                 .ToList();
            foreach (var computeRuleType in computeRuleTypes)
            {
                services.TryAddScoped(computeRuleType);
            }

        }

        public static void AddDomainEvent(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

        }
    }
}
