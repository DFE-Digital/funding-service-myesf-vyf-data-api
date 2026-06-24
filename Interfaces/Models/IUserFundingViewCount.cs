namespace PDS.ViewYourFunding.Data.Interfaces.Models
{
    /// <summary>
    /// An interface representing visit count of users for funding statements.
    /// </summary>
    public interface IUserFundingViewCount
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the count for the number of unread new fundings.
        /// </summary>
        public int UnreadNewFundings { get; set; }

        /// <summary>
        /// Gets or sets the count for the number of unread updated fundings.
        /// </summary>
        public int UnreadUpdatedFundings { get; set; }
    }
}