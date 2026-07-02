using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PDS.ViewYourFunding.Data.API.Helpers;
using PDS.VYF.Data.Services.Abstracts.AppServices;
using PDS.VYF.Data.Services.Models.AzSearchModels;
using PDS.VYF.Data.Services.Models.RequestModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FundingApi.Controllers
{
    /// <summary>
    /// The New Funding Controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Authorize(Policy = nameof(ToggleAuthorizeRequirement))]
    [Route("api/[controller]")]
    [ApiController]
    public class ParentController : ControllerBase
    {
        private readonly IParentSearchServices parentSearchServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentController" /> class.
        /// </summary>
        /// <param name="parentSearchServices">The parent search services.</param>
        public ParentController(IParentSearchServices parentSearchServices)
        {
            this.parentSearchServices = parentSearchServices;
        }

        /// <summary>
        /// Searches the funding.
        /// </summary>
        /// <param name="parentRequest">The parent request.</param>
        /// <returns>List of LoggedInFundingAzSearchModel.</returns>
        [HttpPost("SearchParent")]
        public async Task<List<LoggedInParentAzSearchModel>> SearchParent(ParentRequest parentRequest)
                    => await parentSearchServices.SearchParent(parentRequest, default);

        /// <summary>
        /// Determines whether the specified ukprn is parent.
        /// </summary>
        /// <param name="ukprn">The ukprn.</param>
        /// <param name="fundingStreamPeriod">The funding stream period.</param>
        /// <returns>
        ///   <c>true</c> if the specified ukprn is parent; otherwise, <c>false</c>.
        /// </returns>
        [HttpPost("IsParent/{ukprn}")]
        public async Task<bool> IsParent(string ukprn, [FromBody] List<string> fundingStreamPeriod)
                    => await parentSearchServices.IsParent(ukprn, fundingStreamPeriod, default);

        /// <summary>
        /// Determines whether [is my child] [the specified parent ukprn].
        /// </summary>
        /// <param name="parentUkprn">The parent ukprn.</param>
        /// <param name="childUkprn">The child ukprn.</param>
        /// <param name="fundingStreamPeriod">The funding stream period.</param>
        /// <returns>
        ///   <c>true</c> if [is my child] [the specified parent ukprn]; otherwise, <c>false</c>.
        /// </returns>
        [HttpPost("IsMyChild/{parentUkprn}/{childUkprn}")]
        public async Task<bool> IsMyChild(string parentUkprn, string childUkprn, [FromBody] List<string> fundingStreamPeriod)
                    => await parentSearchServices.IsMyChild(parentUkprn, childUkprn, fundingStreamPeriod, default);
    }
}
