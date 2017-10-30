using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class Program
{
    static ILog log = LogManager.GetLogger<Program>();

    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Endpoint1";
        var endpointConfiguration = new EndpointConfiguration("Endpoint1");
        endpointConfiguration.UseTransport<MsmqTransport>();
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.SendOnly();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        Console.WriteLine("Press S to send a message");
        Console.WriteLine("Press E to send a message that will throw an exception");
        Console.WriteLine("Press any key to exit");

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key == ConsoleKey.S)
            {
                await SendMessage(endpointInstance);
                continue;
            }
            if (key.Key == ConsoleKey.E)
            {
                await SendMessage(endpointInstance, true);
                continue;
            }

            break;
        }

        Console.ReadKey();
        await endpointInstance.Stop().ConfigureAwait(false);
    }

    private static async Task SendMessage(IEndpointInstance endpointInstance, bool willThrow = false)
    {
        var sendOptions = new SendOptions();
        sendOptions.SetDestination("Endpoint2");

        var message = new TheMessage()
        {
            ThrowException = willThrow
        };

        await endpointInstance.Send(message, sendOptions).ConfigureAwait(false);

        Console.WriteLine($"Sent a message, will it throw : {willThrow}");
    }
}