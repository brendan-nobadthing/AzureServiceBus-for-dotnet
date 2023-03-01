using Azure.Messaging.ServiceBus;

namespace ServiceBusDemo.MessageReceiver;

// inspired by https://github.com/jeffhollan/functions-csharp-queue-exponential/blob/master/ExponentialRetry.cs
// not to be confused with https://learn.microsoft.com/en-us/dotnet/api/azure.messaging.servicebus.servicebusretryoptions?view=azure-dotnet

public class MessageReceiverWorkerWithBackOffRetry : BackgroundService
{
    private readonly ILogger<MessageReceiverWorker> _logger;
    private readonly ServiceBusClient _serviceBusClient;

    public MessageReceiverWorkerWithBackOffRetry(ILogger<MessageReceiverWorker> logger, ServiceBusClient serviceBusClient)
    {
        _logger = logger;
        _serviceBusClient = serviceBusClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var messageProcessor = _serviceBusClient.CreateProcessor("my-queue");
        messageProcessor.ProcessMessageAsync += HandleMessageAsync;
        messageProcessor.ProcessErrorAsync += HandleReceivedExceptionAsync;
        

        _logger.LogInformation("Starting Message Processor...");
        await messageProcessor.StartProcessingAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("Closing message processor..."); 
        await messageProcessor.CloseAsync(cancellationToken: stoppingToken);
        _logger.LogInformation("Message processor closed"); 

    }


    private static int maxRetries = 5;
    
    private async Task HandleMessageAsync (ProcessMessageEventArgs processMessageEventArgs)
    {
        
        try 
        {
            throw new ApplicationException("TestException");
            _logger.LogInformation("Received Message with status: {@Status}", processMessageEventArgs.Message.Subject);
            await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
        } catch (Exception ex) 
        {
            _logger.LogError(ex, ex.Message);
            var originalMessage = processMessageEventArgs.Message;
            var retryMessage = new ServiceBusMessage(originalMessage);
            
            // If the message doesn't have a retry-count, set as 0
            if(!retryMessage.ApplicationProperties.ContainsKey("retry-count"))
            {
                retryMessage.ApplicationProperties["retry-count"] = 0;
                retryMessage.ApplicationProperties["original-SequenceNumber"] = originalMessage.SequenceNumber;
            }

            // If there are more retries available
            if((int)retryMessage.ApplicationProperties["retry-count"] < maxRetries)
            {
                var retryCount = (int)retryMessage.ApplicationProperties["retry-count"] + 1;
                var interval = 5 * retryCount;
                var scheduledTime = DateTimeOffset.Now.AddSeconds(interval);

                retryMessage.ApplicationProperties["retry-count"] = retryCount;
                var retrySender = _serviceBusClient.CreateSender("my-queue");
                
                await retrySender.ScheduleMessageAsync(retryMessage, scheduledTime);
                await processMessageEventArgs.CompleteMessageAsync(originalMessage);
                _logger.LogInformation($"Scheduled message retry {retryCount} to wait {interval} seconds and arrive at {scheduledTime.UtcDateTime}");
            }

            // If there are no more retries, deadletter the message
            else 
            {
                _logger.LogError("Exhausted all retries for message sequence # {@SequenceNumber}", originalMessage.ApplicationProperties["original-SequenceNumber"]?.ToString());
                await processMessageEventArgs.DeadLetterMessageAsync(originalMessage);
            }
        }
    }

    private Task HandleReceivedExceptionAsync(ProcessErrorEventArgs exceptionEvent)
    {
        _logger.LogError(exceptionEvent.Exception, "Unable to process message");
        return Task.CompletedTask;
    }


}
