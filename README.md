# Manage Your Education and Skills Funding View Your Funding Data API

The Manage Your Education and Skills Funding (MYESF) View Your Funding (VYF) Data API provides funding and provider data used by the View Your Funding service.

The API is responsible for:

- Retrieving funding allocation and provider funding information from underlying data stores.
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

To run the application locally, a valid `appsettings.json` file must be created within the API project.

An `appsettings.example.json` file is included within the repository and can be used as a template. All required settings should be populated using values obtained from the appropriate Azure resources.

## Application Settings (`appsettings.json`)

Refer to `appsettings.example.json` for the complete configuration structure.

### Key Configuration Areas

#### AzureAd

Configuration used to validate Azure Active Directory authentication tokens.

#### Authentication

Settings used when acquiring tokens for authenticated communication with protected resources.

#### PdsApplicationInsights

Application Insights configuration for telemetry, monitoring and logging.

#### Repositories:AzureSearch

Configuration used to connect to Azure AI Search indexes used by the application.

#### Repositories:CosmosDb

Configuration used to connect to Cosmos DB and retrieve funding data.

#### StorageAccount

Configuration for Azure Storage resources used by the application.

#### StorageCache

Redis configuration used for application caching.

## Running Locally

1. Create an `appsettings.json` file using `appsettings.example.json` as a template.
2. Populate all required configuration values.
3. Restore project dependencies.