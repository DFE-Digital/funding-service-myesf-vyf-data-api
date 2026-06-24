namespace PDS.VYF.Data.Services.Abstracts.AppServices
{
    using System.Collections.Generic;
    using PDS.VYF.Data.Services.Models.AzSearchModels;
    using PDS.VYF.Data.Services.Models.RequestModels;
    using PDS.VYF.Data.Services.Models.ResponseModels;

    /// <summary>
    /// The Comparison Services.
    /// </summary>
    public interface IComparisonServices
    {
        /// <summary>
        /// Gets the final statement previous year.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="childComparisonRequest">The child comparison request.</param>
        /// <param name="currentChildFunding">The current child funding.</param>
        /// <param name="childFundings">The child fundings.</param>
        /// <returns>The ChildComparisonResponse.</returns>
        ChildComparisonResponse? GetFinalStatementPreviousYear(string fundingStreamCode, ChildComparisonRequest childComparisonRequest, LoggedInChildAzSearchModel currentChildFunding, IEnumerable<LoggedInChildAzSearchModel> childFundings);

        /// <summary>
        /// Gets the previous statement current year.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="childComparisonRequest">The child comparison request.</param>
        /// <param name="currentChildFunding">The current child funding.</param>
        /// <param name="childFundings">The child fundings.</param>
        /// <returns>The ChildComparisonResponse.</returns>
        ChildComparisonResponse? GetPreviousStatementCurrentYear(string fundingStreamCode, ChildComparisonRequest childComparisonRequest, LoggedInChildAzSearchModel currentChildFunding, IEnumerable<LoggedInChildAzSearchModel> childFundings);

        /// <summary>
        /// Determines whether [is latest provider funding] [the specified current provider funding identifier].
        /// </summary>
        /// <param name="currentProviderFundingId">The current provider funding identifier.</param>
        /// <param name="childFundings">The child fundings.</param>
        /// <returns>
        ///   <c>true</c> if [is latest provider funding] [the specified current provider funding identifier]; otherwise, <c>false</c>.
        /// </returns>
        bool IsLatestProviderFunding(string currentProviderFundingId, IEnumerable<LoggedInChildAzSearchModel> childFundings);
    }
}
