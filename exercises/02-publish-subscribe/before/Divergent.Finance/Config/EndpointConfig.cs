using System;
using System.Configuration;
using System.IO;
using ILog = Common.Logging.ILog;
using LogManager = Common.Logging.LogManager;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence;

namespace Divergent.Finance.Config
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
            endpointConfiguration.Recoverability().Delayed(c=>c.NumberOfRetries(0));
            endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));

            var routing = endpointConfiguration.UseTransport<MsmqTransport>()
                .ConnectionString("deadLetter=false;journal=false")
                .Routing();

            endpointConfiguration.UsePersistence<NHibernatePersistence>()
                .ConnectionString(ConfigurationManager.ConnectionStrings["Divergent.Finance"].ToString());

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Commands") && t.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") && t.Name.EndsWith("Event"));

            endpointConfiguration.EnableInstallers();
        }
    }
}
