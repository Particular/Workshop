using System;

namespace Messages.Events
{
    public class OrderReceived
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
    }
}