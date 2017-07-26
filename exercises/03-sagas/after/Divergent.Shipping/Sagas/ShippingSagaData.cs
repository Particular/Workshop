using System.Collections.Generic;
using NServiceBus;

namespace Divergent.Shipping.Sagas
{
    class ShippingSagaData : ContainSagaData
    {
        public virtual int OrderId { get; set; }
        public virtual int CustomerId { get; set; }

        public virtual bool IsPaymentProcessedYet { get; set; }
        public virtual bool IsOrderSubmitted { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public class Product
        {
            public virtual int Identifier { get; set; }
        }
    }
}
