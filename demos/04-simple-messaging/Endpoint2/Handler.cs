using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class Handler : IHandleMessages<TheMessage>
{
    static ILog log = LogManager.GetLogger<Handler>();

    public Task Handle(TheMessage message, IMessageHandlerContext context)
    {
        var headers = context.MessageHeaders;
        //var isRetryOfMessage = !string.IsNullOrEmpty(headers["ServiceControl.Retry.UniqueMessageId"]);

        if (message.ThrowException)
        {
            log.Info($"Received. MessageId:{context.MessageId}. Going to throw an exception.");
            throw new Exception("The exception message.");
        }

        log.Info($"Finished handling. MessageId:{context.MessageId}");

        return Task.CompletedTask;
    }
}
