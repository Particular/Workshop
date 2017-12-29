using System;
using System.Configuration;
using System.IO;
using System.Linq;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence;
using ILog = Common.Logging.ILog;
using LogManager = Common.Logging.LogManager;
using Divergent.Sales.Data.Context;

namespace Divergent.Sales.Config
{
    public class EndpointConfig
    {
        private static readonly ILog Log = LogManager.GetLogger<EndpointConfig>();

        private static void InitializeDatbase()
        {
            Log.Debug("Initializing database");

            var context = new SalesContext();
            var products = context.Products.ToList();

            Log.DebugFormat("Database initialized, first product is {0}", products.First());
        }

        public static void Customize(EndpointConfiguration endpointConfiguration)
        {
            Log.Info("Customize...");

            NServiceBus.Logging.LogManager.Use<DefaultFactory>();

            InitializeDatbase();

            var container = ContainerSetup.Create();

            var licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\..\\License.xml");
            endpointConfiguration.LicensePath(licensePath);
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));
            endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));
            endpointConfiguration.UseTransport<MsmqTransport>()
                .ConnectionString("deadLetter=false;journal=false");
            endpointConfiguration.UsePersistence<NHibernatePersistence>()
                .ConnectionString(ConfigurationManager.ConnectionStrings["Divergent.Sales"].ToString());

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Commands") && t.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") && t.Name.EndsWith("Event"));

            endpointConfiguration.EnableInstallers();
        }
    }
}
