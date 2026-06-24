using PDS.ViewYourFunding.Data.API.Interfaces;

namespace FundingApi.DTOs
{
    /// <inheritdoc cref="IFundingApiAddUserVisitResponse"/>
    public class FundingApiAddUserVisitResponse : IFundingApiAddUserVisitResponse
    {
        /// <inheritdoc/>
        public bool Success { get; set; }
    }
}