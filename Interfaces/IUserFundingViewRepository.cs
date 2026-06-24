using PDS.ViewYourFunding.Data.Interfaces.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Interfaces
{
    /// <summary>
    /// Interface to operate on IUserFundingView type table entity.
    /// </summary>
    public interface IUserFundingViewRepository : IAzureTableStorageRepository<IUserFundingView>
    {
        /// <summary>
        /// Checks if user has already seen the funding or not.
        /// </summary>
        /// <param name="userId">The user id to lookup.</param>
        /// <param name="fundingId">The funding id to lookup.</param>
        /// <returns>True or false depending on whether user has visited funding or not.</returns>
        Task<bool> HasUserVisitedFunding(string userId, string fundingId);

        /// <summary>
        /// Returns the unread count of new and updated provider fundings for user.
        /// </summary>
        /// <param name="userId">The user id to lookup.</param>
        /// <param name="fundingVersionDetails">The funding ids to match on.</param>
        /// <returns>True or false depending on whether user has visited funding or not.</returns>
        Task<IUserFundingViewCount> GetUserFundingViewCount(string userId, IEnumerable<IFundingVersionDetail> fundingVersionDetails);
    }
}