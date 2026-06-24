namespace PDS.VYF.Data.Services.Models.ResponseModels
{
    using PDS.VYF.Data.Services.Enums;
    using PDS.VYF.Data.Services.Models.AzSearchModels;

    /// <summary>
    /// The response model for child comparison.
    /// </summary>
    public class ChildComparisonResponse
    {
        /// <summary>
        /// Gets or sets the provider funding identifier.
        /// </summary>
        /// <value>
        /// The provider funding identifier.
        /// </value>
        public string ProviderFundingId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the funding stream period code.
        /// </summary>
        /// <value>
        /// The funding stream period code.
        /// </value>
        public string FundingStreamPeriodCode { get; set; } = default!;

        /// <summary>
        /// Gets or sets the status changed date only.
        /// </summary>
        /// <value>
        /// The status changed date only.
        /// </value>
        public DateTime StatusChangedDateOnly { get; set; }

        /// <summary>
        /// Gets or sets the type of the comparison.
        /// </summary>
        /// <value>
        /// The type of the comparison.
        /// </value>
        public ComparisonTypeEnum ComparisonType { get; set; }

        /// <summary>
        /// Gets or sets the logged in child az search model.
        /// </summary>
        /// <value>
        /// The logged in child az search model.
        /// </value>
        public LoggedInChildAzSearchModel? LoggedInChildAzSearchModel { get; set; }
    }
}
