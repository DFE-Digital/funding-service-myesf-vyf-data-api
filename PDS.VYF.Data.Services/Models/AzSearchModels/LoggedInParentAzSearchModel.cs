namespace PDS.VYF.Data.Services.Models.AzSearchModels
{
    using System.ComponentModel.DataAnnotations;
    using Azure.Search.Documents.Indexes;
    using Azure.Search.Documents.Indexes.Models;

    /// <summary>
    /// The logged in parent az search model.
    /// </summary>
    public class LoggedInParentAzSearchModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [SearchableField(IsFilterable = true)]
        [Key]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the grouping type.
        /// </summary>
        /// <value>
        /// The grouping type.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? GroupingType { get; set; }

        /// <summary>
        /// Gets or sets the grouping reason.
        /// </summary>
        /// <value>
        /// The grouping reason.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? GroupingReason { get; set; }

        /// <summary>
        /// Gets or sets the funding stream grouping type reason.
        /// </summary>
        /// <value>
        /// The funding stream grouping type reason.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? FundingStreamGroupingTypeReason { get; set; }

        /// <summary>
        /// Gets or sets the funding period code.
        /// </summary>
        /// <value>
        /// The funding period code.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? FundingPeriodCode { get; set; }

        /// <summary>
        /// Gets or sets the funding stream code.
        /// </summary>
        /// <value>
        /// The funding stream code.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? FundingStreamCode { get; set; }

        /// <summary>
        /// Gets or sets the funding stream period.
        /// </summary>
        /// <value>
        /// The funding stream period.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? FundingStreamPeriod { get; set; }

        /// <summary>
        /// Gets or sets the status changed date.
        /// </summary>
        /// <value>
        /// The status changed date.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public DateTime? StatusChangedDate { get; set; }

        /// <summary>
        /// Gets or sets the funding version.
        /// </summary>
        /// <value>
        /// The funding version.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public string? FundingVersion { get; set; }

        /// <summary>
        /// Gets or sets the funding version integer.
        /// </summary>
        /// <value>
        /// The funding version integer.
        /// </value>
        [SimpleField(IsFilterable = true, IsSortable = true)]
        public int? FundingVersionInt { get; set; }

        /// <summary>
        /// Gets or sets the schema version.
        /// </summary>
        /// <value>
        /// The schema version.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public string? SchemaVersion { get; set; }

        /// <summary>
        /// Gets or sets the template version.
        /// </summary>
        /// <value>
        /// The template version.
        /// </value>
        [SimpleField]
        public string? TemplateVersion { get; set; }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        /// <value>
        /// The group name.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? GroupName { get; set; }

        /// <summary>
        /// Gets or sets the searchable group name.
        /// </summary>
        /// <value>
        /// The searchable group name.
        /// </value>
        [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.StandardAsciiFoldingLucene)]
        public string? SearchableGroupName { get; set; }

        /// <summary>
        /// Gets or sets the group UKPRN.
        /// </summary>
        /// <value>
        /// The group UKPRN.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? GroupUkprn { get; set; }

        /// <summary>
        /// Gets or sets the group code.
        /// </summary>
        /// <value>
        /// The group code.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? GroupCode { get; set; }

        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        /// <value>
        /// The total amount.
        /// </value>
        [SimpleField]
        public double? TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the funding lines.
        /// </summary>
        /// <value>
        /// The funding lines.
        /// </value>
        [SimpleField]
        public IEnumerable<LoggedInTemplateLine>? FundingLines { get; set; }

        /// <summary>
        /// Gets or sets the calculations.
        /// </summary>
        /// <value>
        /// The calculations.
        /// </value>
        [SimpleField]
        public IEnumerable<LoggedInCalculation>? Calculations { get; set; }

        /// <summary>
        /// Gets or sets the provider fundings.
        /// </summary>
        /// <value>
        /// The provider fundings.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public IEnumerable<string>? ProviderFundings { get; set; }

        /// <summary>
        /// Gets or sets the child UKPRNs.
        /// </summary>
        /// <value>
        /// The child UKPRNs.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public IEnumerable<string>? ChildUKPRNs { get; set; }

        /// <summary>
        /// Gets or sets the variation reasons.
        /// </summary>
        /// <value>
        /// The variation reasons.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public IEnumerable<string>? VariationReasons { get; set; }

        /// <summary>
        /// Gets or sets the statement channel version.
        /// </summary>
        /// <value>
        /// The statement channel version.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public int? StatementChannelVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is parent.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is parent; otherwise, <c>false</c>.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public bool? IsParent { get; set; }
    }
}
