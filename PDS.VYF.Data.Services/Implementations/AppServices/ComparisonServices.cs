namespace PDS.VYF.Data.Services.Implementations.AppServices
{
    using PDS.VYF.Data.Services.Abstracts.AppServices;
    using PDS.VYF.Data.Services.Enums;
    using PDS.VYF.Data.Services.Extensions;
    using PDS.VYF.Data.Services.Models.AzSearchModels;
    using PDS.VYF.Data.Services.Models.RequestModels;
    using PDS.VYF.Data.Services.Models.ResponseModels;

    /// <summary>
    /// The Comparison Services.
    /// </summary>
    public class ComparisonServices : IComparisonServices
    {

        /// <summary>
        /// Determines whether [is latest provider funding] [the specified current child funding].
        /// </summary>
        /// <param name="currentProviderFundingId">The current provider funding identifier.</param>
        /// <param name="childFundings">The child fundings.</param>
        /// <returns>
        ///   <c>true</c> if [is latest provider funding] [the specified current child funding]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLatestProviderFunding(string currentProviderFundingId, IEnumerable<LoggedInChildAzSearchModel> childFundings)
        {
            return childFundings
                .OrderByDescending(a => a.YearFrom)
                .ThenByDescending(a => a.FundingVersionInt)
                .ThenByDescending(a => a.StatusChangedDate)
                .FirstOrDefault()
                ?.Id == currentProviderFundingId;
        }

        /// <summary>
        /// Gets the previous statement current year.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="childComparisonRequest">The child comparison request.</param>
        /// <param name="currentChildFunding">The current child funding.</param>
        /// <param name="childFundings">The child fundings.</param>
        /// <returns>The ChildComparisonResponse.</returns>
        public ChildComparisonResponse? GetPreviousStatementCurrentYear(string fundingStreamCode, ChildComparisonRequest childComparisonRequest, LoggedInChildAzSearchModel currentChildFunding, IEnumerable<LoggedInChildAzSearchModel> childFundings)
        {

            var currentYearPreviousProivderFunding = childFundings
                                                        .Where(a => a.FundingStreamPeriod == childComparisonRequest.CurrentFundingStreamPeriodCode)
                                                        .OrderByDescending(a => a.FundingVersionInt)
                                                        .ThenByDescending(a => a.StatusChangedDate)
                                                        .Skip(1)
                                                        .FirstOrDefault();

            if (currentYearPreviousProivderFunding == null || currentYearPreviousProivderFunding.Id == null)
            {
                return null;
            }

            if (fundingStreamCode.Equals("GAG", StringComparison.OrdinalIgnoreCase)
                && !this.IsValidPreviousStatementCurrentYearForGAG(fundingStreamCode, childComparisonRequest, currentChildFunding, currentYearPreviousProivderFunding))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(childComparisonRequest.ParentUKPRN)
                && childComparisonRequest.ChildUKPRN != childComparisonRequest.ParentUKPRN
                && currentYearPreviousProivderFunding?.ParentInfo?.Any(a => a.ParentUKPRN == childComparisonRequest.ParentUKPRN) != true)
            {
                return null;
            }

            return new ChildComparisonResponse
            {
                ProviderFundingId = currentYearPreviousProivderFunding.Id,
                ComparisonType = ComparisonTypeEnum.PreviousStatementCurrentYear,
                FundingStreamPeriodCode = childComparisonRequest.CurrentFundingStreamPeriodCode,
                StatusChangedDateOnly = currentYearPreviousProivderFunding.StatusChangedDateOnly!.Value,
            };
        }

        /// <summary>
        /// Gets the final statement previous year.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="childComparisonRequest">The child comparison request.</param>
        /// <param name="currentChildFunding">The current child funding.</param>
        /// <param name="childFundings">The child fundings.</param>
        /// <returns>The ChildComparisonResponse.</returns>
        public ChildComparisonResponse? GetFinalStatementPreviousYear(string fundingStreamCode, ChildComparisonRequest childComparisonRequest, LoggedInChildAzSearchModel currentChildFunding, IEnumerable<LoggedInChildAzSearchModel> childFundings)
        {
            var previousFundingStreamPeriodCode = childComparisonRequest.CurrentFundingStreamPeriodCode.GetPreviousFundingStreamPeriodCode();

            var lastYearFinalProivderFunding = childFundings
                                                        .FirstOrDefault(a => a.FundingStreamPeriod == previousFundingStreamPeriodCode && a.IsLatest == true);

            if (lastYearFinalProivderFunding == null || lastYearFinalProivderFunding.Id == null)
            {
                return null;
            }

            if (fundingStreamCode.Equals("GAG", StringComparison.OrdinalIgnoreCase)
                && !this.IsValidFinalStatementPreviousYearForGAG(fundingStreamCode, childComparisonRequest, currentChildFunding, lastYearFinalProivderFunding))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(childComparisonRequest.ParentUKPRN)
                && childComparisonRequest.ChildUKPRN != childComparisonRequest.ParentUKPRN
                && lastYearFinalProivderFunding?.ParentInfo?.Any(a => a.ParentUKPRN == childComparisonRequest.ParentUKPRN) != true)
            {
                return null;
            }

            return new ChildComparisonResponse
            {
                ProviderFundingId = lastYearFinalProivderFunding.Id,
                ComparisonType = ComparisonTypeEnum.FinalStatementPreviousYear,
                FundingStreamPeriodCode = lastYearFinalProivderFunding.FundingStreamPeriod!,
                StatusChangedDateOnly = lastYearFinalProivderFunding.StatusChangedDateOnly!.Value,
            };
        }

        private bool IsValidPreviousStatementCurrentYearForGAG(string fundingStreamCode, ChildComparisonRequest childComparisonRequest, LoggedInChildAzSearchModel currentChildFunding, LoggedInChildAzSearchModel currentYearPreviousProivderFunding)
        {
            if (currentChildFunding.InYearOpener != currentYearPreviousProivderFunding.InYearOpener)
            {
                return false;
            }

            if (currentChildFunding.IsIndicative != currentYearPreviousProivderFunding.IsIndicative)
            {
                return false;
            }

            return true;
        }

        private bool IsValidFinalStatementPreviousYearForGAG(string fundingStreamCode, ChildComparisonRequest childComparisonRequest, LoggedInChildAzSearchModel currentChildFunding, LoggedInChildAzSearchModel lastYearFinalProivderFunding)
        {
            if (currentChildFunding.InYearOpener != lastYearFinalProivderFunding.InYearOpener)
            {
                return false;
            }

            if (currentChildFunding.IsIndicative != lastYearFinalProivderFunding.IsIndicative)
            {
                return false;
            }

            if (currentChildFunding.IsIndicative == false
                && currentChildFunding.InYearOpener == true)
            {
                return false;
            }

            if (currentChildFunding.IsIndicative == true
                    && currentChildFunding.StatementType != "New")
            {
                return false;
            }

            return true;
        }
    }
}
