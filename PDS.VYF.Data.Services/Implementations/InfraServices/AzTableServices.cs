namespace PDS.VYF.Data.Services.Implementations.InfraServices
{
    using Azure.Data.Tables;
    using PDS.VYF.Data.Services.Abstracts.InfraServices;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The Class for Azure Table Services.
    /// </summary>
    /// <seealso cref="PDS.VYF.Data.Services.Abstracts.InfraServices.IAzTableServices" />
    public class AzTableServices : IAzTableServices
    {
        private const string UserFundingView = "UserFundingView";
        private readonly TableServiceClient tableServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzTableServices" /> class.
        /// </summary>
        /// <param name="tableServiceClient">The table service client.</param>
        public AzTableServices(TableServiceClient tableServiceClient)
        {
            this.tableServiceClient = tableServiceClient;
        }

        /// <summary>
        /// Gets the view children identifier asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Dictionary<string, bool>> GetViewChildrenIdAsync(string userId)
        {
            var azTableUserFundingView = this.tableServiceClient.GetTableClient(UserFundingView);

            return await azTableUserFundingView
                            .QueryAsync<TableEntity>(a => a.PartitionKey == userId)
                            .Select(a => a.RowKey)
                            .ToDictionaryAsync(a => a, a => true);
        }

        /// <summary>
        /// Determines whether [is user viewed provider funding identifier] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="providerFundingId">The provider funding identifier.</param>
        /// <returns>
        ///   <c>true</c> if [is user viewed provider funding identifier] [the specified user identifier]; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsUserViewedProviderFundingId(string userId, string providerFundingId)
        {
            var azTableUserFundingView = this.tableServiceClient.GetTableClient(UserFundingView);

            return await azTableUserFundingView
                            .QueryAsync<TableEntity>(a => a.PartitionKey == userId && a.RowKey == providerFundingId)
                            .Select(a => a.RowKey)
                            .AnyAsync();
        }

        /// <summary>
        /// Upserts the entity asynchronous.
        /// </summary>
        /// <param name="tableEntity">The table entity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UpsertEntityAsync(TableEntity tableEntity)
        {
            var azTableUserFundingView = this.tableServiceClient.GetTableClient(UserFundingView);

            var response = await azTableUserFundingView.UpsertEntityAsync(tableEntity);

            return response.Status == 200;
        }
    }
}
