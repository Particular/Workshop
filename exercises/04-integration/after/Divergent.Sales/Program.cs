using Divergent.Sales.Data.Context;
using Divergent.Sales.Data.Migrations;
using ITOps.EndpointConfig;
using Microsoft.EntityFrameworkCore;
using NServiceBus;

const string EndpointName = "Divergent.Sales";

var host = Host.CreateDefaultBuilder((string[])args)
    .ConfigureServices((builder, services) =>
    {
        services.AddDbContext<SalesContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    })
    .UseNServiceBus(context =>
    {
        var endpoint = new EndpointConfiguration(EndpointName);
        endpoint.Configure();

        return endpoint;
    }).Build();

CreateDbIfNotExists(host);

var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

host.Run();

static void CreateDbIfNotExists(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<SalesContext>();
        DatabaseInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}