using System;

namespace PDS.ViewYourFunding.Data.Interfaces.DTOs
{
    /// <summary>
    /// The lookup data for one funding stream.
    /// </summary>
    public class FundingStreamParameters
    {
        /// <summary>
        /// Gets or sets cut off date.
        /// </summary>
        public DateTime BeforeDateTime { get; set; }

        /// <summary>
        /// Gets or sets funding stream code to filter by (e.g. PSG).
        /// </summary>
        public string FundingStreamCode { get; set; }

        /// <summary>
        /// Gets or sets period codes to filter on (e.g. AY-1920).
        /// </summary>
        public string[] PeriodCodes { get; set; }

        /// <summary>
        /// Gets or sets grouping type (e.g. LocalAuthority).
        /// </summary>
        public string GroupingType { get; set; }

        /// <summary>
        /// Gets or sets filters to apply.
        /// </summary>
        public SearchFilterParameters[] Filters { get; set; }
    }
}
