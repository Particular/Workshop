using Topshelf;

namespace Divergent.Shipping.API.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ServiceHost>(s =>
                {
                    s.ConstructUsing(name => new ServiceHost());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalService();
                x.StartAutomatically();

                x.SetDescription("Services UI Composition sample: Shipping API Host");
                x.SetDisplayName("Shipping API Host");
                x.SetServiceName("ShippingAPIHost");
            });
        }
    }
}
