using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PDS.ViewYourFunding.Data.Core;
using PDS.ViewYourFunding.Data.Interfaces;
using System;

namespace PDS.ViewYourFunding.Data.Services.ServiceRegistrations
{
    /// <summary>
    /// The Azure Search Service Manager registration.
    /// </summary>
    public static class AzSearchServiceRegistration
    {
        public static IServiceCollection RegisterAzSearchService(this IServiceCollection services)
        {
            services.AddSingleton<IAzureSearchServiceManager>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value;

                Uri searchServiceEndPoint = new($"https://{configuration.Repositories.AzureSearch.Name}.search.windows.net");
                string adminApiKey = configuration.Repositories.AzureSearch.AdminKey;

                var searchIndexClient = new SearchIndexClient(searchServiceEndPoint, new AzureKeyCredential(adminApiKey));

                var searchIndexerClient = new SearchIndexerClient(searchServiceEndPoint, new AzureKeyCredential(adminApiKey));

                return new AzureSearchServiceManager(
                    new AzureSearchIndexClient(searchIndexClient), new AzureSearchIndexerClient(searchIndexerClient));
            });

            return services;
        }
    }
}