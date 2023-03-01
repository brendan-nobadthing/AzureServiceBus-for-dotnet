using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddAzureClients(azBuilder =>
        {
            azBuilder.AddServiceBusClient(Environment.GetEnvironmentVariable("SERVICE_BUS_CONN_STR"));
        });
        
    })
    .Build();

host.Run();