using System;
using Nancy;
using Nancy.ModelBinding;
using HttpStatusCode = Nancy.HttpStatusCode;

namespace PaymentProviders
{
    public class PaymentProviders : NancyModule
    {
        private readonly Random _random = new Random();

        public PaymentProviders()
        {
            Get["/"] = x => "Hello World";

            Post["/api/unreliable/processpayment/"] = parameters =>
            {
                var item = this.Bind<PaymentRequest>();

                if (_random.Next(3) == 0)
                    return HttpStatusCode.BadRequest;

                var response = new PaymentResponse
                {
                    CustomerId = item.CustomerId,
                    PaymentSucceeded = _random.Next(2) != 0
                };

                return Response.AsJson(response);
            };

            Post["/api/reliable/processpayment/"] = parameters =>
            {
                var item = this.Bind<PaymentRequest>();

                var response = new PaymentResponse
                {
                    CustomerId = item.CustomerId,
                    PaymentSucceeded = true
                };

                return Response.AsJson(response);
            };
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
