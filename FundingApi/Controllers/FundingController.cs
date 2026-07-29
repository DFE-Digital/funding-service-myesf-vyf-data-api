using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PDS.ViewYourFunding.Data.API.DTOs;
using PDS.ViewYourFunding.Data.API.Helpers;
using PDS.ViewYourFunding.Data.API.Interfaces;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.API.Controllers
{
    /// <summary>
    /// Funding api controller.
    /// </summary>
    [Authorize(Policy = nameof(ToggleAuthorizeRequirement))]
    [Route("api/[controller]")]
    [ApiController]
    public class FundingController : ControllerBase
    {
        private const string GroupingTypeLocalAuthorityMss = "LocalAuthorityMss";
        private const string GroupingTypeLocalAuthoritySsf = "LocalAuthoritySsf";
        private const string GroupingTypeLocalAuthority = "LocalAuthority";

        private const string PupilPremiumFundingStreamCode = "PP";

        private readonly string[] _keysForGroupTypeCode = new[] { "Country", "SpecialAcademies", "PupilReferralUnit", "Mainstream", "AlternativeProvision", "AcademyAlternativeProvision", "NonMaintainedSpecialSchools", "LocalAuthorityMss" };

        private readonly IFundingSearchService _fundingSearchService;
        private readonly ILogger<FundingController> _loggerService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FundingController"/> class.
        /// </summary>
        /// <param name="fundingSearchService">The service to use for searching for fundings.</param>
        /// <param name="loggerService">Service to use for logging.</param>
        /// <param name="mapper">The AutoMapper mapper.</param>
        public FundingController(
            IFundingSearchService fundingSearchService,
            ILogger<FundingController> loggerService,
            IMapper mapper)
        {
            _fundingSearchService = fundingSearchService;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        #region Public actions

        /// <summary>
        /// Get a single instance of funding by its id.
        /// </summary>
        /// <param name="id">A funding id to lookup.</param>
        /// <param name="waitForIndexBuild">Should we await the index (re)build (if it necessary), or cancel with an error.</param>
        /// <returns>A single instance of funding, with its associated provider fundings - or null.</returns>
        [HttpGet("GetFunding")]
        public async Task<IFundingApiSearchFunding> GetFunding(string id, bool waitForIndexBuild = false)
        {
            var searchResult = await _fundingSearchService.GetFunding(id, waitForIndexBuild);
            GroupFunding(searchResult);

            return GetSearchFundingResponse(searchResult).Funding?.FirstOrDefault();
        }

        /// <summary>
        /// Get a single instance of provider funding by its id.
        /// </summary>
        /// <param name="id">A provider funding id to lookup.</param>
        /// <param name="waitForIndexBuild">Should we await the index (re)build (if it necessary), or cancel with an error.</param>
        /// <returns>A single instance of provider funding.</returns>
        [HttpGet("GetProviderFunding")]
        public async Task<IFundingApiSearchProviderFunding> GetProviderFunding(string id, bool waitForIndexBuild = false)
        {
            var searchResult = await _fundingSearchService.GetProviderFundingForId(id, waitForIndexBuild);

            return GetSearchProviderFunding(searchResult);
        }

        /// <summary>
        /// Search for the latest provider funding.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A response object containing the list of matching funding providers.</returns>
        [HttpPost("SearchLatestProviderFunding")]
        public async Task<IFundingApiSearchProviderFundingResponse> SearchLatestProviderFunding(FundingApiSearchRequest request)
        {
            _loggerService.LogInformation($"Funding API initiated for {nameof(SearchLatestProviderFunding)} - Search Term = {JsonConvert.SerializeObject(request ?? new FundingApiSearchRequest())}");

            var fundingStreamParameters = GetFundingStreamParameters(request);

            var searchResult = await _fundingSearchService.SearchLatestProviderFunding(
                fundingStreamParameters,
                request?.SearchTerm,
                request?.WaitForIndexBuild ?? false);

            _loggerService.LogInformation($"Funding API completed {nameof(SearchLatestProviderFunding)} -  Search request = {JsonConvert.SerializeObject(request ?? new FundingApiSearchRequest())}");

            return GetSearchProviderFundingResponse(searchResult, request?.BypassGrouping ?? false);
        }

        /// <summary>
        /// Search for provider funding.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A response object containing the list of matching funding providers.</returns>
        [HttpPost("SearchProviderFunding")]
        public async Task<IFundingApiSearchProviderFundingResponse> SearchProviderFunding(FundingApiSearchRequest request)
        {
            _loggerService.LogInformation($"Funding API initiated for {nameof(SearchProviderFunding)} - Search Term = {JsonConvert.SerializeObject(request ?? new FundingApiSearchRequest())}");

            var fundingStreamParameters = GetFundingStreamParameters(request);

            var searchResult = await _fundingSearchService.SearchProviderFunding(
                fundingStreamParameters,
                request?.SearchTerm,
                request?.WaitForIndexBuild ?? false);

            _loggerService.LogInformation($"Funding API completed {nameof(SearchProviderFunding)} -  Search request = {JsonConvert.SerializeObject(request ?? new FundingApiSearchRequest())}");

            return GetSearchProviderFundingResponse(searchResult, request?.BypassGrouping ?? false);
        }

        /// <summary>
        /// Search for fundings by group.
        /// </summary>
        /// <param name="request">The funding APi search request.</param>
        /// <returns>A response object containing the list of matching groups. If only one group is found, the response also contains all of the
        /// fundings for provider fundings under that group.</returns>
        [HttpPost("SearchFunding")]
        public async Task<IFundingApiSearchFundingResponse> SearchFunding(FundingApiSearchRequest request)
        {
            _loggerService.LogInformation($"Funding API initiated for {nameof(SearchFunding)} - Search request = {JsonConvert.SerializeObject(request ?? new FundingApiSearchRequest())}");
            var fundingStreamParameters = GetFundingStreamParameters(request);

            var searchResult = await _fundingSearchService.SearchFunding(
                fundingStreamParameters,
                request?.SearchTerm,
                request?.WaitForIndexBuild ?? false);

            if (request?.BypassGrouping != true)
            {
                GroupFunding(searchResult);
            }

            _loggerService.LogInformation($"Funding API completed {nameof(SearchFunding)} -  Search request = {JsonConvert.SerializeObject(request ?? new FundingApiSearchRequest())}");

            return GetSearchFundingResponse(searchResult);
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Converts the search result from the search service into a response object.
        /// </summary>
        /// <param name="searchResult">The search result from the service.</param>
        private void GroupFunding(ISearchResult<IFundingSearchDocument> searchResult)
        {
            var groupedFunding = searchResult?.Documents?.GroupBy(document => new
            {
                GroupIdentifier = GetGroupIdentifer(document),
                document.FundingPeriodCode,
                document.FundingStreamCode,
                document.GroupingReason
            }).ToList();

            var newDocuments = new List<IFundingSearchDocument>();

            if (searchResult?.Documents != null)
            {
                foreach (var groupedFundingInstance in groupedFunding)
                {
                    var newestResult = groupedFundingInstance
                        .OrderByDescending(document => double.TryParse(document.FundingVersion?.Replace("_", "."), out var fundingVersion) ? fundingVersion : -1.0)
                        .ThenByDescending(document => document.StatusChangedDate)
                        .First();
                    newDocuments.Add(newestResult);
                }

                searchResult.Documents = newDocuments;
            }
        }

        /// <summary>
        /// Converts the search result from the search service into a response object.
        /// </summary>
        /// <param name="searchResult">The search result from the service.</param>
        /// <returns>The search response object.</returns>
        private IFundingApiSearchFundingResponse GetSearchFundingResponse(ISearchResult<IFundingSearchDocument> searchResult)
        {
            if (searchResult == null)
            {
                return new FundingApiSearchFundingResponse
                {
                    Funding = new List<IFundingApiSearchFunding>()
                };
            }

            var result = new FundingApiSearchFundingResponse
            {
                Funding = searchResult.Documents?.Select(document => _mapper.Map<FundingApiSearchFunding>(document))
                    ?? new List<IFundingApiSearchFunding>().AsEnumerable()
            };

            return result;
        }

        /// <summary>
        /// Converts the search result from the search service into a response object.
        /// </summary>
        /// <param name="searchResult">The search result from the service.</param>
        /// <returns>The search response object.</returns>
        private IFundingApiSearchProviderFunding GetSearchProviderFunding(IProviderFundingSearchDocument searchResult)
        {
            if (searchResult == null)
            {
                return new FundingApiSearchProviderFunding();
            }

            return _mapper.Map<FundingApiSearchProviderFunding>(searchResult);
        }

        /// <summary>
        /// Converts the search result from the search service into a response object.
        /// </summary>
        /// <param name="searchResult">The search result from the service.</param>
        /// <param name="bypassGrouping">Bypass grouping and return all results.</param>
        /// <returns>The search response object.</returns>
        private IFundingApiSearchProviderFundingResponse GetSearchProviderFundingResponse(
            ISearchResult<IProviderFundingSearchDocument> searchResult,
            bool bypassGrouping)
        {
            if (searchResult == null)
            {
                return null;
            }

            if (bypassGrouping)
            {
                return new FundingApiSearchProviderFundingResponse
                {
                    ProviderFunding = searchResult.Documents?
                        .Select(document => _mapper.Map<FundingApiSearchProviderFunding>(document))
                        .ToList()
                };
            }

            var providerFunding = searchResult.Documents?
                .GroupBy(document => new
                {
                    document.OrganisationUkprn,
                    document.FundingPeriodCode,
                    document.FundingStreamCode,
                    ParentId = ParentIdExcludingVersion(document.ParentId)
                })?
                .Select(group => group
                    .OrderByDescending(document => double.TryParse(document.FundingVersion.Replace("_", "."), out var p) ? p : -1.0)
                    .ThenByDescending(document => document.StatusChangedDate)
                    .First())?
                .Select(document => _mapper.Map<FundingApiSearchProviderFunding>(document))
                .ToList();

            return new FundingApiSearchProviderFundingResponse
            {
                ProviderFunding = providerFunding
            };
        }

        /// <summary>
        /// Get the parent id, ignoring the version component.
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <returns>A parent id without the version component.</returns>
        private string ParentIdExcludingVersion(string parentId)
        {
            if (string.IsNullOrEmpty(parentId))
            {
                return parentId;
            }

            var lastHypen = parentId.LastIndexOf('-');
            return parentId.Substring(0, lastHypen);
        }

        /// <summary>
        /// Get the funding stream parameters.
        /// </summary>
        /// <param name="request">The request param.</param>
        /// <returns>An array of FundingStreamParameters.</returns>
        private FundingStreamParameters[] GetFundingStreamParameters(FundingApiSearchRequest request)
        {
            return request
                ?.FundingStreams
                ?.Select(f => _mapper.Map<FundingStreamParameters>(f))
                ?.ToArray();
        }

        /// <summary>
        /// Get the group identifier.
        /// </summary>
        /// <param name="document">The funding search document.</param>
        /// <returns>An IFundingSearchDocument containing the group identifier.</returns>
        private string GetGroupIdentifer(IFundingSearchDocument document)
        {
            var isSpecialLaType =
                document.GroupingType == GroupingTypeLocalAuthorityMss
                || document.GroupingType == GroupingTypeLocalAuthoritySsf;

            if (isSpecialLaType && document.FundingStreamCode != PupilPremiumFundingStreamCode)
            {
                return document.Id;
            }

            if (document.GroupingType == GroupingTypeLocalAuthority)
            {
                return document.GroupCode;
            }

            if (document.FundingStreamCode == PupilPremiumFundingStreamCode && _keysForGroupTypeCode.Contains(document.GroupingType))
            {
                return document.GroupingType;
            }

            if (!string.IsNullOrEmpty(document.GroupUkprn))
            {
                return document.GroupUkprn;
            }

            return document.SearchableGroupName;
        }

        #endregion
    }
}