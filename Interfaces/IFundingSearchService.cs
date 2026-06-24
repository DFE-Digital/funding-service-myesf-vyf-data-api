using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Interfaces
{
    /// <summary>
    /// An interface providing methods for searching for funding.
    /// </summary>
    public interface IFundingSearchService
    {
        /// <summary>
        /// Search for latest provider funding.
        /// </summary>
        /// <param name="fundingStreamParameters">The funding streams to look up.</param>
        /// <param name="searchTerm">The search term to match.</param>
        /// <param name="waitForIndexBuild">Should we wait for the index to (re)build.</param>
        /// <returns>A response object containing the list of matching funding providers.</returns>
        Task<ISearchResult<IProviderFundingSearchDocument>> SearchLatestProviderFunding(FundingStreamParameters[] fundingStreamParameters, string searchTerm, bool waitForIndexBuild);

        /// <summary>
        /// Search for provider funding.
        /// </summary>
        /// <param name="fundingStreamParameters">The funding streams to look up.</param>
        /// <param name="searchTerm">The search term to match.</param>
        /// <param name="waitForIndexBuild">Should we wait for the index to (re)build.</param>
        /// <returns>A response object containing the list of matching funding providers.</returns>
        Task<ISearchResult<IProviderFundingSearchDocument>> SearchProviderFunding(FundingStreamParameters[] fundingStreamParameters, string searchTerm, bool waitForIndexBuild);

        /// <summary>
        /// Search for funding by group.
        /// </summary>
        /// <param name="fundingStreamParameters">The funding streams to look up.</param>
        /// <param name="searchTerm">The search term to match.</param>
        /// <param name="waitForIndexBuild">Should we wait for the index to (re)build.</param>
        /// <returns>A response object containing the list of matching groups. If only one group is found, the response also contains all of the
        /// fundings for provider fundings under that group.</returns>
        Task<ISearchResult<IFundingSearchDocument>> SearchFunding(FundingStreamParameters[] fundingStreamParameters, string searchTerm, bool waitForIndexBuild);

        /// <summary>
        /// Get the provider funding for a specific funding.
        /// </summary>
        /// <param name="parentId">The if of parent funding id to get provider funding for.</param>
        /// <param name="waitForIndexBuild">Should we await the index (re)build (if it necessary), or cancel with an error.</param>
        /// <returns>Funding providers that make up the funding.</returns>
        Task<ISearchResult<IProviderFundingSearchDocument>> GetProviderFunding(string parentId, bool waitForIndexBuild);

        /// <summary>
        /// Get a single instance of funding by its id.
        /// </summary>
        /// <param name="id">A funding id to lookup.</param>
        /// <param name="waitForIndexBuild">Should we await the index (re)build (if it necessary), or cancel with an error.</param>
        /// <returns>A single instance of funding, with its associated provider funding - or null.</returns>
        Task<ISearchResult<IFundingSearchDocument>> GetFunding(string id, bool waitForIndexBuild);

        /// <summary>
        /// Get a single instance of provider funding by its id.
        /// </summary>
        /// <param name="id">A provider funding id to lookup.</param>
        /// <param name="waitForIndexBuild">Should we await the index (re)build (if it necessary), or cancel with an error.</param>
        /// <returns>A single instance of provider funding.</returns>
        Task<IProviderFundingSearchDocument> GetProviderFundingForId(string id, bool waitForIndexBuild);
    }
}
