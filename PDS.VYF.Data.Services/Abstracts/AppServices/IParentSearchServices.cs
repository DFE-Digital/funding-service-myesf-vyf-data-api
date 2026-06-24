namespace PDS.VYF.Data.Services.Abstracts.AppServices
{
    using PDS.VYF.Data.Services.Models.AzSearchModels;
    using PDS.VYF.Data.Services.Models.RequestModels;

    /// <summary>
    /// Interface for parent search services.
    /// </summary>
    public interface IParentSearchServices
    {
        /// <summary>
        /// Checks if the specified child is associated with the specified parent.
        /// </summary>
        /// <param name="parentUkprn">The UKPRN of the parent.</param>
        /// <param name="childUkprn">The UKPRN of the child.</param>
        /// <param name="fundingStreamPeriods">The list of funding stream periods.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the child is associated with the parent, false otherwise.</returns>
        Task<bool> IsMyChild(string parentUkprn, string childUkprn, List<string> fundingStreamPeriods, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if the specified UKPRN belongs to a parent.
        /// </summary>
        /// <param name="ukprn">The UKPRN to check.</param>
        /// <param name="fundingStreamPeriods">The list of funding stream periods.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if the UKPRN belongs to a parent, false otherwise.</returns>
        Task<bool> IsParent(string ukprn, List<string> fundingStreamPeriods, CancellationToken cancellationToken);

        /// <summary>
        /// Searches for parents based on the provided parent request.
        /// </summary>
        /// <param name="parentRequest">The parent request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of logged in parent Azure Search models, or null if no parents are found.</returns>
        Task<List<LoggedInParentAzSearchModel>?> SearchParent(ParentRequest parentRequest, CancellationToken cancellationToken);
    }
}
