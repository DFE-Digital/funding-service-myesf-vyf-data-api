using PDS.ViewYourFunding.Data.Interfaces.Models;

namespace PDS.ViewYourFunding.Data.Services.Models
{
    /// <summary>
    /// The class used to hold Funding ID and its corresponding Statement version.
    /// </summary>
    public class FundingVersionDetail : IFundingVersionDetail
    {
        /// <summary>
        /// Gets or sets the Funding/Provider Funding identifier.
        /// </summary>
        public string FundingId { get; set; }

        /// <summary>
        /// Gets or sets the Statement version.
        /// </summary>
        public int? StatementChannelVersion { get; set; }
    }
}
