namespace PDS.VYF.Data.Services.Implementations.AppServices
{
    using PDS.VYF.Data.Services.Abstracts.AppServices;
    using PDS.VYF.Data.Services.Abstracts.InfraServices;
    using PDS.VYF.Data.Services.Enums;
    using PDS.VYF.Data.Services.Extensions;
    using PDS.VYF.Data.Services.Models.AzSearchModels;
    using PDS.VYF.Data.Services.Models.RequestModels;

    /// <summary>
    /// The Parent Search Services class.
    /// </summary>
    /// <seealso cref="PDS.VYF.Data.Services.Implementations.AppServices.SearchServicesBase" />
    /// <seealso cref="PDS.VYF.Data.Services.Abstracts.AppServices.IParentSearchServices" />
    public class ParentSearchServices : SearchServicesBase, IParentSearchServices
    {
        private readonly IAzSearchSearchingServices azSearchSearchingServices;
        private readonly AzSearchIndexTypeEnum parentAzSearchIndexType = AzSearchIndexTypeEnum.LoggedIn_Parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentSearchServices"/> class.
        /// </summary>
        /// <param name="azSearchSearchingServices">The az search searching services.</param>
        public ParentSearchServices(IAzSearchSearchingServices azSearchSearchingServices)
        {
            this.azSearchSearchingServices = azSearchSearchingServices;
        }

        /// <summary>
        /// Searches the parent.
        /// </summary>
        /// <param name="parentRequest">The parent request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>List of Parent Model.</returns>
        public async Task<List<LoggedInParentAzSearchModel>?> SearchParent(ParentRequest parentRequest, CancellationToken cancellationToken)
        {
            var filter = parentRequest.BuildFilter().ToString();

            if (parentRequest.HasToBeLatestFunding
                        && parentRequest.SelectFields?.Count > 0)
            {
                parentRequest.TryAddSelectFields(a => new { a.GroupUkprn, a.FundingStreamPeriod, a.GroupingType, a.GroupingReason, a.FundingVersionInt, });
            }

            var result = await this.azSearchSearchingServices
                                    .SearchDocumentAsync<LoggedInParentAzSearchModel>(this.parentAzSearchIndexType, "*", filter, parentRequest.Select)
                                    .ToListAsync(cancellationToken: cancellationToken);

            if (parentRequest.HasToBeLatestFunding)
            {
                return result.GroupBy(
                                a => new { a.GroupUkprn, a.FundingStreamPeriod, a.GroupingType, a.GroupingReason, },
                                (key, groupedValues) => groupedValues.OrderByDescending(a => a.FundingVersionInt).First())
                            .ToList();
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified parent ukprn is parent.
        /// </summary>
        /// <param name="parentUkprn">The parent ukprn.</param>
        /// <param name="fundingStreamPeriods">The funding stream periods.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   <c>true</c> if the specified parent ukprn is parent; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsParent(string parentUkprn, List<string> fundingStreamPeriods, CancellationToken cancellationToken)
        {
            ParentRequest parentRequest = new()
            {
                FundingStreamPeriods = fundingStreamPeriods,
                HasToBeLatestFunding = false,
                ListOfUKPRNs = new List<string>() { parentUkprn },
            };

            parentRequest.SetSelectFields(a => new { a.GroupUkprn, a.ProviderFundings, });

            var filter = parentRequest
                            .BuildFilter()
                            .AppendEqFilter(true, nameof(LoggedInParentAzSearchModel.IsParent))
                            .ToString();

            return await this.azSearchSearchingServices
                                    .SearchDocumentAsync<LoggedInParentAzSearchModel>(this.parentAzSearchIndexType, "*", filter, parentRequest.Select)
                                    .AnyAsync();
        }

        /// <summary>
        /// Determines whether [is my child] [the specified parent ukprn].
        /// </summary>
        /// <param name="parentUkprn">The parent ukprn.</param>
        /// <param name="childUkprn">The child ukprn.</param>
        /// <param name="fundingStreamPeriods">The funding stream periods.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   <c>true</c> if [is my child] [the specified parent ukprn]; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsMyChild(string parentUkprn, string childUkprn, List<string> fundingStreamPeriods, CancellationToken cancellationToken)
        {
            ParentRequest parentRequest = new()
            {
                FundingStreamPeriods = fundingStreamPeriods,
                HasToBeLatestFunding = false,
                ListOfUKPRNs = new List<string>() { parentUkprn },
            };

            parentRequest.SetSelectFields(a => new { a.GroupUkprn, a.ChildUKPRNs, });

            // var search = $"{nameof(LoggedInParentAzSearchModel.ProviderFundings)}/any(g: search.ismatch(g, '-{childUkprn}- ')";
            var filter = parentRequest
                            .BuildFilter()
                            .AddSearchInFilter(new List<string>() { childUkprn }, nameof(LoggedInParentAzSearchModel.ChildUKPRNs), true)
                            .ToString();

            return await this.azSearchSearchingServices
                                    .SearchDocumentAsync<LoggedInParentAzSearchModel>(this.parentAzSearchIndexType, "*", filter, parentRequest.Select)
                                    .AnyAsync();
        }
    }
}
