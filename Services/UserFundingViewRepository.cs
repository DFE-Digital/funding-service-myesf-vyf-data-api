using Azure.Data.Tables;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using PDS.ViewYourFunding.Data.Services.Extensions;
using PDS.ViewYourFunding.Data.Services.Helpers;
using PDS.ViewYourFunding.Data.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Services
{
    /// <inheritdoc cref="IUserFundingViewRepository"/>
    public class UserFundingViewRepository : IUserFundingViewRepository
    {
        private readonly TableServiceClient _tableServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFundingViewRepository"/> class.
        /// </summary>
        /// <param name="azureTableStorageClient">The azure table storage client.</param>
        public UserFundingViewRepository(IAzureTableStorageClient azureTableStorageClient)
        {
            _tableServiceClient = azureTableStorageClient.TableServiceClient;
        }

        /// <inheritdoc/>
        public async Task AddOrReplace(IUserFundingView entity)
        {
            TableClient azTableUserView = _tableServiceClient.GetTableClient(nameof(UserFundingView));

            await azTableUserView.UpsertEntityAsync(entity);
        }

        /// <inheritdoc/>
        public async Task<bool> HasUserVisitedFunding(string userId, string fundingId)
        {
            return await CheckUserVisitedFunding(userId, fundingId);
        }

        /// <inheritdoc/>
        public async Task<IUserFundingViewCount> GetUserFundingViewCount(string userId, IEnumerable<IFundingVersionDetail> fundingVersionDetails)
        {
            var result = new UserFundingViewCount { UserId = userId };

            foreach (var fundingVersionDetail in fundingVersionDetails)
            {
                if (!await CheckUserVisitedFunding(userId, fundingVersionDetail.FundingId))
                {
                    if (fundingVersionDetail.IsFirstVersionOfFunding())
                    {
                        result.UnreadNewFundings++;
                        continue;
                    }

                    result.UnreadUpdatedFundings++;
                }
            }

            return result;
        }

        private async Task<bool> CheckUserVisitedFunding(string userId, string fundingId)
        {
            var azTableUserFundingView = _tableServiceClient.GetTableClient(nameof(UserFundingView));

            return await azTableUserFundingView
                        .QueryAsync<TableEntity>(a => a.PartitionKey == userId && a.RowKey == fundingId)
                        .Select(a => a.RowKey)
                        .AnyAsync();
        }
    }
}