using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.API.Interfaces
{
    /// <summary>
    /// An interface representing the response from a provider funding search request on the Fundings API.
    /// </summary>
    public interface IFundingApiSearchProviderFundingResponse
    {
        /// <summary>
        /// Gets or sets the collection of matching provider fundings.
        /// </summary>
        IEnumerable<IFundingApiSearchProviderFunding> ProviderFunding { get; set; }
    }
}
