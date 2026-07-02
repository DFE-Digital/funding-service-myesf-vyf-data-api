using PDS.ViewYourFunding.Data.Interfaces.Models;

namespace PDS.ViewYourFunding.Data.Services.Helpers
{
    /// <summary>
    /// Extension class for IFundingVersionDetail Interface.
    /// </summary>
    public static class StatementChannelVersionHelper
    {
        /// <summary>
        /// Returns if this funding is the first version.
        /// </summary>
        /// <param name="fundingOrProviderFundingID">Funding or Provider Funding ID.</param>
        /// <param name="statementChannelVersion">Version of Statement Channel.</param>
        /// <returns>If the funding is first version it returns true else it return false.</returns>
        public static bool IsFirstVersionOfFunding(string fundingOrProviderFundingID, int? statementChannelVersion)
        {
            if (statementChannelVersion == 1)
            {
                return true;
            }
            else if (statementChannelVersion > 1)
            {
                return false;
            }
            else if (fundingOrProviderFundingID is not null)
            {
                return fundingOrProviderFundingID.EndsWith("-1_0");
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns if this funding is the first version.
        /// </summary>
        /// <param name="fundingVersionDetail">Funding version details.</param>
        /// <returns>If the funding is first version it returns true else it return false.</returns>
        public static bool IsFirstVersionOfFunding(this IFundingVersionDetail fundingVersionDetail)
        {
            return IsFirstVersionOfFunding(fundingVersionDetail.FundingId, fundingVersionDetail.StatementChannelVersion);
        }
    }
}
