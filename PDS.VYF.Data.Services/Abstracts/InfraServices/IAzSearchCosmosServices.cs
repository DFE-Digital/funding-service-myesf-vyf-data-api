namespace PDS.VYF.Data.Services.Abstracts.InfraServices
{
    using PDS.VYF.Data.Services.Enums;

    /// <summary>
    /// Interface for AzSearchCosmosServices.
    /// </summary>
    public interface IAzSearchCosmosServices
    {
        /// <summary>
        /// Gets the container name for the specified AzSearchIndexTypeEnum.
        /// </summary>
        /// <param name="azSearchIndexTypeEnum">The AzSearchIndexTypeEnum value.</param>
        /// <returns>The container name.</returns>
        string GetContainerName(AzSearchIndexTypeEnum azSearchIndexTypeEnum);

        /// <summary>
        /// Gets the Cosmos query for the specified AzSearchIndexTypeEnum.
        /// </summary>
        /// <param name="azSearchIndexTypeEnum">The AzSearchIndexTypeEnum value.</param>
        /// <returns>The Cosmos query.</returns>
        string GetCosmosQuery(AzSearchIndexTypeEnum azSearchIndexTypeEnum);

        /// <summary>
        /// Gets the index field type for the specified AzSearchIndexTypeEnum.
        /// </summary>
        /// <param name="azSearchIndexTypeEnum">The AzSearchIndexTypeEnum value.</param>
        /// <returns>The index field type.</returns>
        Type GetIndexFieldType(AzSearchIndexTypeEnum azSearchIndexTypeEnum);

        /// <summary>
        /// Gets the index types for the specified AzSearchIndexTypeEnum.
        /// </summary>
        /// <param name="azSearchIndexTypeEnum">The AzSearchIndexTypeEnum value.</param>
        /// <returns>The array of index types.</returns>
        Type[] GetIndexTypesForThumbPrint(AzSearchIndexTypeEnum azSearchIndexTypeEnum);
    }
}
