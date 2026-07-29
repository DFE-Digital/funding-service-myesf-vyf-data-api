using FundingApi.DTOs;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PDS.ViewYourFunding.Data.API.Helpers;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Services.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FundingApi.Controllers
{
    /// <summary>
    /// User api controller.
    /// </summary>
    [Authorize(Policy = nameof(ToggleAuthorizeRequirement))]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserFundingViewRepository _userFundingViewRepository;
        private readonly ILogger<UserController> _loggerService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userFundingViewRepository">The user funding view repository.</param>
        /// <param name="loggerService">Service to use for logging.</param>
        /// <param name="mapper">The AutoMapper mapper.</param>
        public UserController(
            IUserFundingViewRepository userFundingViewRepository,
            ILogger<UserController> loggerService,
            IMapper mapper)
        {
            _userFundingViewRepository = userFundingViewRepository;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        /// <summary>
        /// Checks whether the user has already viewed the funding.
        /// </summary>
        /// <param name="userId">The user id to lookup.</param>
        /// <param name="fundingId">The funding id to lookup.</param>
        /// <returns>True if user has viewed the funding else false.</returns>
        [HttpGet("HasUserVisitedFunding")]
        public async Task<bool> HasUserVisitedFunding(string userId, string fundingId)
        {
            try
            {
                return await _userFundingViewRepository.HasUserVisitedFunding(userId, fundingId);
            }
            catch (Exception exception)
            {
                _loggerService.LogError($"Error occurred while checking HasUserVisitedFunding UserId: {userId} and FundingId: {fundingId} due to {exception.Message}");
                return false;
            }
        }

        /// <summary>
        /// Adds or replaces an instance of user funding funding view details.
        /// </summary>
        /// <param name="request">The user funding view detail to add.</param>
        /// <returns>The asynchronous task.</returns>
        [HttpPost("AddUserFundingViewDetail")]
        public async Task<bool?> AddUserFundingViewDetail(AddUserFundingViewRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.UserId) || string.IsNullOrWhiteSpace(request?.FundingId))
                {
                    return null;
                }

                await _userFundingViewRepository.AddOrReplace(new UserFundingView(request.UserId, request.FundingId) { ViewedAt = request.ViewedAt });

                return true;
            }
            catch (Exception exception)
            {
                _loggerService.LogError($"Error occurred while saving User visit details for: UserId: {request.UserId} and FundingId: {request?.FundingId} due to {exception.Message}");
                return null;
            }
        }

        /// <summary>
        /// Returns a response representing the number of unread new or updated fundings for user.
        /// </summary>
        /// <param name="request">The request with the user id and funding ids to check on.</param>
        /// <returns>The asynchronous task.</returns>
        [HttpPost("GetUserFundingViewCount")]
        public async Task<UserFundingViewCountResponse> GetUserFundingViewCount(UserFundingViewCountRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.UserId) || request?.FundingVersionDetails == null)
            {
                return null;
            }

            try
            {
                var result = await _userFundingViewRepository.GetUserFundingViewCount(request.UserId, request.FundingVersionDetails);

                return _mapper.Map<UserFundingViewCountResponse>(result);
            }
            catch (Exception exception)
            {
                _loggerService.LogError($"Error occurred in GetUserFundingViewCount User visit details for: UserId: {request.UserId} and fundingIds : {string.Join(",", request.FundingVersionDetails.Select(a => a.FundingId).ToArray())} due to {exception.Message}");
                return null;
            }
        }
    }
}