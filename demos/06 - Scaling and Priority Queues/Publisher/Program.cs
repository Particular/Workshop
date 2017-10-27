using System;
using System.Linq;
using System.Threading.Tasks;
using Messages.Events;
using NServiceBus;
using Shared;

static class Program
{
    static readonly int batchSize = 250;
    static Random rnd = new Random();
    static Guid[] customers = Customers.GetAllCustomers().ToArray();

    static async Task Main()
    {
        var endpointName = "ScalingDemo.Publisher";

        Console.Title = endpointName;
        var endpointConfiguration = new EndpointConfiguration(endpointName);
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
        transport.ConnectionString(@"server=.\sqlexpress;database=nservicebus;Trusted_Connection=True");

        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));
        conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        await Start(endpointInstance).ConfigureAwait(false);
        await endpointInstance.Stop().ConfigureAwait(false);
    }

    static async Task Start(IEndpointInstance endpointInstance)
    {
        Console.WriteLine("Press '1' to publish the OrderReceived event");
        Console.WriteLine($"Press '2' to publish {batchSize} OrderReceived events");
        Console.WriteLine("Press any other key to exit");

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            switch (key.Key)
            {
                case ConsoleKey.D1:
                    var orderReceived = CreateNewOrder();
                    await endpointInstance.Publish(orderReceived).ConfigureAwait(false);
                    Console.WriteLine($"Published OrderReceived Event with Id {orderReceived.OrderId} for customer {orderReceived.CustomerId}.");
                    break;
                case ConsoleKey.D2:
                    Console.WriteLine($"Publishing {batchSize} events");
                    Parallel.For(0, batchSize, i =>
                    {
                        endpointInstance.Publish(CreateNewOrder()).ConfigureAwait(false);
                    });
                    Console.WriteLine("Done...");
                    break;
                default:
                    return;
            }
        }
    }

    static OrderReceived CreateNewOrder()
    {
        return new OrderReceived
        {
            OrderId = Guid.NewGuid(),
            CustomerId = customers[rnd.Next(customers.Length)]
        };
    }
}