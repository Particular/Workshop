using System.Configuration;
using Topshelf;

namespace Divergent.Shipping.API.Host
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            HostFactory.Run(host =>
            {
                host.Service<ShippingAPI>(service =>
                {
                    service.ConstructUsing(name => new ShippingAPI());
                    service.WhenStarted(api => api.Start(ConfigurationManager.AppSettings["baseAddress"]));
                    service.WhenStopped(api => api.Stop());
                });

                host.RunAsLocalService();
                host.StartAutomatically();
                host.SetDescription("Services UI Composition demo: Shipping API Host");
                host.SetDisplayName("Shipping API Host");
                host.SetServiceName("ShippingAPIHost");
            });
        }
    }
}
