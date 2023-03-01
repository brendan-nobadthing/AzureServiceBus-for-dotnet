using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServiceBusDemo.IsolatedFunctionApp;

public class MessageReceiverWithRetry
{

    private readonly ServiceBusClient _serviceBusClient;

    private static int maxRetries = 5; 

    public MessageReceiverWithRetry(ServiceBusClient serviceBusClient)
    {
        _serviceBusClient = serviceBusClient;
    }

    [Function("ServiceBusFunction")]
    public async Task ReceiveMessageDemo(
        [ServiceBusTrigger("my-queue", Connection = "SERVICE_BUS_CONN_STR")] string messageBody,
        FunctionContext context
    )
    {
        var log = context.GetLogger<MessageReceiver>();
        try
        {
            HandleMessage(messageBody);
            log.LogInformation("Message {@MessageId} Handled", context.BindingContext.BindingData["MessageId"]);
        }
        catch (Exception ex)
        {
            var messageApplicationProperties = JsonConvert.DeserializeObject<IDictionary<string, string>>(
                context.BindingContext.BindingData["ApplicationProperties"]?.ToString() ?? string.Empty);
            int retries = messageApplicationProperties?.ContainsKey("retries")??false ? int.Parse(messageApplicationProperties["retries"]) : 0;
            retries = retries + 1;

            var sender = _serviceBusClient.CreateSender("my-queue");
            var retryMessage = new ServiceBusMessage(messageBody)
            {
                ScheduledEnqueueTime = DateTimeOffset.UtcNow.AddSeconds(5),
                ApplicationProperties = { { "retries", retries }, {"original-message-id", context.BindingContext.BindingData["MessageId"] } }
            };
            
            if (retries > maxRetries) // set to 0 TTL and scheduled time to force to DLQ
            {
                log.LogError("retries exhausted for {@MessageId}",context.BindingContext.BindingData["MessageId"] );
                //TODO - send the message somewhere else?
                // storage, cosmos or an unwatched queue with a short TTL
            }
            else
            {
                log.LogInformation("retry {@retries} for {@MessageId}", retries, context.BindingContext.BindingData["MessageId"] );
                await sender.SendMessageAsync(retryMessage);
            }
            
           
            // Exception is swallowed so we complete the original message
        }
    }


    private void HandleMessage(string message)
    {
        var rnd = new Random();
        if (rnd.Next(1, 10) < 11)
        {
            throw new ApplicationException("Throw test application");
        }
    }
    
    
    
    
}