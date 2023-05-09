using System;
using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Azurerm.Provider;
using HashiCorp.Cdktf.Providers.Azurerm.ResourceGroup;
using HashiCorp.Cdktf.Providers.Azurerm.ServicebusNamespace;
using HashiCorp.Cdktf.Providers.Azurerm.ServicebusQueue;

namespace MyCompany.MyApp
{
    class MainStack : TerraformStack
    {
        public MainStack(Construct scope, string id) : base(scope, id)
        {
            var azurermProvider = new AzurermProvider(this, "AzureRm", new AzurermProviderConfig {
                Features = new AzurermProviderFeatures()
            });

            var rg = new ResourceGroup(this, "rg", new ResourceGroupConfig()
            {
                Name = "rg-brendan-cdktf",
                Location = "australiaeast"
            });
            
            var sbNamespace = new ServicebusNamespace(this, "sbNamespace", new ServicebusNamespaceConfig()
            {
                Name = "sb-brendan-cdktf",
                ResourceGroupName = rg.Name,
                Location = "australiaeast",
                Sku = "Standard",

            });
            
            var queue = new ServicebusQueue(this, "test-queue", new ServicebusQueueConfig()
            {
                Name = "test-queue",
                NamespaceId = sbNamespace.Id,
                MaxDeliveryCount = 5
            });
        }
        
    }
}