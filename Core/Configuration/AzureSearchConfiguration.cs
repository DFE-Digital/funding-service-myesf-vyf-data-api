namespace PDS.ViewYourFunding.Data.Core.Configuration
{
    /// <summary>
    /// The Azure Search configuration.
    /// </summary>
    public class AzureSearchConfiguration
    {
        /// <summary>
        /// Gets or sets the funding azure search admin key.
        /// </summary>
        public string AdminKey { get; set; }

        /// <summary>
        /// Gets or sets the funding azure search name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the funding azure search query key.
        /// </summary>
        public string QueryKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the az resource base.
        /// </summary>
        /// <value>
        /// The name of the az resource base.
        /// </value>
        public string AzResourceBaseName { get; set; } = "vyf-fundingapi";
    }
}