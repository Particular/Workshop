using Divergent.Customers.Data.Migrations;
using ITOps.EndpointConfig;
using NServiceBus;

const string EndpointName = "Divergent.Customers";

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        services.Configure<LiteDbOptions>(configuration.GetSection("LiteDbOptions"))
            .Configure<LiteDbOptions>(s =>
            {
                s.DatabaseName = "customers";
                s.DatabaseInitializer = DatabaseInitializer.Initialize;
            });
        services.AddSingleton<ILiteDbContext, LiteDbContext>();
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