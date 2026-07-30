using FundingApi.DTOs;
using Mapster;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PDS.ViewYourFunding.Data.API.DTOs;
using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using PDS.ViewYourFunding.Data.Services.Models;


namespace FundingApi.Extentions
{
    /// <summary>
    /// extention added for mapster.
    /// </summary>
    public static class TypeAdapterConfigExtentions
    {
        /// <summary>
        /// Download a spreadsheet.
        /// </summary>
        /// <param name="config">The internal filename as it is in storage.</param>
        public static void Configure(this TypeAdapterConfig config)
        {
            config.NewConfig<FundingApiSearchFilterParameters, SearchFilterParameters>();
            config.NewConfig<FundingApiSearchFundingStreamParameters, FundingStreamParameters>()
            .AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);
            config.NewConfig<AzureProviderFundingSearchDocument, FundingApiSearchProviderFunding>()
            .AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);
            config.NewConfig<UserFundingViewCount, UserFundingViewCountResponse>();
            config.NewConfig<IFundingSearchDocument, FundingApiSearchFunding>()
            .AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);
            config.NewConfig<AzureFundingSearchDocument, FundingApiSearchFunding>()
            .AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);
        }
    }
}
