using Autofac;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;
using System;
using System.Data.SqlClient;
using System.IO;
using ILog = Common.Logging.ILog;
using LogManager = Common.Logging.LogManager;

namespace ITOps.EndpointConfig
{
    public static class EndpointConfigurationExtensions
    {
        static readonly ILog Log = LogManager.GetLogger(typeof(EndpointConfigurationExtensions));

        public static EndpointConfiguration Configure(
            this EndpointConfiguration endpointConfiguration,
            string connectionString,
            Action<RoutingSettings<MsmqTransport>> configureRouting)
        {
            NServiceBus.Logging.LogManager.Use<DefaultFactory>();

            Log.Info("Configuring endpoint...");

            var licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\..\\License.xml");
            endpointConfiguration.LicensePath(licensePath);
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));

            var routing = endpointConfiguration.UseTransport<MsmqTransport>()
                .ConnectionString("deadLetter=false;journal=false")
                .Routing();

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(() => new SqlConnection(connectionString));
            persistence.SubscriptionSettings().CacheFor(TimeSpan.FromMinutes(1));

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Commands") && t.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") && t.Name.EndsWith("Event"));

            endpointConfiguration.EnableInstallers();

            configureRouting?.Invoke(routing);

            return endpointConfiguration;
        }
    }
}
