using System;
using Castle.Windsor;
using NServiceBus;
using Castle.MicroKernel.Registration;
using System.IO;
using Divergent.Sales.Messages.Commands;

namespace Divergent.Sales.API
{
    public class NServiceBusConfig
    {
        internal static void Configure(IWindsorContainer container)
        {
            var config = new EndpointConfiguration("Sales.API");

            config.SendOnly();

            var licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\..\\..\\License.xml");
            config.LicensePath(licensePath);

            var transport = config.UseTransport<MsmqTransport>();
            transport.DisableDeadLetterQueueing();

            var routing = transport.Routing();

            routing.RouteToEndpoint(typeof(SubmitOrderCommand), "Divergent.Sales");

            config.UseSerialization<NewtonsoftSerializer>();
            config.UsePersistence<InMemoryPersistence>();

            config.SendFailedMessagesTo("error");

            config.Conventions()
                .DefiningCommandsAs(t => t.Namespace != null && t.Namespace == "Divergent.Messages" || t.Name.EndsWith("Command"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace == "Divergent.Messages" || t.Name.EndsWith("Event"));

            var endpoint = Endpoint.Start(config).GetAwaiter().GetResult();

            container.Register(Component.For<IEndpointInstance>()
                .Instance(endpoint)
                .LifestyleSingleton());
        }
    }
}