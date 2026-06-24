using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PDS.ViewYourFunding.Data.Core;
using PDS.ViewYourFunding.Data.Interfaces;

namespace PDS.ViewYourFunding.Data.Services.ServiceRegistrations
{
    public static class FundingSearchServiceRegistrations
    {
        public static IServiceCollection RegisterFundingSearchService(this IServiceCollection services)
        {
            services.AddSingleton<IFundingSearchService>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value;
                var searchServiceManager = serviceProvider.GetService<IAzureSearchServiceManager>();

                var searchIndexClientManager = new AzureSearchAliasClientManager(
                    configuration.Repositories.AzureSearch.Name,
                    configuration.Repositories.AzureSearch.QueryKey,
                    searchServiceManager,
                    serviceProvider.GetService<ILogger<AzureSearchAliasClientManager>>(),
                    configuration);

                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                var requestUrl = httpContextAccessor.HttpContext.Request;
                var appBaseUrl = $"{requestUrl.Scheme}://{requestUrl.Host}";

                return new AzureFundingSearchService(
                    searchIndexClientManager,
                    configuration,
                    searchServiceManager,
                    serviceProvider.GetService<ILogger<AzureFundingSearchService>>(),
                    serviceProvider.GetService<IBackgroundTaskQueue>());
            });

            return services;
        }
    }
}
