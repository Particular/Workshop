using System.Configuration;
using Topshelf;

namespace Divergent.Sales.API.Host
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            HostFactory.Run(host =>
            {
                host.Service<SalesAPI>(service =>
                {
                    service.ConstructUsing(name => new SalesAPI());
                    service.WhenStarted(api => api.Start(ConfigurationManager.AppSettings["baseAddress"]));
                    service.WhenStopped(api => api.Stop());
                });

                host.RunAsLocalService();
                host.StartAutomatically();
                host.SetDescription("Services UI Composition demo: Sales API Host");
                host.SetDisplayName("Sales API Host");
                host.SetServiceName("SalesAPIHost");
            });
        }
    }
}
