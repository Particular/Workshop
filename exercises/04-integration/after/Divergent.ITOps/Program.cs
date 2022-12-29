using Divergent.ITOps;
using Divergent.ITOps.Interfaces;
using ITOps.EndpointConfig;
using NServiceBus;

const string EndpointName = "Divergent.ITOps";

var host = Host.CreateDefaultBuilder((string[]) args)
    .ConfigureServices((builder, services) =>
    {
        var assemblies = ReflectionHelper.GetAssemblies("..\\..\\..\\Providers", ".Data.dll");
        services.Scan(s =>
        {
            s.FromAssemblies(assemblies)
                .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Provider")))
                .AsImplementedInterfaces()
                .WithTransientLifetime();
        });

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