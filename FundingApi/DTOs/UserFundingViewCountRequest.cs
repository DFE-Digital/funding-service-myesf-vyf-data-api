using PDS.ViewYourFunding.Data.Services.Models;
using System.Collections.Generic;

namespace FundingApi.DTOs
{
    /// <summary>
    /// Request for number of unread new and updated provider fundings for a user.
    /// </summary>
    public class UserFundingViewCountRequest
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the funding ids available for the user.
        /// </summary>
        public List<FundingVersionDetail> FundingVersionDetails { get; set; }
    }
}
