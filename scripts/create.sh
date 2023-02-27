#!/bin/bash

# get yourself logged in az cli and set your subscription
# az account set --subscription xxxx

az group create --location AustraliaEast --resource-group rg-brendan-azure-service-bus

az configure --defaults location=AustraliaEast
az configure --defaults group=rg-brendan-azure-service-bus

az servicebus namespace create --name brendan-servicebus --sku Standard

az servicebus queue create --namespace-name brendan-servicebus --name my-queue

az servicebus topic create --namespace-name brendan-servicebus --name my-topic

az servicebus topic subscription create --namespace-name brendan-servicebus --topic-name my-topic --name my-sub-01
az servicebus topic subscription create --namespace-name brendan-servicebus --topic-name my-topic --name my-sub-02
