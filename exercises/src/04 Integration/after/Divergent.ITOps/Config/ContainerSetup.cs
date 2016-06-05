using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Common.Logging;

namespace Divergent.ITOps.Config
{
    class ContainerSetup
    {
        private static readonly ILog Log = LogManager.GetLogger<ContainerSetup>();

        public static IContainer Create(IEnumerable<Assembly> assemblies)
        {
            Log.Info("Initializing dependency injection...");

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .Where(t => t.Name.EndsWith("Provider"))
                .AsImplementedInterfaces();

            return builder.Build();
        }
    }
}
