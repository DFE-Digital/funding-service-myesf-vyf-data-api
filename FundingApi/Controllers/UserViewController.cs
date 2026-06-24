using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PDS.VYF.Data.Services.Abstracts.AppServices;
using PDS.VYF.Data.Services.Models.RequestModels;
using PDS.VYF.Data.Services.Models.ResponseModels;
using System.Threading.Tasks;

namespace FundingApi.Controllers
{
    /// <summary>
    /// The User view Controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class UserViewController : ControllerBase
    {
        private readonly IUserViewCountServices userViewCountServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserViewController"/> class.
        /// </summary>
        /// <param name="userViewCountServices">The user view count services.</param>
        public UserViewController(IUserViewCountServices userViewCountServices)
        {
            this.userViewCountServices = userViewCountServices;
        }

        /// <summary>
        /// Gets the user view count.
        /// </summary>
        /// <param name="userViewCountRequest">The user view count request.</param>
        /// <returns>The UserViewCountResponse.</returns>
        [HttpPost]
        [Route("GetUserViewCount")]
        public async Task<UserViewCountResponse> GetUserViewCount(UserViewCountRequestModel userViewCountRequest)
            => await this.userViewCountServices.GetUserViewCount(userViewCountRequest);

        /// <summary>
        /// Determines whether [has user visited] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="providerFundingId">The provider funding identifier.</param>
        /// <returns>
        ///   <c>true</c> if [has user visited] [the specified user identifier]; otherwise, <c>false</c>.
        /// </returns>
        [HttpGet]
        [Route("HasUserVisited/{userId}/{providerFundingId}")]
        public async Task<bool> HasUserVisited(string userId, string providerFundingId)
            => await this.userViewCountServices.HasUserVisited(userId, providerFundingId);

        /// <summary>
        /// Adds the provider funding identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="providerFundingId">The provider funding identifier.</param>
        /// <returns>
        ///   <c>true</c> if [Visited Info successfully updated.] <c>true</c> otherwise, <c>false</c>.
        /// </returns>
        [HttpPost]
        [Route("AddUserVisitedInfo/{userId}/{providerFundingId}")]
        public async Task<bool> AddUserVisitedInfo(string userId, string providerFundingId)
            => await this.userViewCountServices.AddUserVisitedInfo(userId, providerFundingId);
    }
}
