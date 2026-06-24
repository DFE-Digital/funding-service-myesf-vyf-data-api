namespace PDS.ViewYourFunding.Data.Core.Configuration
{
    /// <summary>
    /// The Repositories Configuration.
    /// </summary>
    public class RepositoriesConfiguration
    {
        /// <summary>
        /// Gets or sets the azure search.
        /// </summary>
        /// <value>
        /// The azure search.
        /// </value>
        public AzureSearchConfiguration AzureSearch { get; set; } = new AzureSearchConfiguration();

        /// <summary>
        /// Gets or sets the cosmos database.
        /// </summary>
        /// <value>
        /// The cosmos database.
        /// </value>
        public CosmosDbConfiguration CosmosDb { get; set; } = new CosmosDbConfiguration();
    }
}