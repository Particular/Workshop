using System.Xml.Linq;
using Divergent.ITOps.Interfaces;
using Divergent.ITOps.Messages.Commands;
using NServiceBus;

namespace Divergent.ITOps.Handlers;

public class ShipWithFedexCommandHandler : IHandleMessages<ShipWithFedexCommand>
{
    private readonly IProvideShippingInfo _shippingProvider;
    private readonly IProvideCustomerInfo _customerProvider;
    private readonly ILogger<ShipWithFedexCommandHandler> _logger;

    public ShipWithFedexCommandHandler(IProvideShippingInfo shippingProvider, IProvideCustomerInfo customerProvider, ILogger<ShipWithFedexCommandHandler> logger)
    {
        _shippingProvider = shippingProvider;
        _customerProvider = customerProvider;
        _logger = logger;
    }

    public async Task Handle(ShipWithFedexCommand message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Handle ShipWithFedexCommand");

        var shippingInfo = _shippingProvider.GetPackageInfo(message.Products);
        var customerInfo = _customerProvider.GetCustomerInfo(message.CustomerId);

        var fedExRequest = CreateFedexRequest(shippingInfo, customerInfo);
        await CallFedexWebService(fedExRequest);
        _logger.LogInformation("Order {MessageOrderId} shipped with Fedex", message.OrderId);
    }

    private XDocument CreateFedexRequest(PackageInfo packageInfo, CustomerInfo customerInfo)
    {
        var shipment =
            new XDocument(
                new XElement("FedExShipment",
                    new XElement("ShipTo",
                        new XElement("Name", customerInfo.Name),
                        new XElement("Street", customerInfo.Street),
                        new XElement("City", customerInfo.City),
                        new XElement("PostalCode", customerInfo.PostalCode),
                        new XElement("Country", customerInfo.Country)),
                    new XElement("Measurements",
                        new XElement("Volume", packageInfo.Volume),
                        new XElement("Weight", packageInfo.Weight))));
        return shipment;
    }

    private Task CallFedexWebService(XDocument fedExRequest)
    {
        //do web service call etc.
        return Task.CompletedTask;
    }
}