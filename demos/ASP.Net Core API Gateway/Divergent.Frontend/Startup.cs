using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ITOps.ViewModelComposition.Gateway;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ITOps.ViewModelComposition;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ITOps.ViewModelComposition.Mvc;

namespace Divergent.Frontend
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddViewModelComposition();
            services
                .AddMvc()
                .AddViewModelCompositionMvcSupport();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            DefaultAppConfigure(app, env, loggerFactory);

            app.UseStaticFiles();

            app.Map("/compose", appBuilder =>
            {
                //via branching composition gateway can be hosted alongside anohter Mvc App.
                appBuilder.RunCompositionGatewayWithDefaultRoutes();
            });

            app.Map("", appBuilder =>
            {
                appBuilder.UseMvcWithDefaultRoute();
            });
        }

        void DefaultAppConfigure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
        }
    }
}
