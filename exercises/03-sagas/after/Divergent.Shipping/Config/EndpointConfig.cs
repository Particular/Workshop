using System;
using System.IO;
using System.Configuration;
using ILog = Common.Logging.ILog;
using LogManager = Common.Logging.LogManager;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence;
using Divergent.Finance.Messages.Events;
using Divergent.Sales.Messages.Events;

namespace Divergent.Shipping.Config
{
    public class EndpointConfig
    {
        private static readonly ILog Log = LogManager.GetLogger<EndpointConfig>();

        public static void Customize(EndpointConfiguration endpointConfiguration)
        {
            Log.Info("Customize...");

            NServiceBus.Logging.LogManager.Use<DefaultFactory>();

            var container = ContainerSetup.Create();

            var licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\..\\License.xml");
            endpointConfiguration.LicensePath(licensePath);
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));
            endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));

            var routing = endpointConfiguration.UseTransport<MsmqTransport>()
                .ConnectionString("deadLetter=false;journal=false")
                .Routing();

            routing.RegisterPublisher(typeof(PaymentSucceededEvent), "Divergent.Finance");
            routing.RegisterPublisher(typeof(OrderSubmittedEvent), "Divergent.Sales");

            endpointConfiguration.UsePersistence<NHibernatePersistence>()
                .ConnectionString(ConfigurationManager.ConnectionStrings["Divergent.Shipping"].ToString());

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Commands") && t.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") && t.Name.EndsWith("Event"));

            endpointConfiguration.EnableInstallers();
        }
    }
}
