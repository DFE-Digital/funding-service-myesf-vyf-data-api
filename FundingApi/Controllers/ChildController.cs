using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PDS.ViewYourFunding.Data.API.Helpers;
using PDS.VYF.Data.Services.Abstracts.AppServices;
using PDS.VYF.Data.Services.Enums;
using PDS.VYF.Data.Services.Implementations.AppServices;
using PDS.VYF.Data.Services.Models.AzSearchModels;
using PDS.VYF.Data.Services.Models.RequestModels;
using PDS.VYF.Data.Services.Models.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FundingApi.Controllers
{
    /// <summary>
    /// The Child Controller.
    /// </summary>
    [Authorize(Policy = nameof(ToggleAuthorizeRequirement))]
    [Route("api/[controller]")]
    [ApiController]
    public class ChildController
    {
        private readonly IChildSearchServices childSearchServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildController"/> class.
        /// </summary>
        /// <param name="childSearchServices">The child search services.</param>
        public ChildController(IChildSearchServices childSearchServices)
        {
            this.childSearchServices = childSearchServices;
        }

        /// <summary>
        /// Searches the child.
        /// </summary>
        /// <param name="childRequest">The child request.</param>
        /// <returns>List of LoggedInChildAzSearchModel.</returns>
        [HttpPost("SearchChild")]
        public async Task<List<LoggedInChildAzSearchModel>> SearchChild(ChildRequest childRequest)
                    => await childSearchServices.SearchChild(childRequest, default);

        /// <summary>
        /// Searches the latest funding period.
        /// </summary>
        /// <param name="childRequest">The child request.</param>
        /// <returns>List of latest funding period.</returns>
        [HttpPost("LatestFundingPeriod")]
        public async Task<List<string>> LatestFundingPeriod(ChildRequest childRequest)
                    => await childSearchServices.LatestFundingPeriod(childRequest, default);

        /// <summary>
        /// Determines whether [is latest statement] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="childRequest">The child request.</param>
        /// <returns>
        ///   <c>true</c> if [is latest statement] [the specified identifier]; otherwise, <c>false</c>.
        /// </returns>
        [HttpPost("IsLatestStatement/{id}")]
        public async Task<bool> IsLatestStatement(string id, [FromBody] ChildRequest childRequest)
                    => await childSearchServices.IsLatestStatement(id, childRequest, default);

        /// <summary>
        /// Searches current children of Parent.
        /// </summary>
        /// <param name="parentUkprn">The parent ukprn.</param>
        /// <param name="childUkprn">The child UKPRNs for parent.</param>
        /// <returns>
        ///   <c>true</c> if [is latest statement] [the specified identifier]; otherwise, <c>false</c>.
        /// </returns>
        [HttpPost("GetCurrentChildUkprnsForParent/{parentUkprn}")]
        public async Task<List<string>> GetCurrentChildUkprnsForParent(string parentUkprn, [FromBody] List<string> childUkprn)
                    => await childSearchServices.GetCurrentChildUkprnsForParent(parentUkprn, childUkprn, default);

        /// <summary>
        /// Searches the children of a parent.
        /// </summary>
        /// <param name="parentUkprn">The parent ukprn.</param>
        /// <param name="childRequest">The child request.</param>
        /// <returns>List of LoggedInChildAzSearchModel.</returns>
        [HttpPost("SearchChildrenOfAParent/{parentUkprn}")]
        public async Task<List<LoggedInChildAzSearchModel>> SearchChildrenOfAParent(string parentUkprn, [FromBody] ChildRequest childRequest)
                    => await childSearchServices.SearchChildrenOfAParent(parentUkprn, childRequest, default);

        /// <summary>
        /// Gets the child comparison.
        /// </summary>
        /// <param name="childComparisonRequest">The child comparison request.</param>
        /// <returns>List of ChildComparisonResponse.</returns>
        [HttpPost("GetChildComparison")]
        public async Task<Dictionary<ComparisonTypeEnum, ChildComparisonResponse>> GetChildComparison(ChildComparisonRequest childComparisonRequest)
            => await childSearchServices.GetChildComparison(childComparisonRequest, default);
    }
}
