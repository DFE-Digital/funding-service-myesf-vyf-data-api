namespace PDS.VYF.Data.Services.Abstracts.InfraServices
{
    using PDS.VYF.Data.Services.Enums;

    /// <summary>
    /// Represents the interface for searching services in Azure Search.
    /// </summary>
    public interface IAzSearchSearchingServices
    {
        /// <summary>
        /// Searches documents asynchronously in the specified Azure Search index.
        /// </summary>
        /// <typeparam name="T">The type of the documents to search.</typeparam>
        /// <param name="azSearchIndexType">The type of the Azure Search index.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="filters">The filters to apply.</param>
        /// <param name="selectFields">The fields to select.</param>
        /// <returns>An asynchronous enumerable of the search results.</returns>
        IAsyncEnumerable<T> SearchDocumentAsync<T>(AzSearchIndexTypeEnum azSearchIndexType, string searchText = "*", string? filters = null, string? selectFields = null);
    }
}
