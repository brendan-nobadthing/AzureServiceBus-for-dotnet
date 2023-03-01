using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;

namespace ServiceBusDemo.FunctionApp;

public class MessageReceiverWithBackoff
{

    private readonly ServiceBusClient _serviceBusClient;

    public MessageReceiverWithBackoff(ServiceBusClient serviceBusClient)
    {
        _serviceBusClient = serviceBusClient;
    }

    private static int maxRetries = 5; 

    [FunctionName("MessageReceiverWithBackoff")]                    
    public async Task Run(
        [ServiceBusTrigger("my-queue", Connection = "SERVICE_BUS_CONN_STR")]
        ServiceBusReceivedMessage sbMessage,
        ServiceBusMessageActions messageActions, // This exact name is required for binding to work!
        ILogger log) 
    {
        log.LogInformation($"C# ServiceBus queue trigger function processing message: {sbMessage.Body.ToString()}");

        try
        {
            var rnd = new Random();
            if (rnd.Next(minValue:1, maxValue:10) > 5)
            {
                throw new ApplicationException("Test Exception Thrown");
            }
            log.LogInformation("Message {@Id} consumed successfully", sbMessage.MessageId);
            await messageActions.CompleteMessageAsync(sbMessage);
        }
        catch (Exception ex)
        {
            
            var retryMessage = new ServiceBusMessage(sbMessage);
            
            // If the message doesn't have a retry-count, set as 0
            if(!retryMessage.ApplicationProperties.ContainsKey("retry-count"))
            {
                retryMessage.ApplicationProperties["retry-count"] = 0;
                retryMessage.ApplicationProperties["original-SequenceNumber"] = sbMessage.SequenceNumber;
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
                await messageActions.CompleteMessageAsync(sbMessage);
                log.LogInformation($"Scheduled message retry {retryCount} to wait {interval} seconds and arrive at {scheduledTime.UtcDateTime}");
            }

            // If there are no more retries, deadLetter the message
            else 
            {
                log.LogError("Exhausted all retries for message sequence # {@SequenceNumber}", sbMessage.ApplicationProperties["original-SequenceNumber"]?.ToString());
                await messageActions.DeadLetterMessageAsync(sbMessage);
            }
            
        }
        

    }
}