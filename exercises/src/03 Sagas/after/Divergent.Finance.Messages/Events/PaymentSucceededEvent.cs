using System;

namespace Divergent.Finance.Messages.Events
{
    public class PaymentSucceededEvent
    {
        public Guid OrderId { get; set; }
    }
}
