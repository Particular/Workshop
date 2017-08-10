using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

            var licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\License.xml");
            config.LicensePath(licensePath);

            var routing = config.UseTransport<MsmqTransport>()
                .ConnectionString("deadLetter=false;journal=false")
                .Routing();

            routing.RouteToEndpoint(typeof(SubmitOrderCommand), "Divergent.Sales");

            config.UseSerialization<JsonSerializer>();
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