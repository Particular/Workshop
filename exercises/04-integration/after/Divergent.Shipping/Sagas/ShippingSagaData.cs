using System.Collections.Generic;
using NServiceBus;

namespace Divergent.Shipping.Sagas
{
    public class ShippingSagaData : ContainSagaData
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public bool IsOrderSubmitted { get; set; }
        public bool IsPaymentProcessed { get; set; }
        public ICollection<Product> Products { get; set; }

        public class Product
        {
            public int Identifier { get; set; }
        }
    }
}