using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Divergent.ITOps;
using ITOps.EndpointConfig;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.ITOps
{
    class Host
    {
        static readonly ILog Log = LogManager.GetLogger<Host>();
        readonly string connectionString;
        IEndpointInstance endpoint;

        public static string EndpointName => "Divergent.ITOps";

        public Host(string connectionString) => this.connectionString = connectionString;

        public async Task Start()
        {
            try
            {
                var endpointConfiguration = new EndpointConfiguration(EndpointName)
                    .Configure(connectionString, null);

                var builder = new ContainerBuilder();
                var assemblies = ReflectionHelper.GetAssembliesPath("..\\..\\Providers", ".Data.dll");
                builder.RegisterAssemblyTypes(assemblies.ToArray())
                    .Where(t => t.Name.EndsWith("Provider"))
                    .AsImplementedInterfaces();

                endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(builder.Build()));

                endpoint = await Endpoint.Start(endpointConfiguration);
            }
            catch (Exception ex)
            {
                FailFast("Failed to start.", ex);
            }
        }

        public async Task Stop()
        {
            try
            {
                await endpoint?.Stop();
            }
            catch (Exception ex)
            {
                FailFast("Failed to stop correctly.", ex);
            }
        }

        async Task OnCriticalError(ICriticalErrorContext context)
        {
            try
            {
                await context.Stop();
            }
            finally
            {
                FailFast($"Critical error, shutting down: {context.Error}", context.Exception);
            }
        }

        void FailFast(string message, Exception exception)
        {
            try
            {
                Log.Fatal(message, exception);
            }
            finally
            {
                Environment.FailFast(message, exception);
            }
        }
    }
}
