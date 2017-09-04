using Autofac;
using Common.Logging;

namespace Divergent.Sales.Config
{
    class ContainerSetup
    {
        private static readonly ILog Log = LogManager.GetLogger<ContainerSetup>();

        public static IContainer Create()
        {
			Log.Info("Initializing dependency injection...");

            var builder = new ContainerBuilder();

            return builder.Build();
        }
    }
}
