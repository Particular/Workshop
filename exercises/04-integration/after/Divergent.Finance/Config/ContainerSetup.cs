using Autofac;
using Divergent.Finance.PaymentClient;
using NServiceBus.Logging;

namespace Divergent.Finance.Config
{
    class ContainerSetup
    {
        private static readonly ILog Log = LogManager.GetLogger<ContainerSetup>();

        public static IContainer Create()
        {
			Log.Info("Initializing dependency injection...");

            var builder = new ContainerBuilder();
            builder.RegisterType<ReliablePaymentClient>();

            return builder.Build();
        }
    }
}
