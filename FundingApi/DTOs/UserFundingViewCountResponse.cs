namespace FundingApi.DTOs
{
    /// <summary>
    /// Response representing number of unread new and updated provider fundings for a user.
    /// </summary>
    public class UserFundingViewCountResponse
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
