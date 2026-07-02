namespace PDS.VYF.Data.Services.Implementations.AppServices
{
    using PDS.VYF.Data.Services.Abstracts.AppServices;
    using PDS.VYF.Data.Services.Abstracts.InfraServices;
    using PDS.VYF.Data.Services.Constants;
    using PDS.VYF.Data.Services.Enums;
    using PDS.VYF.Data.Services.Extensions;
    using PDS.VYF.Data.Services.Models.AzSearchModels;
    using PDS.VYF.Data.Services.Models.RequestModels;
    using PDS.VYF.Data.Services.Models.ResponseModels;
    using System.Globalization;

    /// <summary>
    /// The Service class for Child Search API requests.
    /// </summary>
    /// <seealso cref="PDS.VYF.Data.Services.Implementations.AppServices.SearchServicesBase" />
    /// <seealso cref="PDS.VYF.Data.Services.Abstracts.AppServices.IChildSearchServices" />
    public class ChildSearchServices : SearchServicesBase, IChildSearchServices
    {
        private readonly IAzSearchSearchingServices azSearchSearchingServices;
        private readonly IInYearOpenerCalcServices inYearOpenerCalcServices;
        private readonly IComparisonServices comparisonServices;
        private readonly AzSearchIndexTypeEnum childAzSearchIndexType = AzSearchIndexTypeEnum.LoggedIn_Child;
        private readonly AzSearchIndexTypeEnum parentAzSearchIndexType = AzSearchIndexTypeEnum.LoggedIn_Parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildSearchServices" /> class.
        /// </summary>
        /// <param name="azSearchSearchingServices">The az search searching services.</param>
        /// <param name="inYearOpenerCalcServices">The in year opener calculate services.</param>
        public ChildSearchServices(IAzSearchSearchingServices azSearchSearchingServices, IInYearOpenerCalcServices inYearOpenerCalcServices, IComparisonServices comparisonServices)
        {
            this.azSearchSearchingServices = azSearchSearchingServices;
            this.inYearOpenerCalcServices = inYearOpenerCalcServices;
            this.comparisonServices = comparisonServices;
        }

        /// <summary>
        /// Searches the child.
        /// </summary>
        /// <param name="childRequest">The child request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<List<LoggedInChildAzSearchModel>?> SearchChild(ChildRequest childRequest, CancellationToken cancellationToken)
        {
            var filter = childRequest.BuildFilter().ToString();

            if (childRequest.SelectFields?.Count > 0 && (childRequest.HasToBeLatestFunding || childRequest.DoFindIsLatest))
            {
                childRequest.TryAddSelectFields(a => new { a.OrganisationUkprn, a.FundingStreamPeriod, a.FundingVersionInt, a.StatusChangedDate, });
            }

            if (childRequest.SelectFields?.Count > 0
                && (childRequest.HasIYOToBeRemoved
                        || childRequest.SelectFields?.Contains(nameof(LoggedInChildAzSearchModel.InYearOpener)) == true))
            {
                childRequest.TryAddSelectFields(a => new { a.OpenReason, a.CalculationsForSummary, a.FundingPeriodCode, a.DateOpened, a.IsIndicative, a.YearFrom, a.YearTo, a.CloseReason, a.DateClosed });
            }

            var result = await this.azSearchSearchingServices
                                .SearchDocumentAsync<LoggedInChildAzSearchModel>(this.childAzSearchIndexType, "*", filter, childRequest.Select)
                                .ToListAsync(cancellationToken: cancellationToken);

            if (childRequest.HasIYOToBeRemoved || string.IsNullOrWhiteSpace(childRequest.Select) || childRequest.SelectFields?.Contains(nameof(LoggedInChildAzSearchModel.InYearOpener)) == true)
            {
                result.ForEach(a => a.InYearOpener = this.inYearOpenerCalcServices.IsInYearOpener(a));
            }

            if (childRequest.DoFindIsLatest)
            {
                result.GroupBy(
                            a => new { a.OrganisationUkprn, a.FundingStreamPeriod, },
                            (key, groupedValues) => groupedValues
                                    .OrderByDescending(a => a.FundingVersionInt)
                                    .ThenBy(a => a.StatusChangedDate)
                                    .First()).ToList().ForEach(a => a.IsLatest = true);
            }

            if (childRequest.HasIYOToBeRemoved == true)
            {
                result.RemoveAll(a => a.InYearOpener == true);
            }

            if (childRequest.HasToBeLatestFunding)
            {
                if (childRequest.DoFindIsLatest)
                {
                    return result.Where(a => a.IsLatest == true && !childRequest.HasIYOToBeRemoved).ToList();
                }

                return result.GroupBy(
                                a => new { a.OrganisationUkprn, a.FundingStreamPeriod, },
                                (key, groupedValues) => groupedValues
                                        .OrderByDescending(a => a.FundingVersionInt)
                                        .ThenBy(a => a.StatusChangedDate)
                                        .First()).ToList();
            }

            return result;
        }

        /// <summary>
        /// Searches the latest funding stream period.
        /// </summary>
        /// <param name="childRequest">The child request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<List<string?>> LatestFundingPeriod(ChildRequest childRequest, CancellationToken cancellationToken)
        {
            var filter = childRequest.BuildFilter().ToString();
            childRequest.HasIYOToBeRemoved = false;

            childRequest.SetSelectFields(a => new { a.OrganisationUkprn, a.FundingStreamPeriod, a.FundingVersionInt, a.Id, a.StatusChangedDate, a.YearFrom, a.YearTo });

            var result = await this.azSearchSearchingServices
                                .SearchDocumentAsync<LoggedInChildAzSearchModel>(this.childAzSearchIndexType, "*", filter, childRequest.Select)
                                .ToListAsync(cancellationToken: cancellationToken);

            var latestPeriodCode = new List<string?>();

            latestPeriodCode = result.GroupBy(
                      a => new { a.OrganisationUkprn },
                      (key, groupedValues) => groupedValues
                              .OrderByDescending(a => a.YearFrom)
                              .ThenBy(a => a.StatusChangedDate).ThenBy(a => a.FundingVersionInt)
                              .First()).Select(b => b.FundingStreamPeriod).ToList();

            return latestPeriodCode;
        }

        /// <summary>
        /// Determines whether the specified statement is the latest.
        /// </summary>
        /// <param name="id">The identifier of the statement.</param>
        /// <param name="childRequest">The child request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   <c>true</c> if the specified statement is the latest; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsLatestStatement(string id, ChildRequest childRequest, CancellationToken cancellationToken)
        {
            var filter = childRequest.BuildFilter().ToString();

            // childRequest.HasIYOToBeRemoved = false;
            childRequest.SetSelectFields(a => new { a.OrganisationUkprn, a.FundingStreamPeriod, a.FundingVersionInt, a.Id, a.StatusChangedDate });

            var result = await this.azSearchSearchingServices
                                .SearchDocumentAsync<LoggedInChildAzSearchModel>(this.childAzSearchIndexType, "*", filter, childRequest.Select)
                                .ToListAsync(cancellationToken: cancellationToken);

            var latestStatementsForAYear = result.GroupBy(
                                                    a => new { a.OrganisationUkprn, a.FundingStreamPeriod, },
                                                    (key, groupedValues) => groupedValues
                                                            .OrderByDescending(a => a.FundingVersionInt)
                                                            .ThenBy(a => a.StatusChangedDate)
                                                            .First());

            return latestStatementsForAYear.Any(a => a.Id == id);
        }

        /// <summary>
        /// Send current latest child UKPRNs.
        /// </summary>
        /// <param name="parentUkprn">The current logged in parent UKPRN.</param>
        /// <param name="childUkprn">The child ukprns.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   <c>true</c> if the specified statement is the latest; otherwise, <c>false</c>.
        /// </returns>
        public async Task<List<string>?> GetCurrentChildUkprnsForParent(string parentUkprn, List<string> childUkprn, CancellationToken cancellationToken)
        {
            var childRequest = new ChildRequest
            {
                HasToBeLatestFunding = true,
                ListOfUKPRNs = childUkprn,
            };

            var filter = childRequest.BuildFilter().ToString();

            childRequest.SetSelectFields(a => new
            {
                a.OrganisationUkprn,
                a.FundingStreamPeriod,
                a.FundingVersionInt,
                a.Id,
                a.StatusChangedDate,
                a.ParentInfo,
                a.YearFrom
            });

            var result = await this.azSearchSearchingServices
                .SearchDocumentAsync<LoggedInChildAzSearchModel>(
                    this.childAzSearchIndexType,
                    "*",
                    filter,
                    childRequest.Select)
                .ToListAsync(cancellationToken);

            var latestYearFrom = result
            .Where(r => r.YearFrom.HasValue)
            .Max(r => r.YearFrom);

            var validResults = result
            .Where(r =>
                r.YearFrom == latestYearFrom ||
                r.YearFrom == latestYearFrom - 1);

            var childrenForThisParent = validResults
                .GroupBy(c => c.OrganisationUkprn)
                .Select(g => g
                    .OrderByDescending(c => c.StatusChangedDate)
                    .ThenByDescending(c => c.FundingVersionInt)
                    .First())
                .Where(c =>
                    c.ParentInfo != null &&
                    c.ParentInfo.Any(p => p.ParentUKPRN == parentUkprn))
                .Select(c => c.OrganisationUkprn)
                .Distinct()
                .ToList();

            return childrenForThisParent;
        }

        /// <summary>
        /// Searches the children of a parent.
        /// </summary>
        /// <param name="parentUkprn">The parent ukprn.</param>
        /// <param name="childRequest">The child request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<List<LoggedInChildAzSearchModel>?> SearchChildrenOfAParent(string parentUkprn, ChildRequest childRequest, CancellationToken cancellationToken)
        {
            ParentRequest parentRequest = new()
            {
                ListOfUKPRNs = new() { parentUkprn },
                FundingStreamPeriods = childRequest.FundingStreamPeriods,
                HasToBeLatestFunding = true
            };

            parentRequest.SetSelectFields(a => new { a.GroupUkprn, a.FundingStreamPeriod, a.FundingStreamGroupingTypeReason, a.ChildUKPRNs, a.ProviderFundings, a.FundingVersionInt, });

            var parentFilter = parentRequest.BuildFilter().ToString();

            var parents = await this.azSearchSearchingServices
                                    .SearchDocumentAsync<LoggedInParentAzSearchModel>(this.parentAzSearchIndexType, "*", parentFilter, parentRequest.Select)
                                    .ToListAsync(cancellationToken: cancellationToken);

            parents = parents.GroupBy(
                            a => new { a.GroupUkprn, a.FundingStreamPeriod, a.FundingStreamGroupingTypeReason },
                            (key, groupedValues) => groupedValues.OrderByDescending(a => a.FundingVersionInt).First())
                        .ToList();

            childRequest.ListOfUKPRNs = parents
                                            ?.SelectMany(a => a.ChildUKPRNs ?? new List<string>())
                                            ?.Where(id => id.IsValidUKPRN())
                                            ?.Distinct()
                                            ?.ToList();

            var childrenUKPRNs = await this.GetCurrentChildUkprnsForParent(parentUkprn, childRequest.ListOfUKPRNs, cancellationToken);

            childRequest.ListOfUKPRNs = childrenUKPRNs;

            if (childrenUKPRNs?.Count == 0)
            {
                return new List<LoggedInChildAzSearchModel>();
            }

            return await this.SearchChild(childRequest, cancellationToken);
        }

        /// <summary>
        /// Gets the child comparison.
        /// </summary>
        /// <param name="childComparisonRequest">The child comparison request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// List of ChildComparisonResponse.
        /// </returns>
        public async Task<Dictionary<ComparisonTypeEnum, ChildComparisonResponse>> GetChildComparison(ChildComparisonRequest childComparisonRequest, CancellationToken cancellationToken)
        {
            var result = new Dictionary<ComparisonTypeEnum, ChildComparisonResponse>();

            ChildRequest childRequest = new()
            {
                ListOfUKPRNs = new() { childComparisonRequest.ChildUKPRN },
                HasToBeLatestFunding = false,
                HasIYOToBeRemoved = true,
                DoFindIsLatest = true,
                FundingStreamCode = childComparisonRequest.FundingStreamCode,
            };

            childRequest.SetSelectFields(a => new { a.Id, a.StatusChangedDateOnly, a.InYearOpener, a.ParentInfo });

            var childFundings = await this.SearchChild(childRequest, cancellationToken);


            DateTime.TryParseExact(
                            childComparisonRequest.StatusChangedDateOnly,
                            DateConstants.SettingDateFormat,
                            DateConstants.EnGbCultureInfo,
                            DateTimeStyles.AdjustToUniversal,
                            out var childStatuschangeDate);

            var currentProviderFunding = childFundings
                                            ?.OrderByDescending(a => a.FundingStreamPeriod).ThenByDescending(a => a.FundingVersionInt)
                                            ?.FirstOrDefault(a => a.FundingStreamPeriod == childComparisonRequest.CurrentFundingStreamPeriodCode
                                                                    && a.StatusChangedDateOnly.Value.Date == childStatuschangeDate);

            if (childFundings == null
                || currentProviderFunding == null
                || !this.comparisonServices.IsLatestProviderFunding(currentProviderFunding.Id!, childFundings))
            {
                return result;
            }

            var currentYearPreviousComparisonResponse = this
                                                        .comparisonServices
                                                        .GetPreviousStatementCurrentYear(
                                                            childComparisonRequest.FundingStreamCode,
                                                            childComparisonRequest,
                                                            currentProviderFunding,
                                                            childFundings);

            var lastYearFinalComparisonResponse = this
                                                    .comparisonServices
                                                    .GetFinalStatementPreviousYear(
                                                        childComparisonRequest.FundingStreamCode,
                                                        childComparisonRequest,
                                                        currentProviderFunding,
                                                        childFundings);

            if (currentYearPreviousComparisonResponse != null)
            {
                result.Add(currentYearPreviousComparisonResponse.ComparisonType, currentYearPreviousComparisonResponse);
            }

            if (lastYearFinalComparisonResponse != null)
            {
                result.Add(lastYearFinalComparisonResponse.ComparisonType, lastYearFinalComparisonResponse);
            }

            if (result.Count == 0)
            {
                return result;
            }

            var comparisonProviderFundings = await this.SearchChild(new ChildRequest { ListOfIds = result.Select(a => a.Value.ProviderFundingId).ToList() }, cancellationToken);

            if (currentYearPreviousComparisonResponse != null)
            {
                currentYearPreviousComparisonResponse.LoggedInChildAzSearchModel = comparisonProviderFundings?.FirstOrDefault(a => a.Id == currentYearPreviousComparisonResponse.ProviderFundingId);
            }

            if (lastYearFinalComparisonResponse != null)
            {
                lastYearFinalComparisonResponse.LoggedInChildAzSearchModel = comparisonProviderFundings?.FirstOrDefault(a => a.Id == lastYearFinalComparisonResponse.ProviderFundingId);
            }

            return result;
        }
    }
}
