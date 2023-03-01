using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ServiceBusDemo.IsolatedFunctionApp;

public class MessageReceiver
{
    
     
    // [Function("ServiceBusFunction")]
    // public void ReceiveMessageDemo(
    //     [ServiceBusTrigger("my-queue", Connection = "SERVICE_BUS_CONN_STR")] string messageBody,
    //     FunctionContext context
    // )
    // {
    //     var log = context.GetLogger<MessageReceiver>();
    //     log.LogInformation("receive message {@Id}: @{Body}", context.BindingContext.BindingData["MessageId"], messageBody);
    //     log.LogInformation("Application Properties is json: {@ApplicationProperties}", context.BindingContext.BindingData["ApplicationProperties"]);
    //     
    //     var random = new Random().Next(10);
    //     if (random >= 8)
    //     {
    //         throw new ApplicationException("Thrown exception for testing DLQ behaviour");
    //     }
    //
    // }
    
}