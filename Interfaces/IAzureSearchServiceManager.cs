using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Interfaces
{
    /// <summary>
    /// Interface for managing an Azure Search service.
    /// </summary>
    public interface IAzureSearchServiceManager
    {
        /// <summary>
        /// Has the indexer ever ran.
        /// </summary>
        /// <param name="hash">The hash to look for.</param>
        /// <typeparam name="T">Type of search document.</typeparam>
        /// <returns>True if it has ever been ran, false if it has not been ran (is probably new).</returns>
        Task<bool> HasIndexerEverRan<T>(int hash)
            where T : class;

        /// <summary>
        /// Does an index and indexer exist with the passed hash.
        /// </summary>
        /// <param name="hash">A hash of the class fields.</param>
        /// <typeparam name="T">Type of search document.</typeparam>
        /// <returns>True if it exists, false if not.</returns>
        Task<bool> DoesIndexAndIndexerExist<T>(int hash)
            where T : class;

        /// <summary>
        /// Rebuild the index for the given type, using the given Cosmos DB data source.
        /// </summary>
        /// <typeparam name="T">The model representing the structure of the indexed documents.</typeparam>
        /// <param name="cosmosDbDataSourceParameters">The parameters configuring the Cosmos DB data source.</param>
        /// <returns>An async task.</returns>
        Task RebuildIndex<T>(CosmosDbDataSourceParameters cosmosDbDataSourceParameters)
            where T : class;

        /// <summary>
        /// Get the index names with the prefix of the document we are setup for.
        /// </summary>
        /// <returns>A list of index names.</returns>
        Task<IList<string>> GetAllIndexNames();

        /// <summary>
        /// Get the indexer names with the prefix of the document we are setup for.
        /// </summary>
        /// <returns>A list of indexer names.</returns>
        Task<IList<string>> GetAllIndexerNames();

        /// <summary>
        /// Deletes all indexes, indexers and data sources matching the given prefix, except those in the respective '...ToKeep' parameters.
        /// </summary>
        /// <param name="prefixToDelete">The prefix of the names of components that will be deleted.</param>
        /// <returns>The list of deleted index names.</returns>
        Task<List<string>> DeleteComponents(string prefixToDelete);
    }
}