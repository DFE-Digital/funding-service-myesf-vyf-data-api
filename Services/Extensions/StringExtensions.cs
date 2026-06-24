namespace PDS.ViewYourFunding.Data.Services.Extensions
{
    /// <summary>
    /// Helper class to work with business string values.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns if this funding is the first version.
        /// </summary>
        /// <param name="fundingId">The funding id to match on.</param>
        /// <returns>True or false depending on whether it's the first version of funding.</returns>
        public static bool IsFirstVersionOfFunding(this string fundingId)
        {
            return fundingId.EndsWith("1_0");
        }
    }
}
