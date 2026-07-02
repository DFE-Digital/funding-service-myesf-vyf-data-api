namespace PDS.VYF.Data.Services.ServiceRegistrations
{
    using Azure;
    using Azure.Core;
    using Azure.Data.Tables;
    using Azure.Search.Documents.Indexes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using PDS.ViewYourFunding.Data.Core;
    using PDS.VYF.Data.Services.Abstracts.InfraServices;
    using PDS.VYF.Data.Services.Implementations.InfraServices;
    using System;

    /// <summary>
    /// The infra services registrations.
    /// </summary>
    public static class InfraServicesRegistrations
    {
        /// <summary>
        /// Registers the infra services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>The same Services Instances.</returns>
        public static IServiceCollection RegisterInfraServices(this IServiceCollection services)
        {
            services.AddSingleton<IAzSearchCosmosServices, AzSearchCosmosServices>();
            services.AddSingleton(s => BuildAzSearchManagingServices(s));
            services.AddSingleton(BuildAzSearchSearchingServices);
            services.AddSingleton(BuildAzTableServices);
            services.AddSingleton<IAzSearchThumbPrintServices, AzSearchThumbPrintServices>();

            return services;
        }

        private static IAzSearchSearchingServices BuildAzSearchSearchingServices(IServiceProvider serviceProvider)
        {
            var appConfiguration = serviceProvider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value;

            Uri searchServiceEndPoint = new($"https://{appConfiguration.Repositories.AzureSearch.Name}.search.windows.net");
            string queryApiKey = appConfiguration.Repositories.AzureSearch.QueryKey;

            var searchIndexClient = new SearchIndexClient(searchServiceEndPoint, new AzureKeyCredential(queryApiKey));

            return new AzSearchSearchingServices(
                            serviceProvider.GetRequiredService<IAzSearchCosmosServices>(),
                            searchIndexClient);
        }

        private static IAzSearchManagingServices BuildAzSearchManagingServices(IServiceProvider serviceProvider)
        {
            var appConfiguration = serviceProvider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value;

            Uri searchServiceEndPoint = new Uri($"https://{appConfiguration.Repositories.AzureSearch.Name}.search.windows.net");
            string adminApiKey = appConfiguration.Repositories.AzureSearch.AdminKey;

            var searchIndexClient = new SearchIndexClient(searchServiceEndPoint, new AzureKeyCredential(adminApiKey));
            var searchIndexerClient = new SearchIndexerClient(searchServiceEndPoint, new AzureKeyCredential(adminApiKey));

            return new AzSearchManagingServices(
                serviceProvider.GetRequiredService<ILogger<AzSearchManagingServices>>(),
                appConfiguration,
                serviceProvider.GetRequiredService<IAzSearchCosmosServices>(),
                serviceProvider.GetRequiredService<IAzSearchThumbPrintServices>(),
                searchIndexClient,
                searchIndexerClient);
        }

        private static IAzTableServices BuildAzTableServices(IServiceProvider serviceProvider)
        {
            var appConfiguration = serviceProvider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value;

            var accountName = appConfiguration.StorageAccount.AccountName;
            var storageAccountKey = appConfiguration.StorageAccount.AccountKey;
            var storageUri = new Uri($"https://{accountName}.table.core.windows.net/");

            var tableServiceClient = new TableServiceClient(
                                        storageUri,
                                        new TableSharedKeyCredential(accountName, storageAccountKey),
                                        new TableClientOptions()
                                        {
                                            Retry =
                                                    {
                                                                Delay = TimeSpan.FromSeconds(2),
                                                                MaxRetries = 5,
                                                                Mode = RetryMode.Exponential,
                                                                MaxDelay = TimeSpan.FromSeconds(10),
                                                                NetworkTimeout = TimeSpan.FromSeconds(100)
                                                    },
                                        });

            return new AzTableServices(tableServiceClient);
        }
    }
}
