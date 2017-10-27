using System;
using NServiceBus;

public class OrderPaid : IEvent
{
    public Guid OrderId { get; set; }
}