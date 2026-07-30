using FundingApi.DTOs;
using Mapster;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PDS.ViewYourFunding.Data.API.DTOs;
using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using PDS.ViewYourFunding.Data.Services.Models;


namespace PDS.ViewYourFunding.Data.API.Extensions
{
    /// <summary>
    /// extention added for mapster.
    /// </summary>
    public static class TypeAdapterConfigExtensions
    {
        /// <summary>
        /// Download a spreadsheet.
        /// </summary>
        /// <param name="config">The internal filename as it is in storage.</param>
        public static void Configure(this TypeAdapterConfig config)
        {
            config.Default.AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);
            config.NewConfig<FundingApiSearchFilterParameters, SearchFilterParameters>();
            config.NewConfig<FundingApiSearchFundingStreamParameters, FundingStreamParameters>();
            config.NewConfig<AzureProviderFundingSearchDocument, FundingApiSearchProviderFunding>();
            config.NewConfig<UserFundingViewCount, UserFundingViewCountResponse>();
            config.NewConfig<IFundingSearchDocument, FundingApiSearchFunding>();
            config.NewConfig<AzureFundingSearchDocument, FundingApiSearchFunding>();
        }
    }
}
