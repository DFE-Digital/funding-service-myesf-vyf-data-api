using PDS.ViewYourFunding.Data.Interfaces.Models;

namespace PDS.ViewYourFunding.Data.Services.Models
{
    /// <inheritdoc cref="IUserFundingViewCount"/>
    public class UserFundingViewCount : IUserFundingViewCount
    {
        /// <inheritdoc/>
        public string UserId { get; set; }

        /// <inheritdoc/>
        public int UnreadNewFundings { get; set; }

        /// <inheritdoc/>
        public int UnreadUpdatedFundings { get; set; }
    }
}
