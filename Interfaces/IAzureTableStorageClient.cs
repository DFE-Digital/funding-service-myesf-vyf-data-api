using Azure.Data.Tables;

namespace PDS.ViewYourFunding.Data.Interfaces
{
    /// <summary>
    /// Sets up the cloud table client to access azure table storage.
    /// </summary>
    public interface IAzureTableStorageClient
    {
        /// <summary>
        /// Gets the azure table service client.
        /// </summary>
        TableServiceClient TableServiceClient { get; }
    }
}