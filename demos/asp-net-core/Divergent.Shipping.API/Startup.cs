using ITOps.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Divergent.Shipping.API
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddCors(options => { options.AddPolicy("AllowAllOrigins", builder => { builder.AllowAnyOrigin(); }); });
            services.AddControllers();

            var db = new LiteDbContext(new LiteDbOptions()
            {
                DatabaseName = "Shipping",
                DatabaseInitializer = Data.Migrations.DatabaseInitializer.Initialize
            });

            services.AddSingleton<ILiteDbContext>(db);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseRouting();
            app.UseCors("AllowAllOrigins");
            app.UseEndpoints(builder => builder.MapControllers());
        }
    }
}
