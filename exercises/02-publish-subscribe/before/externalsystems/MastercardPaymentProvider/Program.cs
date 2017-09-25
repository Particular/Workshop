using System;
using System.Net.Http;
using System.Threading.Tasks;
using Nancy;
using Nancy.Hosting.Self;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace PaymentProviders
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HostConfiguration {UrlReservations = {CreateAutomatically = true}};

            using (var host = new NancyHost(new Uri("http://localhost:1234"), new DefaultNancyBootstrapper(), config))
            {
                host.Start();
                Console.WriteLine("Running on http://localhost:1234");
                Console.WriteLine("[1] Test unreliable, but cheap payment provider.");
                Console.WriteLine("[2] Test reliable, but expensive payment provider.");

                while (true)
                {
                    var cki = Console.ReadKey(true);

                    switch (cki.Key)
                    {
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            Task.Run(async () => await TestImplementation("/api/unreliable/processpayment/")).Wait();
                            break;
                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            Task.Run(async () => await TestImplementation("/api/reliable/processpayment/")).Wait();
                            break;
                    }

                    if (cki.Key == ConsoleKey.Escape)
                        break;
                }
            }
        }

        private static async Task TestImplementation(string url)
        {
            HttpResponseMessage response = null;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://127.0.0.1:1234/");
                var msg = new PaymentRequest
                {
                    CustomerId = 12,
                    Amount = 300d
                };
                response = await httpClient.PostAsJsonAsync(url, msg);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"HttpStatusCode : {response.StatusCode.ToString()}");
                return;
            }

            var result = response.Content.ReadAsAsync<PaymentResponse>();
            Console.WriteLine($"Customer {result.Result.CustomerId} payment succesful? : {result.Result.PaymentSucceeded}");
        }
    }
}
