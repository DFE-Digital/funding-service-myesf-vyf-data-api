namespace PDS.VYF.Data.Services.Enums
{
    /// <summary>
    /// The options for variance comparison.
    /// </summary>
    public enum ComparisonTypeEnum
    {
        /// <summary>
        /// Calculate variance with current year previous statement.
        /// </summary>
        PreviousStatementCurrentYear = 0,

        /// <summary>
        /// Calculate variance with previous year final statement.
        /// </summary>
        FinalStatementPreviousYear = 1,
    }
}
