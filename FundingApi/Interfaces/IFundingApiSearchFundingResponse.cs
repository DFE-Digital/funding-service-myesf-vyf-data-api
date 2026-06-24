using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.API.Interfaces
{
    /// <summary>
    /// An interface representing the response from a search request on the Fundings API.
    /// </summary>
    public interface IFundingApiSearchFundingResponse
    {
        /// <summary>
        /// Gets or sets the collection of matching funding.
        /// </summary>
        IEnumerable<IFundingApiSearchFunding> Funding { get; set; }
    }
}
