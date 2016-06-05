using System;
using System.Collections.Generic;
using NServiceBus;

namespace Divergent.Shipping.Sagas
{
    public class ShippingSagaData : ContainSagaData
    {
        public virtual Guid OrderId { get; set; }

        public virtual Guid CustomerId { get; set; }

        public virtual bool IsOrderSubmitted { get; set; }
        public virtual bool IsPaymentProcessedYet { get; set; }
        
        public virtual ICollection<Product> Products { get; set; }

        public class Product
        {
            public virtual Guid Identifier { get; set; }
        }

    }
}