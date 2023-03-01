using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;

[assembly: FunctionsStartup(typeof(ServiceBusDemo.FunctionApp.Startup))]

namespace ServiceBusDemo.FunctionApp;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddAzureClients(azBuilder =>
        {
            azBuilder.AddServiceBusClient(Environment.GetEnvironmentVariable("SERVICE_BUS_CONN_STR"));
        });
    }
}
