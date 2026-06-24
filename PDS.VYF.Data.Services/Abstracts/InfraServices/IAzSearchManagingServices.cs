namespace PDS.VYF.Data.Services.Abstracts.InfraServices
{
    using System.Threading.Tasks;
    using PDS.VYF.Data.Services.Enums;

    /// <summary>
    /// The interface for Azure search managing services.
    /// </summary>
    public interface IAzSearchManagingServices
    {
        /// <summary>
        /// Creates the or replace az search resources.
        /// </summary>
        /// <param name="azSearchIndexType">Type of the az search index.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> CreateOrReplaceAzSearchResources(AzSearchIndexTypeEnum azSearchIndexType, CancellationToken cancellationToken);
    }
}
