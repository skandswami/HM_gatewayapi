using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ApiProvisioner.CLI.Extensions
{
        public static class ServiceCollectionExtensions
        {
            public static void RegisterAllTypes<T>(this IServiceCollection services, Assembly[] assemblies,
                ServiceLifetime lifetime = ServiceLifetime.Transient)
            {
                IEnumerable<TypeInfo> typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))));
                foreach (TypeInfo type in typesFromAssemblies)
                    services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
            }
        }
}

