using System;
using NServiceBus;

public class OrderShipped : IEvent
{
    public Guid OrderId { get; set; }
}