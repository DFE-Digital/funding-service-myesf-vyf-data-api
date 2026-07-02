using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Logging;
using PDS.ViewYourFunding.Data.Core;
using PDS.VYF.Data.Services.Abstracts.InfraServices;
using PDS.VYF.Data.Services.Enums;
using PDS.VYF.Data.Services.Extensions;
using PDS.VYF.Data.Services.Models.InfraModels;
using System.Net;

namespace PDS.VYF.Data.Services.Implementations.InfraServices
{
    /// <summary>
    /// The Service class for Azure Search Manager.
    /// </summary>
    /// <seealso cref="PDS.VYF.Data.Services.Abstracts.InfraServices.IAzSearchManagingServices" />
    public class AzSearchManagingServices : IAzSearchManagingServices
    {
        private readonly SearchIndexClient searchIndexClient;
        private readonly SearchIndexerClient searchIndexerClient;

        private readonly IAzSearchCosmosServices azSearchCosmosServices;
        private readonly ApplicationConfiguration appConfiguration;
        private readonly ILogger<AzSearchManagingServices> logger;
        private readonly IAzSearchThumbPrintServices azSearchThumbPrintServices;
        private readonly string cosmosConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzSearchManagingServices" /> class.
        /// </summary>
        /// <param name="azSearchCosmosServices">The az search cosmos services.</param>
        /// <param name="appConfiguration">The application configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="azSearchThumbPrintServices">The az search thumb print services.</param>
        /// <param name="searchIndexClient">The search index client.</param>
        /// <param name="searchIndexerClient">The search indexer client.</param>
        public AzSearchManagingServices(
            ILogger<AzSearchManagingServices> logger,
            ApplicationConfiguration appConfiguration,
            IAzSearchCosmosServices azSearchCosmosServices,
            IAzSearchThumbPrintServices azSearchThumbPrintServices,
            SearchIndexClient searchIndexClient,
            SearchIndexerClient searchIndexerClient)
        {
            this.logger = logger;
            this.azSearchCosmosServices = azSearchCosmosServices;
            this.azSearchThumbPrintServices = azSearchThumbPrintServices;
            this.appConfiguration = appConfiguration;

            this.searchIndexClient = searchIndexClient;
            this.searchIndexerClient = searchIndexerClient;
            this.cosmosConnectionString = this.appConfiguration.Repositories.CosmosDb.ConnectionString;
        }

        /// <summary>
        /// Creates the or replace az search resources.
        /// </summary>
        /// <param name="azSearchIndexType">Type of the az search index.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if all operations completed as expected. If any of the operation failed return false.</returns>
        public async Task<bool> CreateOrReplaceAzSearchResources(AzSearchIndexTypeEnum azSearchIndexType, CancellationToken cancellationToken)
        {
            var azSearchManagingServiceModel = this.GetModel(azSearchIndexType);

            azSearchManagingServiceModel.HasToBeReplaced = await this.FindHasToBeReplaced(azSearchManagingServiceModel, cancellationToken);

            if (azSearchManagingServiceModel.HasToBeReplaced)
            {
                await this.DeleteAzSearchResourcesIfExists(azSearchManagingServiceModel, cancellationToken);
            }

            await this.CreateAzSearchResourcesIfNotExists(azSearchManagingServiceModel, cancellationToken);

            return true;
        }

        private AzSearchManagingServiceModel GetModel(AzSearchIndexTypeEnum azSearchIndexType)
        {
            var cosmosQuery = this.azSearchCosmosServices.GetCosmosQuery(azSearchIndexType);
            var cosmosContainerName = this.azSearchCosmosServices.GetContainerName(azSearchIndexType);
            var typeOfField = this.azSearchCosmosServices.GetIndexFieldType(azSearchIndexType);

            var typesForThumbPrint = this.azSearchCosmosServices.GetIndexTypesForThumbPrint(azSearchIndexType);
            List<string> keyParamsForThumPrint = new() { this.appConfiguration.Repositories.CosmosDb.ConnectionString, cosmosQuery, cosmosContainerName };
            var thumbPrint = this.azSearchThumbPrintServices.AzIndexThumbPrint(keyParamsForThumPrint, typesForThumbPrint);

            return new AzSearchManagingServiceModel
            {
                AzSearchIndexType = azSearchIndexType,
                IndexName = azSearchIndexType.GetIndexName(cosmosContainerName),
                IndexerName = azSearchIndexType.GetIndexerName(cosmosContainerName),
                DatasourceName = azSearchIndexType.GetDatasourceName(cosmosContainerName),
                TypeOfField = typeOfField,
                TypesForThumbPrint = typesForThumbPrint,
                ThumbPrint = thumbPrint,
                CosmosContainerName = cosmosContainerName,
                CosmosQuery = cosmosQuery,
                HasToBeReplaced = false
            };
        }

        private async Task<bool> FindHasToBeReplaced(AzSearchManagingServiceModel azSearchManagingServiceModel, CancellationToken cancellationToken)
        {
            if (await this.IsResourceExists(this.searchIndexerClient.GetDataSourceConnectionAsync, azSearchManagingServiceModel.DatasourceName, cancellationToken))
            {
                var dataSource = await this.searchIndexerClient.GetDataSourceConnectionAsync(azSearchManagingServiceModel.DatasourceName);

                if (!string.IsNullOrWhiteSpace(dataSource?.Value?.Description))
                {
                    return !dataSource.Value.Description.Contains(azSearchManagingServiceModel.DescriptionWithThumbPrint);
                }
            }

            return true;
        }

        private async Task<bool> DeleteAzSearchResourcesIfExists(AzSearchManagingServiceModel azSearchManagingServiceModel, CancellationToken cancellationToken)
        {
            bool isAllDeletedSuccessfully = true;

            if (await this.IsResourceExists(this.searchIndexerClient.GetIndexerAsync, azSearchManagingServiceModel.IndexerName, cancellationToken))
            {
                var isIndexerDeleted = await this.DeleteAndLog(azSearchManagingServiceModel.IndexerName, "Indexer", cancellationToken, this.searchIndexerClient.DeleteIndexerAsync);
                isAllDeletedSuccessfully &= isIndexerDeleted;
            }

            if (await this.IsResourceExists(this.searchIndexClient.GetIndexAsync, azSearchManagingServiceModel.IndexName, cancellationToken))
            {
                var isIndexDeleted = await this.DeleteAndLog(azSearchManagingServiceModel.IndexName, "Index", cancellationToken, this.searchIndexClient.DeleteIndexAsync);
                isAllDeletedSuccessfully &= isIndexDeleted;
            }

            if (await this.IsResourceExists(this.searchIndexerClient.GetDataSourceConnectionAsync, azSearchManagingServiceModel.DatasourceName, cancellationToken))
            {
                var isDSDeleted = await this.DeleteAndLog(azSearchManagingServiceModel.DatasourceName, "Datasource Connection", cancellationToken, this.searchIndexerClient.DeleteDataSourceConnectionAsync);
                isAllDeletedSuccessfully &= isDSDeleted;
            }

            return isAllDeletedSuccessfully;
        }

        private async Task<bool> CreateAzSearchResourcesIfNotExists(AzSearchManagingServiceModel azSearchManagingServiceModel, CancellationToken cancellationToken)
        {
            bool isAllCreatedSuccessfully = true;

            if (!await this.IsResourceExists(this.searchIndexerClient.GetDataSourceConnectionAsync, azSearchManagingServiceModel.DatasourceName, cancellationToken))
            {
                var isDsCreated = await this.CreateAndLog(azSearchManagingServiceModel.DatasourceName, "Datasource Connection", async () =>
                {
                    SearchIndexerDataContainer indexerDataContainer = new(azSearchManagingServiceModel.CosmosContainerName)
                    {
                        Query = azSearchManagingServiceModel.CosmosQuery
                    };

                    var dataSourceConnection = new SearchIndexerDataSourceConnection(
                                                        azSearchManagingServiceModel.DatasourceName,
                                                        SearchIndexerDataSourceType.CosmosDb,
                                                        this.cosmosConnectionString,
                                                        indexerDataContainer)
                    {
                        DataChangeDetectionPolicy = new HighWaterMarkChangeDetectionPolicy("_ts"),
                        Description = azSearchManagingServiceModel.DescriptionWithThumbPrintAndCreateDateTime,
                    };

                    return await this.searchIndexerClient.CreateDataSourceConnectionAsync(dataSourceConnection, cancellationToken);
                });

                isAllCreatedSuccessfully &= isDsCreated;
            }

            if (!await this.IsResourceExists(this.searchIndexClient.GetIndexAsync, azSearchManagingServiceModel.IndexName, cancellationToken))
            {
                var isIndexCreated = await this.CreateAndLog(azSearchManagingServiceModel.IndexName, "Index", async () =>
                {
                    return await this.searchIndexClient.CreateIndexAsync(
                                        new SearchIndex(azSearchManagingServiceModel.IndexName, new FieldBuilder().Build(azSearchManagingServiceModel.TypeOfField)),
                                        cancellationToken);
                });

                isAllCreatedSuccessfully &= isIndexCreated;
            }

            if (!await this.IsResourceExists(this.searchIndexerClient.GetIndexerAsync, azSearchManagingServiceModel.IndexerName, cancellationToken))
            {
                var isIndexerCreated = await this.CreateAndLog(azSearchManagingServiceModel.IndexerName, "Indexer", async () =>
                {
                    SearchIndexer searchIndexer = new(azSearchManagingServiceModel.IndexerName, azSearchManagingServiceModel.DatasourceName, azSearchManagingServiceModel.IndexName)
                    {
                        Schedule = new(TimeSpan.FromMinutes(30)),
                        Parameters = new() { BatchSize = 100, },
                        Description = azSearchManagingServiceModel.DescriptionWithThumbPrintAndCreateDateTime,
                    };

                    return await this.searchIndexerClient.CreateIndexerAsync(searchIndexer, cancellationToken);
                });

                isAllCreatedSuccessfully &= isIndexerCreated;
            }

            return isAllCreatedSuccessfully;
        }

        // Helper Methods
        private async Task<bool> IsResourceExists<T>(Func<string, CancellationToken, Task<Response<T>>> getResourceMethod, string name, CancellationToken cancellationToken)
            where T : class
        {
            try
            {
                // Attempt to get the Azure Resource. If it exists, the response will contain the resource
                var response = await getResourceMethod(name, cancellationToken);
                return response?.Value != null;
            }
            catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                // If a 404 error is thrown, the azure resource does not exist.
                return false;
            }
        }

        private async Task<bool> CreateAndLog<T>(string resourceName, string resourceType, Func<Task<Azure.Response<T>>> createFunc)
        {
            try
            {
                var response = await createFunc();

                if (!response.GetRawResponse().IsError)
                {
                    this.logger.LogInformation($"Azure Search {resourceType} : {resourceName} created successfully.");
                    return true;
                }
                else
                {
                    this.logger.LogError($"Error while creating Azure Search {resourceType} : {resourceName}.");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while creating Azure Search {resourceType} : {resourceName}.");
            }

            return false;
        }

        private async Task<bool> DeleteAndLog(string resourceName, string resourceType, CancellationToken cancellation, Func<string, CancellationToken, Task<Response>> deleteFunc)
        {
            try
            {
                var response = await deleteFunc(resourceName, cancellation);

                if (!response.IsError)
                {
                    this.logger.LogInformation($"Azure Search {resourceType} : {resourceName} deleted successfully.");
                    return true;
                }
                else
                {
                    this.logger.LogError($"Error while deleting Azure Search {resourceType} : {resourceName}.");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while deleting Azure Search {resourceType} : {resourceName}.");
            }

            return false;
        }

        private async Task<bool> CreateAzDatasource(AzSearchManagingServiceModel azSearchManagingServiceModel, CancellationToken cancellationToken)
        {
            try
            {
                SearchIndexerDataContainer indexerDataContainer = new(azSearchManagingServiceModel.CosmosContainerName)
                {
                    Query = azSearchManagingServiceModel.CosmosQuery
                };

                var dataSourceConnection = new SearchIndexerDataSourceConnection(
                                                    azSearchManagingServiceModel.DatasourceName,
                                                    SearchIndexerDataSourceType.CosmosDb,
                                                    this.cosmosConnectionString,
                                                    indexerDataContainer)
                {
                    DataChangeDetectionPolicy = new HighWaterMarkChangeDetectionPolicy("_ts"),
                    Description = azSearchManagingServiceModel.DescriptionWithThumbPrintAndCreateDateTime,
                };

                var response = await this.searchIndexerClient.CreateDataSourceConnectionAsync(dataSourceConnection, cancellationToken);

                return !response.GetRawResponse().IsError;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while Creating Azure Search Datasource : {azSearchManagingServiceModel.DatasourceName}.");
                return false;
            }
        }

        private async Task<bool> CreateAzIndex(AzSearchManagingServiceModel azSearchManagingServiceModel, CancellationToken cancellationToken)
        {
            try
            {
                FieldBuilder fieldBuilder = new();
                var fields = fieldBuilder.Build(azSearchManagingServiceModel.TypeOfField);
                SearchIndex newSearchIndex = new(azSearchManagingServiceModel.IndexName, fields);

                var response = await this.searchIndexClient.CreateIndexAsync(newSearchIndex, cancellationToken);

                return !response.GetRawResponse().IsError;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while Creating Azure Search Index : {azSearchManagingServiceModel.IndexName}.");
                return false;
            }
        }

        private async Task<bool> CreateAzIndexer(AzSearchManagingServiceModel azSearchManagingServiceModel, CancellationToken cancellationToken)
        {
            try
            {
                SearchIndexer searchIndexer = new(azSearchManagingServiceModel.IndexerName, azSearchManagingServiceModel.DatasourceName, azSearchManagingServiceModel.IndexName)
                {
                    Schedule = new(TimeSpan.FromMinutes(30)),
                    Parameters = new() { BatchSize = 100, },
                    Description = azSearchManagingServiceModel.DescriptionWithThumbPrintAndCreateDateTime,
                };

                var response = await this.searchIndexerClient.CreateIndexerAsync(searchIndexer, cancellationToken);

                return !response.GetRawResponse().IsError;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error while Creating Azure Search Indexer : {azSearchManagingServiceModel.IndexName}.");
                return false;
            }
        }
    }
}
