namespace PDS.VYF.Data.Services.Abstracts.AppServices
{
    using PDS.VYF.Data.Services.Models.RequestModels;
    using PDS.VYF.Data.Services.Models.ResponseModels;

    /// <summary>
    /// Interface for user view count services.
    /// </summary>
    public interface IUserViewCountServices
    {
        /// <summary>
        /// Adds user visited information.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="providerFundingId">The provider funding identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<bool> AddUserVisitedInfo(string userId, string providerFundingId);

        /// <summary>
        /// Gets the user view count.
        /// </summary>
        /// <param name="userViewCountRequest">The user view count request model.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<UserViewCountResponse> GetUserViewCount(UserViewCountRequestModel userViewCountRequest);

        /// <summary>
        /// Checks if the user has visited.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="providerFundingId">The provider funding identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<bool> HasUserVisited(string userId, string providerFundingId);
    }
}
