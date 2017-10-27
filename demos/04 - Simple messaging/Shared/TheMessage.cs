using NServiceBus;
public class TheMessage : IMessage
{
    public bool ThrowException { get; set; }
}
