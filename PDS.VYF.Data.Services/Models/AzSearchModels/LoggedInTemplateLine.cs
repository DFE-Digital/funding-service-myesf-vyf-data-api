namespace PDS.VYF.Data.Services.Models.AzSearchModels
{
    using Azure.Search.Documents.Indexes;

    /// <summary>
    /// The class for Logged In Template Line.
    /// </summary>
    public class LoggedInTemplateLine
    {
        /// <summary>
        /// Gets or sets the template line identifier.
        /// </summary>
        /// <value>
        /// The template line identifier.
        /// </value>
        [SimpleField]
        public int TemplateLineId { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [SimpleField]
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [SimpleField]
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the distribution periods.
        /// </summary>
        /// <value>
        /// The distribution periods.
        /// </value>
        [SimpleField]
        public List<LoggedInDistributionPeriod>? DistributionPeriods { get; set; }
    }
}
