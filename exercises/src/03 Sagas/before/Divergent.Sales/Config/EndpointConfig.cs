using System;
using System.Linq;
using Divergent.Sales.Data.Context;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;
using ILog = Common.Logging.ILog;
using LogManager = Common.Logging.LogManager;
using System.IO;

namespace Divergent.Sales.Config
{
    [EndpointName("Divergent.Sales")]
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
    {
        private static readonly ILog Log = LogManager.GetLogger<EndpointConfig>();

        public EndpointConfig()
        {
            NServiceBus.Logging.LogManager.Use<DefaultFactory>();

            if (Environment.UserInteractive)
                Console.Title = "Divergent.Sales";
        }

        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            Log.Info("Customize...");

            var container = ContainerSetup.Create();

            var licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "License.xml");
            endpointConfiguration.LicensePath(licensePath);
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.Recoverability().Delayed(c=>c.NumberOfRetries(0));
            endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));
            endpointConfiguration.UseTransport<MsmqTransport>()
                .ConnectionString("deadLetter=false;journal=false");
            //endpointConfiguration.UsePersistence<NHibernatePersistence>()
            //    .ConnectionString(ConfigurationManager.ConnectionStrings["Divergent.Sales"].ToString());
            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            //this is required to avoid SQLite files locking if there are more than 1 message in the input queue
            endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            ConventionsBuilder conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Commands") && t.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") && t.Name.EndsWith("Event"));

            endpointConfiguration.EnableInstallers();
        }
    }
}
