namespace PDS.ViewYourFunding.Data.Interfaces
{
    /// <summary>
    /// An interface for managing Azure Search Index Client connections by alias.
    /// </summary>
    public interface IAzureSearchAliasClientManager
    {
        /// <summary>
        /// Get the Search Index Client for the given type.
        /// </summary>
        /// <typeparam name="T">The document type of the index.</typeparam>
        /// <param name="classThumbprint">The hash/thumbprint of T.</param>
        /// <returns>A Search Index Client for the given type.</returns>
        IAzureSearchAliasClient<T> GetSearchAliasClient<T>(int classThumbprint)
            where T : class;
    }
}
