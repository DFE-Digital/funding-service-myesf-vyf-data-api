using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using PDS.ViewYourFunding.Data.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Services
{
    /// <summary>
    /// A class exposing operations of an Azure Search Index Client.
    /// </summary>
    public class AzureSearchIndexClient : IAzureSearchIndexClient
    {
        /// <summary>
        /// The search index client instance.
        /// </summary>
        private readonly SearchIndexClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchIndexClient"/> class.
        /// </summary>
        /// <param name="searchIndexClient">The search index client.</param>
        public AzureSearchIndexClient(SearchIndexClient searchIndexClient)
        {
            _client = searchIndexClient;
        }

        #region Implementation of IAzureSearchServiceClient

        /// <summary>
        /// Create an Azure Search index for the given model.
        /// </summary>
        /// <typeparam name="T">The type of the model on which to base the index fields.</typeparam>
        /// <param name="indexName">The name of the index to create.</param>
        /// <returns>The created SearchIndex.</returns>
        public async Task<Azure.Response<SearchIndex>> CreateIndex<T>(string indexName)
        {
            FieldBuilder fieldBuilder = new FieldBuilder();
            var definition = new SearchIndex(indexName, fieldBuilder.Build(typeof(T)));

            return await _client.CreateOrUpdateIndexAsync(definition);
        }

        /// <summary>
        /// Get the index names.
        /// </summary>
        /// <returns>A list of index names.</returns>
        public async Task<IList<string>> GetIndexNames()
        {
            return await Task.Run(() =>
                _client.GetIndexNamesAsync().ToBlockingEnumerable().ToList());
        }

        /// <summary>
        /// Deletes an existing index.
        /// </summary>
        /// <param name="indexName">The name of the index to delete.</param>
        /// <returns>An async Task containing the result of DeleteAsync(indexName) if the index exists.</returns>
        public async Task DeleteIndexIfExists(string indexName)
        {
            if (GetIndexNames().Result.Contains(indexName))
            {
                await _client.DeleteIndexAsync(indexName);
            }
        }

        #endregion
    }
}