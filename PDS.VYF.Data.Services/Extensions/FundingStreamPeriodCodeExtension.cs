namespace PDS.VYF.Data.Services.Extensions
{
    /// <summary>
    /// The class containing extension methods for funding stream period code.
    /// </summary>
    public static class FundingStreamPeriodCodeExtension
    {
        /// <summary>
        /// Extracts the year portion from the funding stream period code.
        /// </summary>
        /// <param name="fundingStreamPeriodCode">The funding stream period code.</param>
        /// <returns>The extracted year portion.</returns>
        public static (int yearFrom, int yearTo) GetYearFromYearTo(this string fundingStreamPeriodCode)
        {
            string[] parts = fundingStreamPeriodCode.Split('-');

            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid funding stream period code format.");
            }

            if (parts[2].Length != 4 || !int.TryParse(parts[2], out _))
            {
                throw new ArgumentException("Invalid year format.");
            }

            string lastPart = parts[2];

            var yearFrom = 2000 + int.Parse(lastPart.Substring(0, 2));
            var yearTo = 2000 + int.Parse(lastPart.Substring(2, 2));

            return (yearFrom, yearTo);
        }

        /// <summary>
        /// Gets the previous funding stream period code based on the current code.
        /// </summary>
        /// <param name="fundingStreamPeriodCode">The current funding stream period code.</param>
        /// <returns>The previous funding stream period code.</returns>
        public static string GetPreviousFundingStreamPeriodCode(this string fundingStreamPeriodCode)
        {
            string[] parts = fundingStreamPeriodCode.Split('-');

            (int yearFrom, int yearTo) = GetYearFromYearTo(fundingStreamPeriodCode);

            // Calculate the previous year
            (int previousYearFrom, int previousYearTo) = (yearFrom - 1, yearTo - 1);

            string previousPeriodCode = $"{parts[0]}-{parts[1]}-{previousYearFrom.ToString().Substring(2)}{previousYearTo.ToString().Substring(2)}";

            return previousPeriodCode;
        }

        /// <summary>
        /// Gets the maximum funding stream period code.
        /// </summary>
        /// <param name="fundingStreamPeriodCodes">The funding stream period codes.</param>
        /// <returns>Max of FundingStreamPeriodCode.</returns>
        public static string? GetMaxFundingStreamPeriodCode(this IEnumerable<string> fundingStreamPeriodCodes)
        {
            return fundingStreamPeriodCodes.MaxBy(a => a.GetYearFromYearTo().yearFrom);
        }

        /// <summary>
        /// Determines whether [is latest funding stream period code] [the specified funding stream period code].
        /// </summary>
        /// <param name="fundingStreamPeriodCode">The funding stream period code.</param>
        /// <param name="fundingStreamPeriodCodes">The funding stream period codes.</param>
        /// <returns>
        ///   <c>true</c> if [is latest funding stream period code] [the specified funding stream period code]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLatestFundingStreamPeriodCode(this string fundingStreamPeriodCode, IEnumerable<string> fundingStreamPeriodCodes)
        {
            return fundingStreamPeriodCodes.GetMaxFundingStreamPeriodCode() == fundingStreamPeriodCode;
        }
    }
}
