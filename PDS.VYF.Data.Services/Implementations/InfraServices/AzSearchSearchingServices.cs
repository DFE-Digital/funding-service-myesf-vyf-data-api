namespace PDS.VYF.Data.Services.Implementations.InfraServices
{
    using Azure.Search.Documents;
    using Azure.Search.Documents.Indexes;
    using PDS.VYF.Data.Services.Abstracts.InfraServices;
    using PDS.VYF.Data.Services.Enums;
    using PDS.VYF.Data.Services.Extensions;

    /// <summary>
    /// The service class for Azure Search Queries.
    /// </summary>
    /// <seealso cref="PDS.VYF.Data.Services.Abstracts.InfraServices.IAzSearchSearchingServices" />
    public class AzSearchSearchingServices : IAzSearchSearchingServices
    {
        private readonly SearchIndexClient searchIndexClient;
        private readonly IAzSearchCosmosServices azSearchCosmosServices;

        private readonly Dictionary<string, SearchClient> appSearchClients = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="AzSearchSearchingServices" /> class.
        /// </summary>
        /// <param name="azSearchCosmosServices">The az search cosmos services.</param>
        /// <param name="searchIndexClient">The search index client.</param>
        public AzSearchSearchingServices(IAzSearchCosmosServices azSearchCosmosServices, SearchIndexClient searchIndexClient)
        {
            this.azSearchCosmosServices = azSearchCosmosServices;
            this.searchIndexClient = searchIndexClient;
        }

        /// <summary>
        /// Searches the document asynchronous.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="azSearchIndexType">Type of the az search index.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="selectFields">The select fields.</param>
        /// <returns>The Async enumerable of Search documents.</returns>
        public async IAsyncEnumerable<T> SearchDocumentAsync<T>(AzSearchIndexTypeEnum azSearchIndexType, string searchText = "*", string? filters = null, string? selectFields = null)
        {
            string containerName = this.azSearchCosmosServices.GetContainerName(azSearchIndexType);
            string indexName = azSearchIndexType.GetIndexName(containerName);

            SearchOptions searchOptions = new ()
            {
                Filter = filters
            };

            if (!string.IsNullOrWhiteSpace(selectFields))
            {
                foreach (var item in selectFields.Split(","))
                {
                    searchOptions.Select.Add(item);
                }
            }

            var searchClient = this.GetSearchClient(indexName);
            var response = await searchClient.SearchAsync<T>(searchText, searchOptions);

            await foreach (var item in response.Value.GetResultsAsync())
            {
                yield return item.Document;
            }
        }

        private T GetValueFromDictionary<T>(Dictionary<string, T> dic, string key, Func<T> generator)
        {
            if (dic.TryGetValue(key, out T? value))
            {
                return value;
            }

            return generator();
        }

        private SearchClient GetSearchClient(string indexName)
        {
            return this.GetValueFromDictionary(this.appSearchClients, indexName, () =>
            {
                var value = this.searchIndexClient.GetSearchClient(indexName);
                this.appSearchClients.TryAdd(indexName, value);
                return value;
            });
        }
    }
}
