namespace PDS.VYF.Data.Services.Models.AzSearchModels
{
    using Azure.Search.Documents.Indexes;

    /// <summary>
    /// The model for the logged in child search index.
    /// </summary>
    public class LoggedInChildAzSearchModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [SearchableField(IsKey = true, IsFilterable = true)]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier without version.
        /// </summary>
        /// <value>
        /// The identifier without version.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? IdWithoutVersion { get; set; }

        /// <summary>
        /// Gets or sets the parent information.
        /// </summary>
        /// <value>
        /// The parent information.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public List<LoggedInParentInfoModel>? ParentInfo { get; set; }

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
        /// Gets or sets the year from.
        /// </summary>
        /// <value>
        /// The year from.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public int? YearFrom { get; set; }

        /// <summary>
        /// Gets or sets the year to.
        /// </summary>
        /// <value>
        /// The year to.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public int? YearTo { get; set; }

        /// <summary>
        /// Gets or sets the status changed date.
        /// </summary>
        /// <value>
        /// The status changed date.
        /// </value>
        [SimpleField(IsFilterable = true, IsSortable = true)]
        public DateTime? StatusChangedDate { get; set; }

        /// <summary>
        /// Gets or sets the status changed date only.
        /// </summary>
        /// <value>
        /// The status changed date only.
        /// </value>
        [SimpleField(IsFilterable = true, IsSortable = true)]
        public DateTime? StatusChangedDateOnly { get; set; }

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
        public int FundingVersionInt { get; set; }

        /// <summary>
        /// Gets or sets the template version.
        /// </summary>
        /// <value>
        /// The template version.
        /// </value>
        [SimpleField]
        public string? TemplateVersion { get; set; }

        /// <summary>
        /// Gets or sets the schema version.
        /// </summary>
        /// <value>
        /// The schema version.
        /// </value>
        [SimpleField]
        public string? SchemaVersion { get; set; }

        /// <summary>
        /// Gets or sets the organisation name.
        /// </summary>
        /// <value>
        /// The organisation name.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public string? OrganisationName { get; set; }

        /// <summary>
        /// Gets or sets the searchable organisation name.
        /// </summary>
        /// <value>
        /// The searchable organisation name.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public string? SearchableOrganisationName { get; set; }

        /// <summary>
        /// Gets or sets the organisation UKPRN.
        /// </summary>
        /// <value>
        /// The organisation UKPRN.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public string? OrganisationUkprn { get; set; }

        /// <summary>
        /// Gets or sets the provider URN.
        /// </summary>
        /// <value>
        /// The provider URN.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? ProviderUrn { get; set; }

        /// <summary>
        /// Gets or sets the organisation DFE number.
        /// </summary>
        /// <value>
        /// The organisation DFE number.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public string? OrganisationDfeNumber { get; set; }

        /// <summary>
        /// Gets or sets the organisation town.
        /// </summary>
        /// <value>
        /// The organisation town.
        /// </value>
        [SimpleField]
        public string? OrganisationTown { get; set; }

        /// <summary>
        /// Gets or sets the organisation postcode.
        /// </summary>
        /// <value>
        /// The organisation postcode.
        /// </value>
        [SimpleField]
        public string? OrganisationPostcode { get; set; }

        /// <summary>
        /// Gets or sets the provider type.
        /// </summary>
        /// <value>
        /// The provider type.
        /// </value>
        [SimpleField]
        public string? ProviderType { get; set; }

        /// <summary>
        /// Gets or sets the provider sub type.
        /// </summary>
        /// <value>
        /// The provider sub type.
        /// </value>
        [SimpleField]
        public string? ProviderSubType { get; set; }

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
        /// Gets or sets the funding lines for summary.
        /// </summary>
        /// <value>
        /// The funding lines for summary.
        /// </value>
        [SimpleField]
        public IEnumerable<LoggedInTemplateLine>? FundingLinesForSummary { get; set; }

        /// <summary>
        /// Gets or sets the calculations.
        /// </summary>
        /// <value>
        /// The calculations.
        /// </value>
        [SimpleField]
        public IEnumerable<LoggedInCalculation>? Calculations { get; set; }

        /// <summary>
        /// Gets or sets the calculations for summary.
        /// </summary>
        /// <value>
        /// The calculations for summary.
        /// </value>
        [SimpleField]
        public IEnumerable<LoggedInCalculation>? CalculationsForSummary { get; set; }

        /// <summary>
        /// Gets or sets the provider status.
        /// </summary>
        /// <value>
        /// The provider status.
        /// </value>
        [SimpleField]
        public string? ProviderStatus { get; set; }

        /// <summary>
        /// Gets or sets the date opened.
        /// </summary>
        /// <value>
        /// The date opened.
        /// </value>
        [SimpleField]
        public DateTime? DateOpened { get; set; }

        /// <summary>
        /// Gets or sets the date closed.
        /// </summary>
        /// <value>
        /// The date closed.
        /// </value>
        [SimpleField]
        public DateTime? DateClosed { get; set; }

        /// <summary>
        /// Gets or sets the open reason.
        /// </summary>
        /// <value>
        /// The open reason.
        /// </value>
        [SimpleField]
        public string? OpenReason { get; set; }

        /// <summary>
        /// Gets or sets the close reason.
        /// </summary>
        /// <value>
        /// The close reason.
        /// </value>
        [SimpleField]
        public string? CloseReason { get; set; }

        /// <summary>
        /// Gets or sets the phase of education.
        /// </summary>
        /// <value>
        /// The phase of education.
        /// </value>
        [SimpleField]
        public string? PhaseOfEducation { get; set; }

        /// <summary>
        /// Gets or sets the local authority name.
        /// </summary>
        /// <value>
        /// The local authority name.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public string? LocalAuthorityName { get; set; }

        /// <summary>
        /// Gets or sets the parliamentary constituency name.
        /// </summary>
        /// <value>
        /// The parliamentary constituency name.
        /// </value>
        [SimpleField]
        public string? ParliamentaryConstituencyName { get; set; }

        /// <summary>
        /// Gets or sets the parliamentary constituency code.
        /// </summary>
        /// <value>
        /// The parliamentary constituency code.
        /// </value>
        [SimpleField]
        public string? ParliamentaryConstituencyCode { get; set; }

        /// <summary>
        /// Gets or sets the variation reasons.
        /// </summary>
        /// <value>
        /// The variation reasons.
        /// </value>
        [SearchableField(IsFilterable = true)]
        public IEnumerable<string>? VariationReasons { get; set; }

        /// <summary>
        /// Gets or sets the statement channel version.
        /// </summary>
        /// <value>
        /// The statement channel version.
        /// </value>
        [SimpleField]
        public int? StatementChannelVersion { get; set; }

        /// <summary>
        /// Gets or sets the statement type.
        /// </summary>
        /// <value>
        /// The statement type.
        /// </value>
        [SimpleField]
        public string? StatementType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is indicative.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is indicative; otherwise, <c>false</c>.
        /// </value>
        [SimpleField(IsFilterable = true)]
        public bool? IsIndicative { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in year opener.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is in year opener; otherwise, <c>false</c>.
        /// </value>
        [SimpleField]
        public bool? InYearOpener { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is latest.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is latest; otherwise, <c>false</c>.
        /// </value>
        [SimpleField]
        public bool? IsLatest { get; set; }
    }
}
