using PDS.ViewYourFunding.Data.Core.Configuration;

namespace PDS.ViewYourFunding.Data.Core
{
    /// <summary>
    /// The Application configuration class.
    /// </summary>
    public class ApplicationConfiguration
    {
        /// <summary>
        /// Gets or sets the applications insights key.
        /// </summary>
        public string ApplicationInsightsKey { get; set; }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the build configuration.
        /// </summary>
        public string BuildConfiguration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the API should be secure using OAuth.
        /// </summary>
        /// <value>
        ///  Set to true if OAuth security is enabled on the API.
        /// </value>
        public bool EnableOAuthSecurity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to filter on funding version.
        /// </summary>
        /// <value>
        ///  Set to true if filtering on funding version.
        /// </value>
        public bool FilterOnFundingVersion { get; set; }

        /// <summary>
        /// Gets or sets the repositories.
        /// </summary>
        /// <value>
        /// The repositories.
        /// </value>
        public RepositoriesConfiguration Repositories { get; set; } = new RepositoriesConfiguration();

        /// <summary>
        /// Gets or sets the logging configuration.
        /// </summary>
        public LoggingConfiguration Logging { get; set; } = new LoggingConfiguration();

        /// <summary>
        /// Gets or sets the storage account configuration.
        /// </summary>
        public StorageAccountConfiguration StorageAccount { get; set; } = new StorageAccountConfiguration();

        /// <summary>
        /// Gets or sets the search index client timeout in seconds.
        /// </summary>
        /// <value>
        /// The search index client timeout in seconds.
        /// </value>
        public int SearchIndexClientTimeoutInSeconds { get; set; } = 180;

        /// <summary>
        /// Gets or sets the restricted funding stream codes.
        /// </summary>
        /// <value>
        /// The restricted funding stream codes.
        /// </value>
        public string RestrictedFundingStreamCodes { get; set; } = "GAG,1619,NMSS,1416";

        /// <summary>
        /// Gets or sets the restricted variation reasons.
        /// </summary>
        /// <value>
        /// The restricted variation reasons.
        /// </value>
        public string RestrictedVariationReasons { get; set; } = "LegalNameFieldUpdated,TrustCodeFieldUpdated,FundingUpdated,ProfilingUpdated,CalculationValuesUpdated,DateOpenedFieldUpdated,DateClosedFieldUpdated,TrustStatusFieldUpdated";
    }
}