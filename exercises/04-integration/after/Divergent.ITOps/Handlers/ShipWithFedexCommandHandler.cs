using System.Threading.Tasks;
using System.Xml.Linq;
using Divergent.ITOps.Interfaces;
using Divergent.ITOps.Messages.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.ITOps.Handlers
{
    public class ShipWithFedexCommandHandler : IHandleMessages<ShipWithFedexCommand>
    {
        private readonly IProvideShippingInfo _shippingProvider;
        private readonly IProvideCustomerInfo _customerProvider;
        private static readonly ILog Log = LogManager.GetLogger<ShipWithFedexCommandHandler>();

        public ShipWithFedexCommandHandler(IProvideShippingInfo shippingProvider, IProvideCustomerInfo customerProvider)
        {
            _shippingProvider = shippingProvider;
            _customerProvider = customerProvider;
        }

        public async Task Handle(ShipWithFedexCommand message, IMessageHandlerContext context)
        {
            Log.Info("Handle ShipWithFedexCommand");

            var shippingInfo = await _shippingProvider.GetPackageInfo(message.Products);
            var customerInfo = await _customerProvider.GetCustomerInfo(message.CustomerId);

            var fedExRequest = CreateFedexRequest(shippingInfo, customerInfo);
            await CallFedexWebService(fedExRequest);
            Log.Info($"Order {message.OrderId} shipped with Fedex");
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
}
