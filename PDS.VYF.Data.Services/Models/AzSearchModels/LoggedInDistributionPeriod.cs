namespace PDS.VYF.Data.Services.Models.AzSearchModels
{
    using Azure.Search.Documents.Indexes;

    /// <summary>
    /// The class for Logged In Distribution Period.
    /// </summary>
    public class LoggedInDistributionPeriod
    {
        /// <summary>
        /// Gets or sets the distribution period identifier.
        /// </summary>
        /// <value>
        /// The distribution period identifier.
        /// </value>
        [SimpleField]
        public string? DistributionPeriodId { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [SimpleField]
        public double? Value { get; set; }
    }
}
