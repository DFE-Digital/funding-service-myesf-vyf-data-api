using Newtonsoft.Json;
using PDS.ViewYourFunding.Data.API.Helpers;
using PDS.ViewYourFunding.Data.API.Interfaces;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.API.DTOs
{
    /// <summary>
    /// A class representing the response from a search request on the Funding API.
    /// </summary>
    public class FundingApiSearchFundingResponse : IFundingApiSearchFundingResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FundingApiSearchFundingResponse"/> class.
        /// </summary>
        public FundingApiSearchFundingResponse()
        {
            Funding = new List<FundingApiSearchFunding>();
        }

        /// <summary>
        /// Gets or sets the collection of matching funding.
        /// </summary>
        [JsonConverter(typeof(ConcreteTypeConverter<IEnumerable<FundingApiSearchFunding>>))]
        public IEnumerable<IFundingApiSearchFunding> Funding { get; set; }
    }
}