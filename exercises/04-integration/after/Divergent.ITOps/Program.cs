using System.Reflection;
using Divergent.ITOps;
using Divergent.ITOps.Interfaces;
using ITOps.EndpointConfig;
using NServiceBus;

const string EndpointName = "Divergent.ITOps";

var host = Host.CreateDefaultBuilder((string[]) args)
    .ConfigureServices((builder, services) =>
    {
        var assemblies = ReflectionHelper.GetAssemblies(".Data.dll");
        
        // Find and register all types that end with 'Provider' so we can inject them into ShipWithFedexCommandHandler
        // Those types are included by adding a reference to
        //   - Divergent.Customers.Data
        //   - Divergent.Shipping.Data
        // Normally we deploy them together with Divergent.ITOps using our CI pipeline, but that's impossible
        //   for this workshop, where we need [F5] to work.
        services.Scan(s =>
        {
            s.FromAssemblies(assemblies)
                .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Provider")))
                .AsImplementedInterfaces()
                .WithTransientLifetime();
        });

        // This loads all IRegisterServices to make sure we can access the database for each provider
        var serviceRegistrars = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IRegisterServices).IsAssignableFrom(t))
            .Select(Activator.CreateInstance)
            .Cast<IRegisterServices>()
            .ToList();

        foreach (var serviceRegistrar in serviceRegistrars)
        {
            serviceRegistrar.Register(builder, services);
        }
    })
    .UseNServiceBus(context =>
    {
        var endpoint = new EndpointConfiguration(EndpointName);
        endpoint.Configure();

        return endpoint;
    }).Build();

var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

host.Run();