# Manage Your Education and Skills Funding VYF Data API

The Manage Your Education and Skills Funding (MYESF) VYF Data API provides backend services used by MYESF applications to manage and retrieve funding-related and provider data.

This API supports:
- Retrieval and processing of funding data
- Integration with storage and caching services
- Serving data to internal MYESF services and applications

The service is part of the wider DfE Funding Service ecosystem and will evolve to include additional shared/common functionality where required.

---

## Provider

https://www.gov.uk/government/organisations/department-for-education

---

## About this project

This project is an ASP.NET Core Web API.

- Runs on Azure App Service
- Uses Azure-based services such as Application Insights and Storage
- Uses Redis for distributed caching (via Docker locally)

---

# Local Configuration Guide

To run the application locally, you must create a valid `appsettings.json` file.

A template file (`appsettings.example.json`) is included in the repository and should be used as a base.

---

## Application Setup

1. Navigate to the API project: