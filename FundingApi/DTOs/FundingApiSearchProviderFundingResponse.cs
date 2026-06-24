using Newtonsoft.Json;
using PDS.ViewYourFunding.Data.API.Helpers;
using PDS.ViewYourFunding.Data.API.Interfaces;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.API.DTOs
{
    /// <summary>
    /// A class representing the response from a search request on the Funding API.
    /// </summary>
    public class FundingApiSearchProviderFundingResponse : IFundingApiSearchProviderFundingResponse
    {
        /// <summary>
        /// Gets or sets the collection of matching provider funding.
        /// </summary>
        [JsonConverter(typeof(ConcreteTypeConverter<IEnumerable<FundingApiSearchProviderFunding>>))]
        public IEnumerable<IFundingApiSearchProviderFunding> ProviderFunding { get; set; }
    }
}