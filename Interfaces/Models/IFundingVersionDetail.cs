namespace PDS.ViewYourFunding.Data.Interfaces.Models
{
    /// <summary>
    /// The class used to hold Funding ID and its corresponding Statement version.
    /// </summary>
    public interface IFundingVersionDetail
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
