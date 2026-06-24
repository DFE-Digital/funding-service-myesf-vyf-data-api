using AutoMapper;
using FundingApi.DTOs;
using PDS.ViewYourFunding.Data.API.DTOs;
using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using PDS.ViewYourFunding.Data.Services.Models;

namespace PDS.ViewYourFunding.Data.API
{
    /// <summary>
    /// The AutoMapper Profile.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
        /// </summary>
        public AutoMapperProfile()
        {
            CreateMap<FundingApiSearchFilterParameters, SearchFilterParameters>();
            CreateMap<FundingApiSearchFundingStreamParameters, FundingStreamParameters>();
            CreateMap<AzureFundingSearchDocument, FundingApiSearchFunding>();
            CreateMap<AzureProviderFundingSearchDocument, FundingApiSearchProviderFunding>();
            CreateMap<UserFundingViewCount, UserFundingViewCountResponse>();
            CreateMap<IFundingSearchDocument, FundingApiSearchFunding>();
        }
    }
}