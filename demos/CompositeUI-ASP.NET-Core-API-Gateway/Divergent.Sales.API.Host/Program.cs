using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Divergent.Sales.API.Host
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

                x.SetDescription("Services UI Composition sample: Sales API Host");
                x.SetDisplayName("Sales API Host");
                x.SetServiceName("SalesAPIHost");
            });
        }
    }
}
