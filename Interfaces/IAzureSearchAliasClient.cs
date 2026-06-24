using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Interfaces
{
    /// <summary>
    /// An interface that exposes methods for querying an aliased Azure Search index.
    /// </summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    public interface IAzureSearchAliasClient<T>
        where T : class
    {
        /// <summary>
        /// Perform an Azure Search.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="searchOptions">The search parameters.</param>
        /// <returns>An object containing information about the result of the search.</returns>
        Task<SearchResults<T>> SearchDocumentsAsync(string searchText, SearchOptions searchOptions);

        /// <summary>
        /// Gets the number of documents in the Azure Search index.
        /// </summary>
        /// <returns>The number of documents in the Azure Search index.</returns>
        Task<long> CountDocumentsAsync();
    }
}