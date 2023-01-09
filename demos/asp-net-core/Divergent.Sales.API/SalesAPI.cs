using Microsoft.Owin.Hosting;
using System;

namespace Divergent.Sales.API.Host
{
    internal class SalesAPI
    {
        IDisposable webApp;

        public void Start(string baseAddress)
        {
            webApp = WebApp.Start<Startup>(baseAddress);
            Console.WriteLine($"{this.GetType().Namespace} listening on {baseAddress}");
        }

        public void Stop() => webApp?.Dispose();
    }
}
