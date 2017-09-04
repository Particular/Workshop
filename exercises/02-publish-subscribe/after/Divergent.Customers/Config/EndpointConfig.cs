using System;
using System.Configuration;
using System.IO;
using Divergent.Sales.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence;
using ILog = Common.Logging.ILog;
using LogManager = Common.Logging.LogManager;

namespace Divergent.Customers.Config
{
    [EndpointName("Divergent.Customers")]
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
    {
        private static readonly ILog Log = LogManager.GetLogger<EndpointConfig>();

        public EndpointConfig()
        {
            NServiceBus.Logging.LogManager.Use<DefaultFactory>();

            if (Environment.UserInteractive)
                Console.Title = "Divergent.Customers";
        }

        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            Log.Info("Customize...");

            var container = ContainerSetup.Create();

            var licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\..\\License.xml");
            endpointConfiguration.LicensePath(licensePath);
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.Recoverability().Delayed(c=>c.NumberOfRetries(0));
            endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));

            var routing = endpointConfiguration.UseTransport<MsmqTransport>()
                .ConnectionString("deadLetter=false;journal=false")
                .Routing();

            routing.RegisterPublisher(typeof(OrderSubmittedEvent), "Divergent.Sales");

            endpointConfiguration.UsePersistence<NHibernatePersistence>()
                .ConnectionString(ConfigurationManager.ConnectionStrings["Divergent.Customers"].ToString());

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Commands") && t.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") && t.Name.EndsWith("Event"));

            endpointConfiguration.EnableInstallers();
        }
    }
}
