using System;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;

namespace Divergent.ITOps.Config
{
    [EndpointName("Divergent.ITOps")]
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
    {
        private static readonly ILog Log = LogManager.GetLogger<EndpointConfig>();

        public EndpointConfig()
        {
            NServiceBus.Logging.LogManager.Use<DefaultFactory>();

            if (Environment.UserInteractive)
                Console.Title = "Divergent.ITOps";
        }

        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            Log.Info("Customize...");

            var providerAssemblies = ReflectionHelper.GetAssembliesPath("..\\..\\Providers", ".Data.dll");
            var container = ContainerSetup.Create(providerAssemblies);

            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.DisableFeature<SecondLevelRetries>();
            endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));
            endpointConfiguration.UseTransport<MsmqTransport>();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            ConventionsBuilder conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Commands") && t.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") && t.Name.EndsWith("Event"));

            endpointConfiguration.EnableInstallers();
        }
    }
}
