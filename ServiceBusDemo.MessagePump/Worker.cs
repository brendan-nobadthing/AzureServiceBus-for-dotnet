using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace ServiceBusDemo.MessagePump;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ServiceBusClient _serviceBusClient;

    public Worker(ILogger<Worker> logger, ServiceBusClient serviceBusClient)
    {
        _logger = logger;
        _serviceBusClient = serviceBusClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sender = _serviceBusClient.CreateSender("my-queue");
        var rnd = new Random();
        while (!stoppingToken.IsCancellationRequested)
        {
            var status = (StatusEnum)rnd.Next(1,6);
            var statusMessage = new StatusMessage(Guid.NewGuid(), status, $"Right now I am {status.GetDisplayName()}", DateTime.UtcNow);

            var sbMessage = new ServiceBusMessage(JsonConvert.SerializeObject(statusMessage));

            // service bus message header properties
            sbMessage.MessageId = statusMessage.Id.ToString();
            sbMessage.Subject = statusMessage.Message;
            //sbMessage.ScheduledEnqueueTime
            //sbMessage.TimeToLive
            sbMessage.ApplicationProperties["status"]=statusMessage.Status.GetDisplayName();

            await sender.SendMessageAsync(sbMessage);
            _logger.LogInformation("Message Sent: {@Message}", statusMessage.Message);

            await Task.Delay(2000, stoppingToken);
        }
    }
}
