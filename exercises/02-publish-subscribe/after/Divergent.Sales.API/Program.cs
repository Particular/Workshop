using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Radical.Bootstrapper;

namespace Divergent.Sales.API
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = MethodBase.GetCurrentMethod().DeclaringType.Namespace;

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) => { tcs.SetResult(null); };

            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            var bootstrapper = new WindsorBootstrapper(basePath, filter: "Divergent*.*");
            var container = bootstrapper.Boot();

            NServiceBusConfig.Configure(container);

            using (WebApp.Start(new StartOptions("http://localhost:20185"), builder => WebApiConfig.Configure(builder, container)))
            {
                await Console.Out.WriteLineAsync("Web server is running.");
                await Console.Out.WriteLineAsync("Press Ctrl+C to exit...");

                await tcs.Task;
            }
        }
    }
}
