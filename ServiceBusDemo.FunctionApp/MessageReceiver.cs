using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ServiceBusDemo.FunctionApp;

public class MessageReceiver
{
    // [FunctionName("MessageReceiver")]                    
    // public static void Run(
    //     [ServiceBusTrigger("my-queue", Connection = "SERVICE_BUS_CONN_STR")] 
    //     string myQueueItem,
    //     Int32 deliveryCount,
    //     DateTime enqueuedTimeUtc,
    //     string messageId,
    //     ILogger log)
    // {
    //     log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
    //     log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
    //     log.LogInformation($"DeliveryCount={deliveryCount}");
    //     log.LogInformation($"MessageId={messageId}");
    // }
}