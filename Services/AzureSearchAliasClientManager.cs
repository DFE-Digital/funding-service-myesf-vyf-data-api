using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PDS.ViewYourFunding.Data.Core;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Services.Helpers;
using System.Collections.Concurrent;

namespace PDS.ViewYourFunding.Data.Services
{
    /// <summary>
    /// A class for managing Azure Search Index Client connections.
    /// </summary>
    public class AzureSearchAliasClientManager : IAzureSearchAliasClientManager
    {
        private readonly string _searchServiceName;
        private readonly string _queryApiKey;
        private readonly ConcurrentDictionary<string, AzureSearchAliasClient> _searchAliasClients;
        private readonly IAzureSearchServiceManager _searchServiceManager;
        private readonly ILogger<AzureSearchAliasClientManager> _loggerService;
        private readonly ApplicationConfiguration _applicationConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchAliasClientManager"/> class.
        /// </summary>
        /// <param name="searchServiceName">The name of the Azure Search service.</param>
        /// <param name="queryApiKey">The API key used for querying.</param>
        /// <param name="searchServiceManager">The search service manager to use.</param>
        /// <param name="loggerService">Logger service to use.</param>
        /// <param name="applicationConfiguration">Application config.</param>
        public AzureSearchAliasClientManager(string searchServiceName, string queryApiKey, IAzureSearchServiceManager searchServiceManager, ILogger<AzureSearchAliasClientManager> loggerService, ApplicationConfiguration applicationConfiguration)
        {
            _searchServiceName = searchServiceName;
            _queryApiKey = queryApiKey;
            _searchAliasClients = new ConcurrentDictionary<string, AzureSearchAliasClient>();
            _searchServiceManager = searchServiceManager;
            _loggerService = loggerService;
            _applicationConfiguration = applicationConfiguration;
        }

        /// <summary>
        /// Get the Search Index Client for the given type.
        /// </summary>
        /// <typeparam name="T">The document type of the index.</typeparam>
        /// <param name="classThumbprint">The hash/thumbprint of T.</param>
        /// <returns>A Search Index Client for the given type.</returns>
        public IAzureSearchAliasClient<T> GetSearchAliasClient<T>(int classThumbprint)
            where T : class
        {
            var aliasName = AzureSearchHelper.GetSearchIndexAliasNameForType<T>();

            if (_searchAliasClients.ContainsKey(aliasName))
            {
                return _searchAliasClients[aliasName] as AzureSearchAliasClient<T>;
            }

            var client = new AzureSearchAliasClient<T>(_searchServiceName, _queryApiKey, _searchServiceManager, _loggerService, classThumbprint, _applicationConfiguration);
            _searchAliasClients.TryAdd(aliasName, client);

            return client;
        }
    }
}
