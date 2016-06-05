using Autofac;
using Common.Logging;
using Divergent.Customers.Data.Repositories;

namespace Divergent.Customers.Config
{
    class ContainerSetup
    {
        private static readonly ILog Log = LogManager.GetLogger<ContainerSetup>();

        public static IContainer Create()
        {
			Log.Info("Initializing dependency injection...");

            var builder = new ContainerBuilder();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();

            return builder.Build();
        }
    }
}
