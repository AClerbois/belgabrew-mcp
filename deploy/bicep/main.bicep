// Azure Container Apps deployment for BelgaBrew MCP Server
targetScope = 'resourceGroup'

@description('Location for all resources')
param location string = resourceGroup().location

@description('Name prefix for resources')
param prefix string = 'belgabrew'

var containerAppName = '${prefix}-mcp'
var containerEnvName = '${prefix}-env'
var acrName = '${prefix}acr${uniqueString(resourceGroup().id)}'

// Container Registry
resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: acrName
  location: location
  sku: { name: 'Basic' }
  properties: { adminUserEnabled: true }
}

// Container Apps Environment
resource env 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: containerEnvName
  location: location
  properties: {}
}

// Container App
resource app 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        transport: 'http'
      }
    }
    template: {
      containers: [{
        name: containerAppName
        image: '${acr.properties.loginServer}/${containerAppName}:latest'
        resources: { cpu: json('0.5'), memory: '1Gi' }
      }]
      scale: { minReplicas: 1, maxReplicas: 3 }
    }
  }
}

output mcpEndpoint string = 'https://${app.properties.configuration.ingress.fqdn}/mcp'
