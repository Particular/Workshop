using Carter;

namespace PaymentProviders;

public class PaymentProviders : ICarterModule
{
    private readonly Random _random = new();

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => "Hello World");

        app.MapPost("/api/unreliable/processpayment/", (PaymentRequest item) =>
        {
            if (_random.Next(3) == 0)
                return Results.BadRequest();

            var response = new PaymentResponse
            {
                CustomerId = item.CustomerId,
                PaymentSucceeded = _random.Next(2) != 0
            };

            return Results.Ok(response);
        });

        app.MapPost("/api/reliable/processpayment/", (PaymentRequest item) =>
        {
            var response = new PaymentResponse
            {
                CustomerId = item.CustomerId,
                PaymentSucceeded = true
            };

            return response;
        });
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