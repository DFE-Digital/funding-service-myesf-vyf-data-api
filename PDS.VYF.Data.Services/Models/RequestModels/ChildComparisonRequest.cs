namespace PDS.VYF.Data.Services.Models.RequestModels
{
    /// <summary>
    /// The request model for child comparison.
    /// </summary>
    public class ChildComparisonRequest
    {
        /// <summary>
        /// Gets or sets the child ukprn.
        /// </summary>
        /// <value>
        /// The child ukprn.
        /// </value>
        public string ChildUKPRN { get; set; } = default!;

        /// <summary>
        /// Gets or sets the funding stream period code.
        /// </summary>
        /// <value>
        /// The funding stream period code.
        /// </value>
        public string CurrentFundingStreamPeriodCode { get; set; } = default!;

        /// <summary>
        /// Gets or sets the status changed date only.
        /// </summary>
        /// <value>
        /// The status changed date only.
        /// </value>
        public string StatusChangedDateOnly { get; set; } = default!;

        /// <summary>
        /// Gets or sets the funding stream code.
        /// </summary>
        /// <value>
        /// The funding stream code.
        /// </value>
        public string FundingStreamCode { get; set; } = default!;

        /// <summary>
        /// Gets or sets the parent ukprn.
        /// </summary>
        /// <value>
        /// The parent ukprn.
        /// </value>
        public string? ParentUKPRN { get; set; } = default!;
    }
}
