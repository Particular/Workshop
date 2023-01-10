using Divergent.Sales.Data.Migrations;
using ITOps.EndpointConfig;

namespace Divergent.Sales.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson();

        services.Configure<LiteDbOptions>(Configuration.GetSection("LiteDbOptions"))
            .Configure<LiteDbOptions>(s =>
            {
                s.DatabaseName = "sales";
                s.DatabaseInitializer = DatabaseInitializer.Initialize;
            });
        services.AddSingleton<ILiteDbContext, LiteDbContext>();

        services.AddCors();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors(policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin();
            policyBuilder.AllowAnyMethod();
            policyBuilder.AllowAnyHeader();
        });

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}