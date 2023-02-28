using Microsoft.Extensions.Azure;
using ServiceBusDemo.MessagePump;


var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((hostContext, services) =>
{    
    services.AddHostedService<Worker>();
    services.AddAzureClients(azBuilder =>
    {
        azBuilder.AddServiceBusClient(hostContext.Configuration["ServiceBusConnectionString"]);
    });
});
    

var host = builder.Build();
host.Run();
