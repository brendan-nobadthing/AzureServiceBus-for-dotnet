terraform {
    required_providers {
      azurerm = {
        source = "hashicorp/azurerm"
        version = "~> 3.0.2"
      }
    }
    required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name = "rg-brendan-serivcebus-tf"
  location = "australiaeast"
}


resource "azurerm_servicebus_namespace" "namespace" {
  name                = "brendan-sb-namespace-tf"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "Standard"

  tags = {
    source = "terraform"
  }
}

resource "azurerm_servicebus_queue" "test-queue" {
    name = "test-queue"
    namespace_id = azurerm_servicebus_namespace.namespace.id
    max_delivery_count = 5
}

