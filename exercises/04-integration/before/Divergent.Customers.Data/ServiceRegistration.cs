using Divergent.Customers.Data.Migrations;
using Divergent.ITOps.Interfaces;
using ITOps.EndpointConfig;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Divergent.Customers.Data;

public class ServiceRegistration : IRegisterServices
{
    public void Register(HostBuilderContext context, IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        services.Configure<LiteDbOptions>(configuration.GetSection("LiteDbOptions"))
            .Configure<LiteDbOptions>(s =>
            {
                s.DatabaseName = "customers";
                s.DatabaseInitializer = DatabaseInitializer.Initialize;
            });
        services.AddSingleton<ILiteDbContext, LiteDbContext>();
    }
}