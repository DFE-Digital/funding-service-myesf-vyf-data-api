namespace PDS.ViewYourFunding.Data.Core
{
    /// <summary>
    /// The Cosmos Db config.
    /// </summary>
    public class CosmosDbConfiguration
    {
        /// <summary>
        /// Gets or sets the funding cosmos database connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the funding collection.
        /// </summary>
        /// <value>
        /// The funding collection.
        /// </value>
        public string FundingCollection { get; set; }

        /// <summary>
        /// Gets or sets the funding query.
        /// </summary>
        /// <value>
        /// The funding query.
        /// </value>
        public string FundingQuery { get; set; } = Constants.CosmosDbDefaultFundingQuery;

        /// <summary>
        /// Gets or sets the provider funding collection.
        /// </summary>
        /// <value>
        /// The provider funding collection.
        /// </value>
        public string ProviderFundingCollection { get; set; }

        /// <summary>
        /// Gets or sets the provider funding query.
        /// </summary>
        /// <value>
        /// The provider funding query.
        /// </value>
        public string ProviderFundingQuery { get; set; } = Constants.CosmosDbDefaultProviderFundingQuery;
    }
}
