namespace PDS.VYF.Data.Services.Models.AzSearchModels
{
    using Azure.Search.Documents.Indexes;

    /// <summary>
    /// The model for LoggedInParentInfoModel.
    /// </summary>
    public class LoggedInParentInfoModel
    {
        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>
        /// The parent identifier.
        /// </value>
        [SimpleField]
        public string? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the parent name.
        /// </summary>
        /// <value>
        /// The parent name.
        /// </value>
        [SimpleField]
        public string? ParentName { get; set; }

        /// <summary>
        /// Gets or sets the grouping reason.
        /// </summary>
        /// <value>
        /// The grouping reason.
        /// </value>
        [SimpleField]
        public string? GroupingReason { get; set; }

        /// <summary>
        /// Gets or sets the parent provider type.
        /// </summary>
        /// <value>
        /// The parent provider type.
        /// </value>
        [SimpleField]
        public string? ParentProviderType { get; set; }

        /// <summary>
        /// Gets or sets the grouping reason type.
        /// </summary>
        /// <value>
        /// The grouping reason type.
        /// </value>
        [SimpleField]
        public string? GroupingReasonType { get; set; }

        /// <summary>
        /// Gets or sets the status changed date.
        /// </summary>
        /// <value>
        /// The status changed date.
        /// </value>
        [SimpleField]
        public string? StatusChangedDate { get; set; }

        /// <summary>
        /// Gets or sets the parent primary identifier.
        /// </summary>
        /// <value>
        /// The parent primary identifier.
        /// </value>
        [SimpleField]
        public string? ParentPrimaryIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the parent UKPRN.
        /// </summary>
        /// <value>
        /// The parent UKPRN.
        /// </value>
        [SimpleField]
        public string? ParentUKPRN { get; set; }
    }
}
