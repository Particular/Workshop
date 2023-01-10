using ITOps.EndpointConfig;
using NServiceBus;

const string EndpointName = "Divergent.Shipping";

var host = Host.CreateDefaultBuilder(args)
    .UseNServiceBus(context =>
    {
        var endpoint = new EndpointConfiguration(EndpointName);
        endpoint.Configure();

        return endpoint;
    }).Build();

var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

host.Run();