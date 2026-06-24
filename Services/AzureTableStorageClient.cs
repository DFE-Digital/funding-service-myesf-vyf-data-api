using Azure.Core;
using Azure.Data.Tables;
using PDS.ViewYourFunding.Data.Interfaces;
using System;

namespace PDS.ViewYourFunding.Data.Services
{
    /// <inheritdoc cref="IAzureTableStorageClient"/>
    public class AzureTableStorageClient : IAzureTableStorageClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableStorageClient"/> class.
        /// </summary>
        /// <param name="accountName">The azure storage account name.</param>
        /// <param name="accountKey">The azure storage account key.</param>
        public AzureTableStorageClient(string accountName, string accountKey)
        {
            var storageUri = new Uri($"https://{accountName}.table.core.windows.net/");
            var creds = new TableSharedKeyCredential(accountName, accountKey);

            TableServiceClient = new TableServiceClient(
                storageUri,
                creds,
                new TableClientOptions()
                {
                    Retry =
                            {
                                Delay = TimeSpan.FromSeconds(2),
                                MaxRetries = 5,
                                Mode = RetryMode.Exponential,
                                MaxDelay = TimeSpan.FromSeconds(10),
                                NetworkTimeout = TimeSpan.FromSeconds(100)
                            },
                });
        }

        /// <inheritdoc />
        public TableServiceClient TableServiceClient { get; }
    }
}