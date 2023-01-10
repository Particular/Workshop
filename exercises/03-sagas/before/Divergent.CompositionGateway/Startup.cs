using ITOps.ViewModelComposition;
using ITOps.ViewModelComposition.Gateway;

namespace Divergent.CompositionGateway;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
        services.AddViewModelComposition();
        services.AddCors();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseCors(policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin();
            policyBuilder.AllowAnyMethod();
            policyBuilder.AllowAnyHeader();
        });

        app.RunCompositionGatewayWithDefaultRoutes();
    }
}