using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace ServiceBusDemo.MessageReceiver;

public class MessageReceiverWorkerWithCaughtException : BackgroundService
{
    private readonly ILogger<MessageReceiverWorker> _logger;
    private readonly ServiceBusClient _serviceBusClient;

    public MessageReceiverWorkerWithCaughtException(ILogger<MessageReceiverWorker> logger, ServiceBusClient serviceBusClient)
    {
        _logger = logger;
        _serviceBusClient = serviceBusClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var messageProcessor = _serviceBusClient.CreateProcessor("my-queue", new ServiceBusProcessorOptions(){AutoCompleteMessages = false});
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



    private async Task HandleMessageAsync (ProcessMessageEventArgs processMessageEventArgs)
    {
        try
        {
            throw new ApplicationException("TestException");
            _logger.LogInformation("Received Message with status: {@Status}", processMessageEventArgs.Message.Subject);
            await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Caught exception");
        }

    }

    private Task HandleReceivedExceptionAsync(ProcessErrorEventArgs exceptionEvent)
    {
        _logger.LogError(exceptionEvent.Exception, "Unable to process message");
        return Task.CompletedTask;
    }


}
