using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using PDS.ViewYourFunding.Data.Services.Helpers;
using PDS.ViewYourFunding.Data.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Services
{
    /// <summary>
    /// Class for managing an Azure Search service.
    /// </summary>
    public class AzureSearchServiceManager : IAzureSearchServiceManager
    {
        /// <summary>
        /// Gets the instance of the Azure search index client.
        /// </summary>
        private readonly IAzureSearchIndexClient _azureSearchIndexClient;

        /// <summary>
        /// Gets the instance of the Azure search indexer client.
        /// </summary>
        private readonly IAzureSearchIndexerClient _azureSearchIndexerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchServiceManager"/> class.
        /// </summary>
        /// <param name="azureSearchServiceClient">The client to use to access Azure Search admin functions.</param>
        /// <param name="azureSearchIndexerClient">An interface exposing operations of an Azure Search Indexer Client.</param>
        public AzureSearchServiceManager(IAzureSearchIndexClient azureSearchServiceClient, IAzureSearchIndexerClient azureSearchIndexerClient)
        {
            _azureSearchIndexClient = azureSearchServiceClient;
            _azureSearchIndexerClient = azureSearchIndexerClient;
        }

        #region IAzureSearchServiceManager Implementation

        /// <summary>
        /// Has the indexer ever ran (if false, suggests its just been created).
        /// </summary>
        /// <param name="hash">The hash to look for.</param>
        /// <typeparam name="T">Type of search document.</typeparam>
        /// <returns>True if it has ever been ran, false if it has never been ran.</returns>
        public async Task<bool> HasIndexerEverRan<T>(int hash)
            where T : class
        {
            return await _azureSearchIndexerClient.HasIndexerEverRan<T>(hash);
        }

        /// <summary>
        /// Does an index exist that includes the hash we passed?.
        /// </summary>
        /// <typeparam name="T">Type of search document.</typeparam>
        /// <param name="hash">The hash to look for.</param>
        /// <returns>True if the index exists, false if not.</returns>
        public async Task<bool> DoesIndexAndIndexerExist<T>(int hash)
            where T : class
        {
            var inUseIndexHashes = await GetHashComponentsFromIndexNamesForType<T>();
            var inUseIndexerHashes = await GetHashComponentsFromIndexerNamesForType<T>();

            return inUseIndexHashes.Contains(hash) && inUseIndexerHashes.Contains(hash);
        }

        /// <summary>
        /// Rebuild the index for the given type, using the given Cosmos DB data source.
        /// </summary>
        /// <typeparam name="T">The model representing the structure of the indexed documents.</typeparam>
        /// <param name="cosmosDbDataSourceParameters">The parameters configuring the Cosmos DB data source.</param>
        /// <returns>An async task.</returns>
        public async Task RebuildIndex<T>(CosmosDbDataSourceParameters cosmosDbDataSourceParameters)
            where T : class
        {
            var aliasName = AzureSearchHelper.GetSearchIndexAliasNameForType<T>();
            var indexName = $"{aliasName}-{cosmosDbDataSourceParameters.HashCode}-{DateTime.Now.ToString("yyyyMMddhhmm")}";

            await CreateAndPopulateIndex<T>(indexName, cosmosDbDataSourceParameters);

            // Delete old indices, indexers, data sources from Azure.
            await DeleteComponents(aliasName);
        }

        /// <summary>
        /// Get the index names with the prefix of the document we are setup for.
        /// </summary>
        /// <returns>A list of index names.</returns>
        public async Task<IList<string>> GetAllIndexNames()
        {
            return await _azureSearchIndexClient.GetIndexNames();
        }

        /// <summary>
        /// Get the indexer names with the prefix of the document we are setup for.
        /// </summary>
        /// <returns>A list of indexer names.</returns>
        public async Task<IList<string>> GetAllIndexerNames()
        {
            return await _azureSearchIndexerClient.GetIndexerNames();
        }

        /// <summary>
        /// Deletes all indexes, indexers and data sources matching the given prefix.
        /// </summary>
        /// <param name="prefixToDelete">The prefix of the names of components that will be deleted.</param>
        /// <returns>The list of deleted index names.</returns>
        public async Task<List<string>> DeleteComponents(string prefixToDelete)
        {
            var pattern = new Regex("^" + prefixToDelete + @"-\d{14}-", RegexOptions.Compiled);
            const int numberToKeep = 3;

            var allIndexers = (await _azureSearchIndexerClient.GetIndexerNames())
                .OrderByDescending(name => AzureSearchHelper.GetDateTimeComponent(name, prefixToDelete))
                .Skip(numberToKeep).ToList();

            foreach (var indexerName in allIndexers.Where(i => pattern.IsMatch(i)))
            {
                await _azureSearchIndexerClient.DeleteIndexerIfExists(indexerName);
            }

            var allDataSources = (await _azureSearchIndexerClient.GetDataSourceNames())
                .OrderByDescending(name => AzureSearchHelper.GetDateTimeComponent(name, prefixToDelete))
                .Skip(numberToKeep).ToList();

            foreach (var dataSourceName in allDataSources.Where(i => pattern.IsMatch(i)))
            {
                await _azureSearchIndexerClient.DeleteDataSourceIfExists(dataSourceName);
            }

            var allIndices = (await _azureSearchIndexClient.GetIndexNames())
                .OrderByDescending(name => AzureSearchHelper.GetDateTimeComponent(name, prefixToDelete))
                .Skip(numberToKeep).ToList();

            var deletedIndices = new List<string>();

            foreach (var indexName in allIndices.Where(i => pattern.IsMatch(i)))
            {
                await _azureSearchIndexClient.DeleteIndexIfExists(indexName);
                deletedIndices.Add(indexName);
            }

            return deletedIndices;
        }

        #endregion


        #region Private Helpers

        /// <summary>
        /// Get the hash component of all index names for the given document type.
        /// </summary>
        /// <typeparam name="T">Type of search document.</typeparam>
        /// <returns>A list containing the hash component of each index name for the given document type.</returns>
        private async Task<List<int>> GetHashComponentsFromIndexNamesForType<T>()
            where T : class
        {
            var indexNames = await _azureSearchIndexClient.GetIndexNames();
            var prefix = AzureSearchHelper.GetSearchIndexAliasNameForType<T>();

            return indexNames
                .Where(indexName => indexName.StartsWith($"{prefix}-") && indexName.EndsWith($"-{AzureSearchHelper.GetDateTimeComponent(indexName, prefix)}-index"))
                .Select(indexName => AzureSearchHelper.GetHashComponent(indexName, prefix))
                .Where(indexName => indexName != null)
                .Select(indexName => indexName.Value)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Get the hash component of all indexer names for the given document type.
        /// </summary>
        /// <typeparam name="T">Type of search document.</typeparam>
        /// <returns>A list containing the hash component of each indexer name for the given document type.</returns>
        private async Task<List<int>> GetHashComponentsFromIndexerNamesForType<T>()
            where T : class
        {
            var indexerNames = await _azureSearchIndexerClient.GetIndexerNames();
            var prefix = AzureSearchHelper.GetSearchIndexAliasNameForType<T>();

            return indexerNames
                .Where(indexerName => indexerName.StartsWith($"{prefix}-") && indexerName.EndsWith($"-{AzureSearchHelper.GetDateTimeComponent(indexerName, prefix)}-indexer"))
                .Select(indexerName => AzureSearchHelper.GetHashComponent(indexerName, prefix))
                .Where(indexerName => indexerName != null)
                .Select(indexerName => indexerName.Value)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Creates and populates an index.
        /// </summary>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <param name="indexNamePrefix">The index name prefix.</param>
        /// <param name="cosmosDbDataSourceParameters">The Cosmos database parameters.</param>
        /// <returns>An async Task containing an AzureSearchIndexBuildResult.</returns>
        private async Task<AzureSearchIndexBuildResult> CreateAndPopulateIndex<T>(string indexNamePrefix, CosmosDbDataSourceParameters cosmosDbDataSourceParameters)
        {
            var result = new AzureSearchIndexBuildResult();

            try
            {
                var indexName = $"{indexNamePrefix}-index";
                var indexerName = $"{indexNamePrefix}-indexer";
                var dataSourceName = $"{indexNamePrefix}-datasource";

                await _azureSearchIndexClient.CreateIndex<T>(indexName);

                await _azureSearchIndexerClient.CreateCosmosDbDataSource(dataSourceName, cosmosDbDataSourceParameters);

                await _azureSearchIndexerClient.CreateIndexer(indexerName, indexName, dataSourceName);

                result.IndexerSuccess = await _azureSearchIndexerClient.WaitForIndexerCompletion(indexerName, TimeSpan.FromMinutes(10));

                result.IndexName = indexName;
                result.IndexerName = indexerName;
                result.DataSourceName = dataSourceName;
            }
            catch (Exception exception)
            {
                result.Error = exception;
                result.HasError = true;
            }

            return result;
        }

        #endregion
    }
}