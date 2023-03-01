using ServiceBusDemo.MessageReceiver;
using Microsoft.Extensions.Azure;



var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {    
        //services.AddHostedService<MessageReceiverWorker>();
        //services.AddHostedService<MessageReceiverWorkerWithException>();
        //services.AddHostedService<MessageReceiverWorkerWithCaughtException>();
        //services.AddHostedService<MessageReceiverWorkerWithAbandon>();
        services.AddHostedService<MessageReceiverWorkerWithBackOffRetry>();
        services.AddAzureClients(azBuilder =>
        {
            azBuilder.AddServiceBusClient(hostContext.Configuration["ServiceBusConnectionString"]);
        });
    })
    .Build();

host.Run();
