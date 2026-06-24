namespace PDS.ViewYourFunding.Data.Interfaces.DTOs
{
    /// <summary>
    /// An optional filter to restrict the search by.
    /// </summary>
    public class SearchFilterParameters
    {
        /// <summary>
        /// Gets or sets the search property name.
        /// </summary>
        public FilterPropertyName PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the property value to filter by.
        /// </summary>
        public string PropertyValue { get; set; }

        /// <summary>
        /// Enum for filterable field names.
        /// </summary>
        public enum FilterPropertyName
        {
            /// <summary>
            /// Filter by the parents primary identifier (e.g. LA Code) - only applicable for ProviderFunding.
            /// </summary>
            ParentPrimaryIdentifier = 1,

            /// <summary>
            /// Filter by the UKPRN.
            /// </summary>
            Ukprn = 2,

            /// <summary>
            /// Filter by the primary identifier (e.g. LA Code).
            /// </summary>
            PrimaryIdentifier = 3,

            /// <summary>
            /// Filter by the grouping reason (e.g. Payment or Information).
            /// </summary>
            GroupingReason = 4,

            /// <summary>
            /// Filter by an identifier list (e.g. ukPrn).
            /// </summary>
            PrimaryIdentifierList = 5,

            /// <summary>
            /// Filter by the identifier (e.g. Id).
            /// </summary>
            Id = 6,

            /// <summary>
            /// Filter by the group name (e.g. NOTTINGHAMSHIRE COUNTY COUNCIL).
            /// </summary>
            GroupName = 7,

            /// <summary>
            /// Filter by the funding version (e.g. 1_0, 2_0).
            /// </summary>
            FundingVersion = 8
        }
    }
}
