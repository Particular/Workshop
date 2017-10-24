using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ITOps.ViewModelComposition
{
    public static class ServiceCollectionExtensions
    {
        public static void AddViewModelComposition(this IServiceCollection services) =>
            AddViewModelComposition(services, "*ViewModelComposition*.dll");

        public static void AddViewModelComposition(this IServiceCollection services, string assemblySearchPattern)
        {
            var types = new List<Type>();

            foreach (var assemblyPath in Directory.GetFiles(AppContext.BaseDirectory, assemblySearchPattern))
            {
                var assemblyTypes = AssemblyLoader.Load(assemblyPath)
                    .GetTypes()
                    .Where(type => !type.GetTypeInfo().IsAbstract && typeof(IRouteInterceptor).IsAssignableFrom(type));

                types.AddRange(assemblyTypes);
            }

            foreach (var type in types)
            {
                services.AddSingleton(typeof(IRouteInterceptor), type);
            }
        }
    }
}
