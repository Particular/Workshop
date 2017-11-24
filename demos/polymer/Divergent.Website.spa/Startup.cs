using ITOps.ViewModelComposition;
using ITOps.ViewModelComposition.Gateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Divergent.Website.spa
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddViewModelComposition();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.RunCompositionGateway(c => c.MapComposableGet("compose/{controller}/{id:int?}"));

            app.Map("/compose", appBuilder =>
            {
                appBuilder.RunCompositionGatewayWithDefaultRoutes();
            });

            app.Map("", appBuilder =>
            {
                appBuilder.UseMvcWithDefaultRoute();
            });
        }
    }
}
