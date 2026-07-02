using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using PDS.ViewYourFunding.Data.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Services
{
    /// <summary>
    /// A class exposing operations of an Azure Search Indexer Client.
    /// </summary>
    public class AzureSearchIndexerClient : IAzureSearchIndexerClient
    {
        /// <summary>
        /// The search indexer client instance.
        /// </summary>
        private readonly SearchIndexerClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchIndexerClient"/> class.
        /// </summary>
        /// <param name="searchIndexerClient">The search indexer client.</param>
        public AzureSearchIndexerClient(SearchIndexerClient searchIndexerClient)
        {
            _client = searchIndexerClient;
        }

        /// <summary>
        /// Create a Cosmos DB data source for Azure Search.
        /// </summary>
        /// <param name="dataSourceName">The name of the data source to create.</param>
        /// <param name="dataSourceParameters">The data source parameters including connection and query information.</param>
        /// <returns>A SearchIndexerDataSourceConnection object.</returns>
        public async Task<Azure.Response<SearchIndexerDataSourceConnection>> CreateCosmosDbDataSource(string dataSourceName, CosmosDbDataSourceParameters dataSourceParameters)
        {
            var indexerDataContainer = new SearchIndexerDataContainer(dataSourceParameters.CollectionName)
            {
                Query = dataSourceParameters.Query,
            };
            var cosmosDbDataSource = new SearchIndexerDataSourceConnection(dataSourceName, SearchIndexerDataSourceType.CosmosDb, dataSourceParameters.ConnectionString, indexerDataContainer)
            {
                DataChangeDetectionPolicy = new HighWaterMarkChangeDetectionPolicy("_ts"),
            };

            return await _client.CreateOrUpdateDataSourceConnectionAsync(cosmosDbDataSource);
        }

        /// <summary>
        /// Create an Azure Search indexer.
        /// </summary>
        /// <param name="indexerName">The name of the indexer.</param>
        /// <param name="indexName">The name of the index which should be populated by this indexer.</param>
        /// <param name="dataSourceName">The name of the data source for this indexer.</param>
        /// <param name="updateInterval">An optional <see cref="TimeSpan"/> representing the update frequency for this indexer. Defaults to hourly if not specified.</param>
        /// <returns>As async Task containing the result of CreateIndexerAsync(indexer).</returns>
        public async Task<SearchIndexer> CreateIndexer(string indexerName, string indexName, string dataSourceName, TimeSpan? updateInterval = null)
        {
            var indexer = new SearchIndexer(indexerName, dataSourceName, indexName)
            {
                Schedule = new(updateInterval ?? TimeSpan.FromHours(1)),
                Parameters = new() { BatchSize = 100, },
            };

            return await _client.CreateIndexerAsync(indexer);
        }

        /// <summary>
        /// Wait for the given indexer to finish indexing documents.
        /// </summary>
        /// <param name="indexerName">The name of the indexer to wait for.</param>
        /// <param name="timeout">The maximum amount of time to wait.</param>
        /// <returns>If the indexing operation completed successfully within the allotted time, then true, otherwise false.</returns>
        public async Task<bool> WaitForIndexerCompletion(string indexerName, TimeSpan timeout)
        {
            var isComplete = false;
            var executionTimer = Stopwatch.StartNew();

            while (!isComplete && executionTimer.Elapsed < timeout)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                var indexerInfo = await _client.GetIndexerStatusAsync(indexerName);

                isComplete = indexerInfo.Value.LastResult?.Status == IndexerExecutionStatus.Success;
            }

            return isComplete;
        }

        /// <summary>
        /// Get the indexer names.
        /// </summary>
        /// <returns>A list of indexer names.</returns>
        public async Task<IList<string>> GetIndexerNames()
        {
            return (await _client.GetIndexerNamesAsync()).Value.ToList();
        }

        /// <summary>
        /// Has the indexer ever ran before.
        /// </summary>
        /// <param name="hash">The unique thumbprint/hash for a class definition.   </param>
        /// <typeparam name="T">The class type to look up search indexs for.</typeparam>
        /// <returns>True if it has ever ran, false if it hasn't.</returns>
        public async Task<bool> HasIndexerEverRan<T>(int hash)
            where T : class
        {
            var prefix = AzureSearchHelper.GetSearchIndexAliasNameForType<T>();

            var indexerNames = GetIndexerNames().Result
                .Where(indexerName => indexerName.Contains($"-{hash.ToString()}-"))
                .OrderBy(indexerName => AzureSearchHelper.GetDateTimeComponent(indexerName, prefix))
                .ToList();

            // If there are no indexers, they must never have been ran.
            if (!indexerNames.Any())
            {
                return false;
            }

            var indexerStatus = await _client.GetIndexerStatusAsync(indexerNames.First());
            var indexerLastRunResult = indexerStatus?.Value.LastResult;

            // Initial tracking state is set when its ran
            return indexerLastRunResult?.InitialTrackingState != null;
        }

        /// <summary>
        /// Deletes an existing indexer.
        /// </summary>
        /// <param name="indexerName">The name of the indexer to delete.</param>
        /// <returns>An async Task containing the result of DeleteAsync(indexerName) if the indexer exists.</returns>
        public async Task DeleteIndexerIfExists(string indexerName)
        {
            if (_client.GetIndexerNames().Value.Contains(indexerName))
            {
                await _client.DeleteIndexerAsync(indexerName);
            }
        }

        /// <summary>
        /// Get the Data Source names.
        /// </summary>
        /// <returns>A list of Data Source names.</returns>
        public async Task<IList<string>> GetDataSourceNames()
        {
            var readOnlyDataSourceNames = (await _client.GetDataSourceConnectionNamesAsync()).Value;

            return (IList<string>)readOnlyDataSourceNames;
        }

        /// <summary>
        /// Deletes an existing datasource.
        /// </summary>
        /// <param name="dataSourceName">The name of the data source to delete.</param>
        /// <returns>An async Task containing the result of DeleteAsync(dataSourceName) if the data source exists.</returns>
        public async Task DeleteDataSourceIfExists(string dataSourceName)
        {
            if (_client.GetDataSourceConnectionNames().Value.Contains(dataSourceName))
            {
                await _client.DeleteDataSourceConnectionAsync(dataSourceName);
            }
        }
    }
}