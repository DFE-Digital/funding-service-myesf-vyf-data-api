namespace PDS.VYF.Data.Services.Implementations.AppServices
{
    using Azure.Data.Tables;
    using PDS.VYF.Data.Services.Abstracts.AppServices;
    using PDS.VYF.Data.Services.Abstracts.InfraServices;
    using PDS.VYF.Data.Services.Models.RequestModels;
    using PDS.VYF.Data.Services.Models.ResponseModels;

    /// <summary>
    /// The class for User View Count.
    /// </summary>
    /// <seealso cref="PDS.VYF.Data.Services.Abstracts.AppServices.IUserViewCountServices" />
    public class UserViewCountServices : IUserViewCountServices
    {
        private readonly IAzTableServices azTableServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserViewCountServices"/> class.
        /// </summary>
        /// <param name="azTableServices">The az table services.</param>
        public UserViewCountServices(IAzTableServices azTableServices)
        {
            this.azTableServices = azTableServices;
        }

        /// <summary>
        /// Gets the user view count.
        /// </summary>
        /// <param name="userViewCountRequest">The user view count request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<UserViewCountResponse> GetUserViewCount(UserViewCountRequestModel userViewCountRequest)
        {
            if (userViewCountRequest.ChildStatements == null || userViewCountRequest.ChildStatements.Count == 0)
            {
                return new UserViewCountResponse();
            }

            if (userViewCountRequest.ChildStatements.Count == 1)
            {
                var isViewed = await this.azTableServices.IsUserViewedProviderFundingId(userViewCountRequest.UserId, userViewCountRequest.ChildStatements[0].ChildId);

                return new UserViewCountResponse()
                {
                    NewCount = !isViewed && userViewCountRequest.ChildStatements[0].StatementType == "New" ? 1 : 0,
                    UpdatedCount = !isViewed && userViewCountRequest.ChildStatements[0].StatementType == "Updated" ? 1 : 0,
                };
            }

            var viewedIds = await this.azTableServices.GetViewChildrenIdAsync(userViewCountRequest.UserId);

            return new UserViewCountResponse()
            {
                NewCount = userViewCountRequest.ChildStatements.Count(a => a.StatementType == "New" && !viewedIds.ContainsKey(a.ChildId)),
                UpdatedCount = userViewCountRequest.ChildStatements.Count(a => a.StatementType == "Updated" && !viewedIds.ContainsKey(a.ChildId)),
            };
        }

        /// <summary>
        /// Determines whether [has user visited] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="providerFundingId">The provider funding identifier.</param>
        /// <returns>
        ///   <c>true</c> if [has user visited] [the specified user identifier]; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> HasUserVisited(string userId, string providerFundingId)
        {
            return await this.azTableServices.IsUserViewedProviderFundingId(userId, providerFundingId);
        }

        /// <summary>
        /// Adds the user visited information.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="providerFundingId">The provider funding identifier.</param>
        /// <returns>
        ///   <c>true</c> if [User Added successfully] <c>true</c>; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> AddUserVisitedInfo(string userId, string providerFundingId)
        {
            if (!await this.azTableServices.IsUserViewedProviderFundingId(userId, providerFundingId))
            {
                var entity = new TableEntity(userId, providerFundingId)
                {
                    { "ViewedAt", DateTime.UtcNow }
                };

                return await this.azTableServices.UpsertEntityAsync(entity);
            }

            return true;
        }
    }
}
