using Azure.Search.Documents.Indexes.Models;
using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Interfaces
{
    /// <summary>
    /// An interface exposing operations of an Azure Search Indexer Client.
    /// </summary>
    public interface IAzureSearchIndexerClient
    {
        /// <summary>
        /// Create a Cosmos DB data source for Azure Search.
        /// </summary>
        /// <param name="dataSourceName">The name of the data source to create.</param>
        /// <param name="dataSourceParameters">The data source parameters including connection and query information.</param>
        /// <returns>The created data source.</returns>
        Task<Azure.Response<SearchIndexerDataSourceConnection>> CreateCosmosDbDataSource(string dataSourceName, CosmosDbDataSourceParameters dataSourceParameters);

        /// <summary>
        /// Create an Azure Search indexer.
        /// </summary>
        /// <param name="indexerName">The name of the indexer.</param>
        /// <param name="indexName">The name of the index which should be populated by this indexer.</param>
        /// <param name="dataSourceName">The name of the data source for this indexer.</param>
        /// <param name="updateInterval">An optional <see cref="TimeSpan"/> representing the update frequency for this indexer. Defaults to hourly if not specified.</param>
        /// <returns>The created indexer.</returns>
        Task<SearchIndexer> CreateIndexer(string indexerName, string indexName, string dataSourceName, TimeSpan? updateInterval = null);

        /// <summary>
        /// Wait for the given indexer to finish indexing documents.
        /// </summary>
        /// <param name="indexerName">The name of the indexer to wait for.</param>
        /// <param name="timeout">The maximum amount of time to wait.</param>
        /// <returns>If the indexing operation completed successfully within the allotted time, then true, otherwise false.</returns>
        Task<bool> WaitForIndexerCompletion(string indexerName, TimeSpan timeout);

        /// <summary>
        /// Get the indexer names with the prefix of the document we are setup for.
        /// </summary>
        /// <returns>A list of indexer names.</returns>
        Task<IList<string>> GetIndexerNames();

        /// <summary>
        /// Has the indexer ever ran (i.e. has it just been created).
        /// </summary>
        /// <typeparam name="T">The type of the indexer.</typeparam>
        /// <param name="hash">The hash.</param>
        /// <returns>
        /// True if it has ever ran.
        /// </returns>
        Task<bool> HasIndexerEverRan<T>(int hash)
            where T : class;

        /// <summary>
        /// Deletes an existing indexer.
        /// </summary>
        /// <param name="indexerName">The name of the indexer to delete.</param>
        /// <returns>An async Task containing the result of DeleteAsync(indexerName) if the indexer exists.</returns>
        Task DeleteIndexerIfExists(string indexerName);


        /// <summary>
        /// Get the Data Source names.
        /// </summary>
        /// <returns>A list of Data Source names.</returns>
        Task<IList<string>> GetDataSourceNames();

        /// <summary>
        /// Deletes an existing datasource.
        /// </summary>
        /// <param name="dataSourceName">The name of the data source to delete.</param>
        /// <returns>An async Task containing the result of DeleteAsync(dataSourceName) if the data source exists.</returns>
        Task DeleteDataSourceIfExists(string dataSourceName);
    }
}