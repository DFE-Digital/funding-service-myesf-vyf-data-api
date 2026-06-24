namespace PDS.VYF.Data.Services.Abstracts.InfraServices
{
    using Azure.Data.Tables;

    /// <summary>
    /// Represents the interface for Azure Table services.
    /// </summary>
    public interface IAzTableServices
    {
        /// <summary>
        /// Retrieves the children IDs of a view asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A dictionary containing the children IDs as keys and a boolean value indicating if the ID exists.</returns>
        Task<Dictionary<string, bool>> GetViewChildrenIdAsync(string userId);

        /// <summary>
        /// Checks if a user has viewed a provider funding ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="providerFundingId">The provider funding ID.</param>
        /// <returns>True if the user has viewed the provider funding ID, otherwise false.</returns>
        Task<bool> IsUserViewedProviderFundingId(string userId, string providerFundingId);

        /// <summary>
        /// Upserts an entity asynchronously.
        /// </summary>
        /// <param name="tableEntity">The table entity to upsert.</param>
        /// <returns>True if the entity was upserted successfully, otherwise false.</returns>
        Task<bool> UpsertEntityAsync(TableEntity tableEntity);
    }
}
