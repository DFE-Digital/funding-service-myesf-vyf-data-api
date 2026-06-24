using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Logging;
using PDS.ViewYourFunding.Data.Core;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Services.Exceptions;
using PDS.ViewYourFunding.Data.Services.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Services
{
    /// <summary>
    /// A class that wraps the Azure SearchIndexClient using aliases for the index names.
    /// </summary>
    /// <typeparam name="T">The document type contained in the Azure Search index.</typeparam>
    public class AzureSearchAliasClient<T> : AzureSearchAliasClient, IAzureSearchAliasClient<T>
        where T : class
    {
        private readonly string _aliasName, _searchServiceName, _queryApiKey;
        private readonly int _classThumprint;
        private readonly ApplicationConfiguration _applicationConfiguration;

        private SearchClient _client;
        private IAzureSearchServiceManager _searchServiceManager;
        private ILogger _loggingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchAliasClient{T}"/> class.
        /// </summary>
        /// <param name="searchServiceName">The name of the Azure Search service.</param>
        /// <param name="queryApiKey">The API key to use for search queries.</param>
        /// <param name="searchServiceManager">The search service manager to use.</param>
        /// <param name="loggingService">Service to use for logging.</param>
        /// <param name="classThumprint">The class thumbprint for T.</param>
        /// <param name="applicationConfiguration">The application configuration.</param>
        public AzureSearchAliasClient(
            string searchServiceName,
            string queryApiKey,
            IAzureSearchServiceManager searchServiceManager,
            ILogger loggingService,
            int classThumprint,
            ApplicationConfiguration applicationConfiguration)
        {
            _searchServiceName = searchServiceName;
            _queryApiKey = queryApiKey;
            _searchServiceManager = searchServiceManager;
            _aliasName = AzureSearchHelper.GetSearchIndexAliasNameForType<T>();
            _loggingService = loggingService;
            _classThumprint = classThumprint;
            _applicationConfiguration = applicationConfiguration;
        }

        #region IAzureSearchAliasClient<T> Implementation

        /// <summary>
        /// Gets the number of documents in the Azure Search index.
        /// </summary>
        /// <returns>The number of documents in the Azure Search index.</returns>
        public async Task<long> CountDocumentsAsync()
        {
            try
            {
                await SetupSearchIndexClientIfRequired();
                return await CountDocumentsUsingClientAsync();
            }
            catch (NoMatchingIndexException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex is RequestFailedException && (ex as RequestFailedException).Status == (int)HttpStatusCode.NotFound)
                {
                    _client = null;

                    _loggingService?.LogError(ex, string.Empty);
                }

                await SetupSearchIndexClientIfRequired();
                return await CountDocumentsUsingClientAsync();
            }
        }

        /// <summary>
        /// Searches for documents in the Azure Search index.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="searchOptions">The search options.</param>
        /// <returns>An object containing information about the result of the search.</returns>
        public async Task<SearchResults<T>> SearchDocumentsAsync(string searchText, SearchOptions searchOptions)
        {
            try
            {
                await SetupSearchIndexClientIfRequired();
                return await SearchDocumentsWithClientAsync(searchText, searchOptions);
            }
            catch (NoMatchingIndexException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex is RequestFailedException && (ex as RequestFailedException).Status == (int)HttpStatusCode.NotFound)
                {
                    _client = null;

                    _loggingService?.LogError(ex, string.Empty);
                }

                await SetupSearchIndexClientIfRequired();
                return await SearchDocumentsWithClientAsync(searchText, searchOptions);
            }
        }

        #endregion


        #region Private Helpers

        /// <summary>
        /// Count the documents in the index.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        private async Task<long> CountDocumentsUsingClientAsync()
        {
            return await _client.GetDocumentCountAsync();
        }

        /// <summary>
        /// Search documents in the index.
        /// </summary>
        /// <param name="searchText">The text to search for (prepared into the expected format).</param>
        /// <param name="searchOptions">The searchOptions to filter and paginate with.</param>
        /// <returns>The results of the search.</returns>
        private async Task<SearchResults<T>> SearchDocumentsWithClientAsync(string searchText, SearchOptions searchOptions)
        {
            return await _client?.SearchAsync<T>(searchText, searchOptions);
        }

        /// <summary>
        /// Setup the search index if its required.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        private async Task SetupSearchIndexClientIfRequired()
        {
            if (_client != null)
            {
                return;
            }

            var indexNames = await _searchServiceManager.GetAllIndexNames();
            var matchingIndexNames = indexNames
                .Where(indexName => indexName.StartsWith($"{_aliasName}-")
                    && indexName.Contains(_classThumprint.ToString())
                    && indexName.EndsWith($"-{AzureSearchHelper.GetDateTimeComponent(indexName, _aliasName).ToString()}-index"))
                .OrderBy(indexName => AzureSearchHelper.GetDateTimeComponent(indexName, _aliasName))
                .ToList();

            if (!matchingIndexNames.Any())
            {
                throw new NoMatchingIndexException();
            }

            // Get the first as its already ordered by date asc
            var newIndexName = matchingIndexNames.First();

            _client = new SearchClient(new Uri($"https://{_searchServiceName}.search.windows.net"), newIndexName, new Azure.AzureKeyCredential(_queryApiKey));
        }

        #endregion
    }
}