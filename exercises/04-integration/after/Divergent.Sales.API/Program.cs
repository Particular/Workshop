using Divergent.Sales.API;
using Divergent.Sales.Messages.Commands;
using NServiceBus;

var host = Host.CreateDefaultBuilder(args)
    .UseNServiceBus(_ =>
    {
        var config = new EndpointConfiguration("Sales.API");

        config.SendOnly();

        var transport = config.UseTransport<LearningTransport>();

        var routing = transport.Routing();

        routing.RouteToEndpoint(typeof(SubmitOrderCommand), "Divergent.Sales");

        config.UseSerialization<NewtonsoftJsonSerializer>();
        config.UsePersistence<LearningPersistence>();

        config.SendFailedMessagesTo("error");

        config.Conventions()
            .DefiningCommandsAs(t => t.Namespace != null && t.Namespace == "Divergent.Messages" || t.Name.EndsWith("Command"))
            .DefiningEventsAs(t => t.Namespace != null && t.Namespace == "Divergent.Messages" || t.Name.EndsWith("Event"));

        return config;
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    }).Build();

var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

host.Run();