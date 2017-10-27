using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

static class Program
{
    static async Task Main(string[] args)
    {
        var endpointName = "ScalingDemo.StrategicCustomers";

        if (args.Length > 0)
            endpointName = $"{endpointName}_{args[0]}";

        Console.Title = endpointName;
        var endpointConfiguration = new EndpointConfiguration(endpointName);

        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
        transport.ConnectionString(@"server=.\sqlexpress;database=nservicebus;Trusted_Connection=True");

        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));
        conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));

        var routing = transport.Routing();
        routing.RegisterPublisher(typeof(Customers).Assembly, "ScalingDemo.Publisher");

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop().ConfigureAwait(false);
    }
}