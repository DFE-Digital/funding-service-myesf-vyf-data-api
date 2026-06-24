namespace PDS.ViewYourFunding.Data.API.DTOs
{
    /// <summary>
    /// A colection of all the things to limit to.
    /// </summary>
    public class FundingApiSearchRequest
    {
        /// <summary>
        /// Gets or sets the funding streams to limit to.
        /// </summary>
        public FundingApiSearchFundingStreamParameters[] FundingStreams { get; set; }

        /// <summary>
        /// Gets or sets the search term to limit to.
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether should we wait for an index (re)build or fail if its re-building.
        /// </summary>
        public bool WaitForIndexBuild { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to bypass the grouping and return all versions.
        /// </summary>
        public bool BypassGrouping { get; set; }
    }
}