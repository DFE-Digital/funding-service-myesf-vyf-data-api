namespace PDS.VYF.Data.Services.Abstracts.AppServices
{
    using PDS.VYF.Data.Services.Enums;
    using PDS.VYF.Data.Services.Models.AzSearchModels;
    using PDS.VYF.Data.Services.Models.RequestModels;
    using PDS.VYF.Data.Services.Models.ResponseModels;

    /// <summary>
    /// The Interface of ChildSearchServices.
    /// </summary>
    public interface IChildSearchServices
    {
        /// <summary>
        /// Gets the child comparison.
        /// </summary>
        /// <param name="childComparisonRequest">The child comparison request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The Dictionary of response.</returns>
        Task<Dictionary<ComparisonTypeEnum, ChildComparisonResponse>> GetChildComparison(ChildComparisonRequest childComparisonRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether [is latest statement] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="childRequest">The child request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the statement is latest.</returns>
        Task<bool> IsLatestStatement(string id, ChildRequest childRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Searches current children of Parent.
        /// </summary>
        /// <param name="parentUkprn">The parent ukprn.</param>
        /// <param name="childUkprn">The child UKPRNs for parent.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the statement is latest.</returns>
        Task<List<string>?> GetCurrentChildUkprnsForParent(string parentUkprn, List<string> childUkprn, CancellationToken cancellationToken);

        /// <summary>
        /// Searches the child.
        /// </summary>
        /// <param name="childRequest">The child request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>List of Child Statements.</returns>
        Task<List<LoggedInChildAzSearchModel>?> SearchChild(ChildRequest childRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Searches the children of a parent.
        /// </summary>
        /// <param name="parentUkprn">The parent ukprn.</param>
        /// <param name="childRequest">The child request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>List of Child statements for the given parent.</returns>
        Task<List<LoggedInChildAzSearchModel>?> SearchChildrenOfAParent(string parentUkprn, ChildRequest childRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Searches the latest Funding period for UKPRN.
        /// </summary>
        /// <param name="childRequest">The child request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Latest Funding Period.</returns>
        Task<List<string>?> LatestFundingPeriod(ChildRequest childRequest, CancellationToken cancellationToken);
    }
}
