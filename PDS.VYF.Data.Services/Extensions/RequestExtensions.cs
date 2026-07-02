namespace PDS.VYF.Data.Services.Extensions
{
    using PDS.VYF.Data.Services.Models.AzSearchModels;
    using PDS.VYF.Data.Services.Models.RequestModels;
    using System.Text;

    /// <summary>
    /// The class which holds all the extension methods for Request.
    /// </summary>
    public static class RequestExtensions
    {
        /// <summary>
        /// Builds the filter.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The stringbuilder.
        /// </returns>
        public static StringBuilder BuildFilter(this ChildRequest request)
        {
            return new StringBuilder()
                .AddSearchInFilter(request.ListOfIds, nameof(LoggedInChildAzSearchModel.Id))
                .AddSearchInFilter(request.ListOfUKPRNs, nameof(LoggedInChildAzSearchModel.OrganisationUkprn))
                .AddSearchInFilter(request.FundingStreamPeriods, nameof(LoggedInChildAzSearchModel.FundingStreamPeriod))
                .AppendEqFilter(request.FundingStreamCode, nameof(LoggedInChildAzSearchModel.FundingStreamCode))
                .AppendEqFilter(request.StatusChangedDateOnly, nameof(LoggedInChildAzSearchModel.StatusChangedDateOnly));
        }

        /// <summary>
        /// Builds the filter.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The stringbuilder.</returns>
        public static StringBuilder BuildFilter(this ParentRequest request)
        {
            return new StringBuilder()
                .AddSearchInFilter(request.ListOfIds, nameof(LoggedInParentAzSearchModel.Id))
                .AddSearchInFilter(request.ListOfUKPRNs, nameof(LoggedInParentAzSearchModel.GroupUkprn))
                .AppendEqFilter(request.FundingStreamCode, nameof(LoggedInParentAzSearchModel.FundingStreamCode))
                .AddSearchInFilter(request.FundingStreamPeriods, nameof(LoggedInParentAzSearchModel.FundingStreamPeriod));
        }
    }
}
