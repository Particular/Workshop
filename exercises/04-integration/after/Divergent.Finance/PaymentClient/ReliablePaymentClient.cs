using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.Finance.PaymentClient
{
    public class ReliablePaymentClient
    {
        public async Task<bool> ProcessPayment(int customerId, double amount)
        {
            HttpResponseMessage response;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:1234/");
                var url = "/api/reliable/processpayment/";
                var msg = new PaymentRequest
                {
                    CustomerId = customerId,
                    Amount = amount
                };
                response = await httpClient.PostAsJsonAsync(url, msg);

            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // Business decision, should we throw and retry (using NServiceBus FLR & SLR) or try something else?
                throw new WebException("Could not access PaymentProviders");
            }

            var result = response.Content.ReadAsAsync<PaymentResponse>();
            return result.Result.PaymentSucceeded;
        }
    }

    public class PaymentRequest
    {
        public int CustomerId { get; set; }
        public double Amount { get; set; }
    }

    public class PaymentResponse
    {
        public int CustomerId { get; set; }
        public bool PaymentSucceeded { get; set; }
    }
}