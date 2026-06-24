namespace PDS.VYF.Data.Services.Extensions
{
    /// <summary>
    /// The class which holds all the extension methods for ProviderFunding.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Converts the provider funding identifier to ukprn.
        /// </summary>
        /// <param name="providerFundingId">The provider funding identifier.</param>
        /// <returns>The UKPRN.</returns>
        public static string ConvertProviderFundingIdToUkprn(this string providerFundingId)
        {
            return string.IsNullOrWhiteSpace(providerFundingId) || providerFundingId.Split("-").Length < 4
                    ? string.Empty
                    : providerFundingId.Split("-")[3];
        }

        /// <summary>
        /// Determines whether [is valid ukprn].
        /// </summary>
        /// <param name="ukprn">The ukprn.</param>
        /// <returns>
        ///   <c>true</c> if [is valid ukprn] [the specified ukprn]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidUKPRN(this string ukprn)
        {
            return !string.IsNullOrWhiteSpace(ukprn) && ukprn.Length == 8 && ukprn.All(char.IsDigit);
        }

        /// <summary>
        /// Adds the quote in each value.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="isSingleQuote">if set to <c>true</c> [is single quote].</param>
        /// <returns>Quoted String.</returns>
        public static string AddQuoteInEachValue(this string? input, string delimiter, bool isSingleQuote)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            string quote = isSingleQuote ? "'" : "\"";
            var quoatedEntries = input.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).Select(a => quote + a + quote);
            return string.Join(delimiter, quoatedEntries);
        }
    }
}
