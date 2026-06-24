using Azure;
using Azure.Search.Documents.Indexes.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Interfaces
{
    /// <summary>
    /// An interface exposing operations of an Azure Search Index Client.
    /// </summary>
    public interface IAzureSearchIndexClient
    {
        /// <summary>
        /// Create an Azure Search index for the given model.
        /// </summary>
        /// <typeparam name="T">The type of the model on which to base the index fields.</typeparam>
        /// <param name="indexName">The name of the index to create.</param>
        /// <returns>The created SearchIndex.</returns>
        Task<Response<SearchIndex>> CreateIndex<T>(string indexName);

        /// <summary>
        /// Get the index names with the prefix of the document we are setup for.
        /// </summary>
        /// <returns>A list of index names.</returns>
        Task<IList<string>> GetIndexNames();

        /// <summary>
        /// Deletes an existing index.
        /// </summary>
        /// <param name="indexName">The name of the index to delete.</param>
        /// <returns>An async Task containing the result of DeleteAsync(indexName) if the index exists.</returns>
        Task DeleteIndexIfExists(string indexName);
    }
}