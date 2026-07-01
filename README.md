# Manage Your Education and Skills Funding View Your Funding Data API

The Manage Your Education and Skills Funding (MYESF) View Your Funding (VYF) Data API provides funding and provider data used by the View Your Funding service.

The API is responsible for:

 Retrieving funding allocation and provider funding information from underlying data stores.
- Serving funding-related data to the View Your Funding user interface and supporting services.
- Querying Azure Search indexes to provide access to funding data.
- Accessing Cosmos DB collections containing funding and provider funding information.
- Providing secured endpoints using Azure Active Directory authentication.
## Provider

[The Department for Education](https://www.gov.uk/government/organisations/department-for-education)

## About this project

This project is an ASP.NET Core 8 Web API deployed within Microsoft Azure.

The application uses several Azure services including:

- Azure App Service
- Azure Cosmos DB
- Azure AI Search
- Azure Storage Accounts
- Azure Application Insights
- Redis Cache

The API forms part of the MYESF platform and supports the delivery of funding information to authorised users.

# Local Configuration Guide

In order to run the application locally a valid `appsettings.json` file will need to be created in the `FundingApi` project. Below, and included in the repo, there is `appsettings.example.json` which can be used as a base and populated with the required values, which can be retrieved from the Azure Portal.

## Application Settings (`appsettings.json`)

```json
{
  "AzureAd": {
    "Audience": "[Azure AD Audience]",
    "ClientId": "[Azure AD Client ID]",
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "[Azure AD Tenant ID]"
  },
  "Environment": "[Environment]",
  "Authentication": {
    "ClientId": "[Application Client ID]",
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "[Tenant ID]"
  },
  "EnableOauthSecurity": true,
  "Logging": {
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft": "Error"
      }
    },
    "LogLevel": {
      "Default": "Information"
    }
  },
  "PdsApplicationInsights": {
    "InstrumentationKey": "[Application Insights Instrumentation Key]",
    "Environment": "[Environment]"
  },
  "Repositories": {
    "AzureSearch": {
      "AdminKey": "[Azure Search Admin Key]",
      "Name": "[Azure Search Service Name]",
      "QueryKey": "[Azure Search Query Key]"
    },
    "CosmosDb": {
      "ConnectionString": "[Cosmos DB Connection String]",
      "FundingCollection": "[Funding Collection Name]",
      "ProviderFundingCollection": "[Provider Funding Collection Name]"
    }
  },
  "StorageAccount": {
    "AccountName": "[Storage Account Name]",
    "AccountKey": "[Storage Account Key]"
  },
  "StorageCache": {
    "ConnectionString": "[Redis Connection String]",
    "ItemLifetimeInMinutes": "480"
  },
  "RestrictedFundingStreamCodes": "[Funding Stream Codes]",
  "FilterOnFundingVersion": true,
  "RestrictedVariationReasons": "[Variation Reasons]",
  "AzureSearch": {
    "ChangeInIndex": {
      "LoggedIn_Parent": false,
      "LoggedIn_Child": false
    }
  }
}
```

### Setting Details

- **`AzureAd:Audience`**  
  The audience value used when validating Azure Active Directory access tokens for the API.

- **`AzureAd:ClientId`**  
  The unique identifier of the Azure Active Directory application registration for the API.

- **`AzureAd:Instance`**  
  The authority URL used for Azure Active Directory authentication.

- **`AzureAd:TenantId`**  
  The Azure Active Directory tenant identifier used by the application.

- **`Environment`**  
  The environment the application is running within, such as Local, Dev, Test, AT or Production.

- **`Authentication:ClientId`**  
  The client identifier used when requesting authentication tokens to access protected resources.

- **`Authentication:Instance`**  
  The authentication authority endpoint used when obtaining access tokens.

- **`Authentication:TenantId`**  
  The tenant identifier associated with the application's Azure Active Directory registration.

- **`EnableOauthSecurity`**  
  Determines whether OAuth authentication and authorisation are enforced by the API.

- **`Logging:ApplicationInsights:LogLevel:Default`**  
  The default logging level for the service when logging to Application Insights.

- **`Logging:ApplicationInsights:LogLevel:Microsoft`**  
  The logging level used for Microsoft framework components when logging to Application Insights.

- **`Logging:LogLevel:Default`**  
  The default logging level used by the service.

- **`PdsApplicationInsights:InstrumentationKey`**  
  The key value for the Application Insights resource used for logging and monitoring.

- **`PdsApplicationInsights:Environment`**  
  The environment which the application is running within for logging and monitoring purposes.

- **`Repositories:AzureSearch:AdminKey`**  
  The administrative access key used to manage and access Azure AI Search resources.

- **`Repositories:AzureSearch:Name`**  
  The Azure AI Search service name used by the application.

- **`Repositories:AzureSearch:QueryKey`**  
  The query key used to retrieve information from Azure AI Search indexes.

- **`Repositories:CosmosDb:ConnectionString`**  
  The connection string used to connect to the Cosmos DB database.

- **`Repositories:CosmosDb:FundingCollection`**  
  The Cosmos DB collection containing funding records.

- **`Repositories:CosmosDb:ProviderFundingCollection`**  
  The Cosmos DB collection containing provider funding records.

- **`StorageAccount:AccountName`**  
  The Azure Storage Account name used by the application.

- **`StorageAccount:AccountKey`**  
  The access key associated with the configured Azure Storage Account.

- **`StorageCache:ConnectionString`**  
  The Redis cache connection string used for distributed caching.

- **`StorageCache:ItemLifetimeInMinutes`**  
  The default duration that cached items remain valid before expiring.

- **`RestrictedFundingStreamCodes`**  
  A comma-separated list of funding stream codes which should be treated as restricted.

- **`FilterOnFundingVersion`**  
  Determines whether funding data should be filtered based on funding version.

- **`RestrictedVariationReasons`**  
  A comma-separated list of variation reasons that should be treated as restricted.

- **`AzureSearch:ChangeInIndex:LoggedIn_Parent`**  
  Determines whether change-in-index information is available to parent users.

- **`AzureSearch:ChangeInIndex:LoggedIn_Child`**  
  Determines whether change-in-index information is available to child users.

## Environment Setup

Before running the application locally:

1. Create an `appsettings.json` file from the supplied `appsettings.example.json`.
2. Populate all required configuration values using the appropriate Azure resources.
3. Ensure you have access to the Azure Active Directory application registrations used by the service.
4. Ensure you have access to the Azure Cosmos DB instance and collections containing funding data.
5. Ensure you have access to the Azure AI Search service and indexes used by the application.
6. Ensure you have access to the configured Azure Storage Account.
7. Ensure the configured Redis Cache instance is accessible.
8. Verify Application Insights configuration has been populated if local telemetry is required.
9. Restore project dependencies and run the API locally.