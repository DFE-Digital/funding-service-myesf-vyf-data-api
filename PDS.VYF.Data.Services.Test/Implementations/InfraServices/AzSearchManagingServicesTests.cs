using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PDS.ViewYourFunding.Data.Core;
using PDS.VYF.Data.Services.Abstracts.InfraServices;
using PDS.VYF.Data.Services.Enums;
using PDS.VYF.Data.Services.Implementations.InfraServices;
using PDS.VYF.Data.Services.Models.AzSearchModels;
using PDS.VYF.Data.Services.Models.InfraModels;
using PDS.VYF.Data.Services.Tests.Mocks.ExternalClasses;
using System.Net;
using System.Net.NetworkInformation;

namespace PDS.VYF.Data.Services.Tests.Implementations.InfraServices
{
    /// <summary>
    /// The Azure search managing services tests.
    /// </summary>
    [TestClass]
    public class AzSearchManagingServicesTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<IAzSearchCosmosServices> mockAzSearchCosmosServices;
        private readonly Mock<IOptions<ApplicationConfiguration>> mockOptions;
        private readonly Mock<ILogger<AzSearchManagingServices>> mockLogger;
        private readonly Mock<IAzSearchThumbPrintServices> mockAzSearchThumbPrintServices;
        private readonly Mock<SearchIndexClient> mockSearchIndexClient;
        private readonly Mock<SearchIndexerClient> mockSearchIndexerClient;
        private readonly ApplicationConfiguration applicationConfiguration;

        private AzSearchManagingServices azSearchManagingServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzSearchManagingServicesTests"/> class.
        /// </summary>
        public AzSearchManagingServicesTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            mockAzSearchCosmosServices = mockRepository.Create<IAzSearchCosmosServices>();
            mockOptions = mockRepository.Create<IOptions<ApplicationConfiguration>>();
            mockLogger = new MockLogger<AzSearchManagingServices>(true);
            mockAzSearchThumbPrintServices = mockRepository.Create<IAzSearchThumbPrintServices>();
            mockSearchIndexClient = mockRepository.Create<SearchIndexClient>();
            mockSearchIndexerClient = mockRepository.Create<SearchIndexerClient>();

            applicationConfiguration = new ApplicationConfiguration()
            {
                Repositories = new ViewYourFunding.Data.Core.Configuration.RepositoriesConfiguration
                {
                    CosmosDb = new CosmosDbConfiguration
                    {
                        ConnectionString = "Fake connection string"
                    }
                }
            };

            azSearchManagingServices = new AzSearchManagingServices(
                mockLogger.Object,
                applicationConfiguration,
                mockAzSearchCosmosServices.Object,
                mockAzSearchThumbPrintServices.Object,
                mockSearchIndexClient.Object,
                mockSearchIndexerClient.Object);
        }

        /// <summary>
        /// Creates the or replace az search resources all resources available resources replace required should return true.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task CreateOrReplaceAzSearchResources_AllResourcesAvailable_ResourcesReplaceRequired_ShouldReturnTrue()
        {
            // Arrange
            AzSearchIndexTypeEnum azSearchIndexType = AzSearchIndexTypeEnum.LoggedIn_Parent;
            CancellationToken cancellationToken = CancellationToken.None;


            mockAzSearchCosmosServices.Setup(x => x.GetContainerName(azSearchIndexType)).Returns("containerName");
            mockAzSearchCosmosServices.Setup(x => x.GetCosmosQuery(azSearchIndexType)).Returns("cosmosQuery");
            mockAzSearchCosmosServices.Setup(x => x.GetIndexFieldType(azSearchIndexType)).Returns(typeof(LoggedInParentAzSearchModel));
            mockAzSearchCosmosServices.Setup(x => x.GetIndexTypesForThumbPrint(azSearchIndexType)).Returns(new Type[] { typeof(LoggedInParentAzSearchModel), typeof(LoggedInCalculation) });

            mockAzSearchThumbPrintServices.Setup(x => x.AzIndexThumbPrint(It.IsAny<List<string>>(), It.IsAny<Type[]>())).Returns(123);

            // Refer how to mock Azure Search Clients https://learn.microsoft.com/en-us/dotnet/azure/sdk/unit-testing-mocking?tabs=moq
            var fakeSearchIndexerDataSourceConnection = new SearchIndexerDataSourceConnection(
                "Datasource Name",
                SearchIndexerDataSourceType.CosmosDb,
                "Connection String",
                new SearchIndexerDataContainer("Container Name"))
            {
                Description = @$"
* Important: The description is used by VYF DataApi to recreate search resources!! Don't delete or change any of the description contents including this line!

Thumb Print             - 987
Az Search Index Type    - {AzSearchIndexTypeEnum.LoggedIn_Parent}
Cosmos Container Name   - containerName
Length of Cosmos Query  - {"cosmosQuery".Length}
",
            };

            var fakeIndexer = new SearchIndexer("Fake Indexer Name", "Datasource Name", "Index Name");
            var fakeIndex = new SearchIndex("Fake Index Name");

            // Get Mocks
            mockSearchIndexerClient
                .SetupSequence(a => a.GetDataSourceConnectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeSearchIndexerDataSourceConnection, Mock.Of<Response>(a => a.IsError == false)))
                .ReturnsAsync(Response.FromValue(fakeSearchIndexerDataSourceConnection, Mock.Of<Response>(a => a.IsError == false)))
                .ReturnsAsync(Response.FromValue(fakeSearchIndexerDataSourceConnection, Mock.Of<Response>(a => a.IsError == false)))
                .ThrowsAsync(new Azure.RequestFailedException((int)HttpStatusCode.NotFound, "Sample Error"));

            mockSearchIndexerClient
                .SetupSequence(a => a.GetIndexerAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeIndexer, Mock.Of<Response>(a => a.IsError == false)))
                .ThrowsAsync(new Azure.RequestFailedException((int)HttpStatusCode.NotFound, "Sample Error"));


            mockSearchIndexClient
                .SetupSequence(a => a.GetIndexAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeIndex, Mock.Of<Response>(a => a.IsError == false)))
                .ThrowsAsync(new Azure.RequestFailedException((int)HttpStatusCode.NotFound, "Sample Error"));


            // Create Mocks
            mockSearchIndexerClient
                .Setup(a => a.CreateDataSourceConnectionAsync(It.IsAny<SearchIndexerDataSourceConnection>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeSearchIndexerDataSourceConnection, Mock.Of<Response>(a => a.IsError == false)));

            mockSearchIndexerClient
                .Setup(a => a.CreateIndexerAsync(It.IsAny<SearchIndexer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeIndexer, Mock.Of<Response>(a => a.IsError == false)));

            mockSearchIndexClient
                .Setup(a => a.CreateIndexAsync(It.IsAny<SearchIndex>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeIndex, Mock.Of<Response>(a => a.IsError == false)));

            // Delete Mocks
            mockSearchIndexerClient
                .Setup(a => a.DeleteDataSourceConnectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<Response>(a => a.IsError == false));

            mockSearchIndexerClient
                .Setup(a => a.DeleteIndexerAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<Response>(a => a.IsError == false));

            mockSearchIndexClient
                .Setup(a => a.DeleteIndexAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<Response>(a => a.IsError == false));

            // Act
            var result = await azSearchManagingServices.CreateOrReplaceAzSearchResources(azSearchIndexType, cancellationToken);

            // Assert
            result.Should().BeTrue();
            mockRepository.VerifyAll();
        }

        /// <summary>
        /// Creates the or replace az search resources all resources available resource recreation not required should return true.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task CreateOrReplaceAzSearchResources_AllResourcesAvailable_ResourceRecreationNotRequired_ShouldReturnTrue()
        {
            // Arrange
            AzSearchIndexTypeEnum azSearchIndexType = AzSearchIndexTypeEnum.LoggedIn_Parent;
            CancellationToken cancellationToken = CancellationToken.None;


            mockAzSearchCosmosServices.Setup(x => x.GetContainerName(azSearchIndexType)).Returns("containerName");
            mockAzSearchCosmosServices.Setup(x => x.GetCosmosQuery(azSearchIndexType)).Returns("cosmosQuery");
            mockAzSearchCosmosServices.Setup(x => x.GetIndexFieldType(azSearchIndexType)).Returns(typeof(LoggedInParentAzSearchModel));
            mockAzSearchCosmosServices.Setup(x => x.GetIndexTypesForThumbPrint(azSearchIndexType)).Returns(new Type[] { typeof(LoggedInParentAzSearchModel), typeof(LoggedInCalculation) });

            mockAzSearchThumbPrintServices.Setup(x => x.AzIndexThumbPrint(It.IsAny<List<string>>(), It.IsAny<Type[]>())).Returns(123);

            // Refer how to mock Azure Search Clients https://learn.microsoft.com/en-us/dotnet/azure/sdk/unit-testing-mocking?tabs=moq
            var fakeSearchIndexerDataSourceConnection = new SearchIndexerDataSourceConnection(
                "Datasource Name",
                SearchIndexerDataSourceType.CosmosDb,
                "Connection String",
                new SearchIndexerDataContainer("Container Name"))
            {
                Description = @$"
* Important: The description is used by VYF DataApi to recreate search resources!! Don't delete or change any of the description contents including this line!

Thumb Print             - 123
Az Search Index Type    - {AzSearchIndexTypeEnum.LoggedIn_Parent}
Cosmos Container Name   - containerName
Length of Cosmos Query  - {"cosmosQuery".Length}
",
            };

            var fakeIndexer = new SearchIndexer("Fake Indexer Name", "Datasource Name", "Index Name");
            var fakeIndex = new SearchIndex("Fake Index Name");

            // Get Mocks
            mockSearchIndexerClient
                .Setup(a => a.GetDataSourceConnectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeSearchIndexerDataSourceConnection, Mock.Of<Response>(a => a.IsError == false)));

            mockSearchIndexerClient
                .Setup(a => a.GetIndexerAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeIndexer, Mock.Of<Response>(a => a.IsError == false)));


            mockSearchIndexClient
                .Setup(a => a.GetIndexAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeIndex, Mock.Of<Response>(a => a.IsError == false)));


            // Act
            var result = await azSearchManagingServices.CreateOrReplaceAzSearchResources(azSearchIndexType, cancellationToken);

            // Assert
            result.Should().BeTrue();
            mockRepository.VerifyAll();
        }


        /// <summary>
        /// Creates the or replace az search resources all resources not available resources created a fresh should return true.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task CreateOrReplaceAzSearchResources_AllResourcesNotAvailable_ResourcesCreatedAFresh_ShouldReturnTrue()
        {
            // Arrange
            AzSearchIndexTypeEnum azSearchIndexType = AzSearchIndexTypeEnum.LoggedIn_Parent;
            CancellationToken cancellationToken = CancellationToken.None;


            mockAzSearchCosmosServices.Setup(x => x.GetContainerName(azSearchIndexType)).Returns("containerName");
            mockAzSearchCosmosServices.Setup(x => x.GetCosmosQuery(azSearchIndexType)).Returns("cosmosQuery");
            mockAzSearchCosmosServices.Setup(x => x.GetIndexFieldType(azSearchIndexType)).Returns(typeof(LoggedInParentAzSearchModel));
            mockAzSearchCosmosServices.Setup(x => x.GetIndexTypesForThumbPrint(azSearchIndexType)).Returns(new Type[] { typeof(LoggedInParentAzSearchModel), typeof(LoggedInCalculation) });

            mockAzSearchThumbPrintServices.Setup(x => x.AzIndexThumbPrint(It.IsAny<List<string>>(), It.IsAny<Type[]>())).Returns(123);

            // Refer how to mock Azure Search Clients https://learn.microsoft.com/en-us/dotnet/azure/sdk/unit-testing-mocking?tabs=moq
            var fakeSearchIndexerDataSourceConnection = new SearchIndexerDataSourceConnection(
                "Datasource Name",
                SearchIndexerDataSourceType.CosmosDb,
                "Connection String",
                new SearchIndexerDataContainer("Container Name"))
            {
                Description = "987",
            };

            var fakeIndexer = new SearchIndexer("Fake Indexer Name", "Datasource Name", "Index Name");
            var fakeIndex = new SearchIndex("Fake Index Name");

            // Get Mocks
            mockSearchIndexerClient
                .Setup(a => a.GetDataSourceConnectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Azure.RequestFailedException((int)HttpStatusCode.NotFound, "Sample Error"));

            mockSearchIndexerClient
                .Setup(a => a.GetIndexerAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Azure.RequestFailedException((int)HttpStatusCode.NotFound, "Sample Error"));


            mockSearchIndexClient
                .Setup(a => a.GetIndexAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Azure.RequestFailedException((int)HttpStatusCode.NotFound, "Sample Error"));


            // Create Mocks
            mockSearchIndexerClient
                .Setup(a => a.CreateDataSourceConnectionAsync(It.IsAny<SearchIndexerDataSourceConnection>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeSearchIndexerDataSourceConnection, Mock.Of<Response>(a => a.IsError == false)));

            mockSearchIndexerClient
                .Setup(a => a.CreateIndexerAsync(It.IsAny<SearchIndexer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeIndexer, Mock.Of<Response>(a => a.IsError == false)));

            mockSearchIndexClient
                .Setup(a => a.CreateIndexAsync(It.IsAny<SearchIndex>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeIndex, Mock.Of<Response>(a => a.IsError == false)));

            // Act
            var result = await azSearchManagingServices.CreateOrReplaceAzSearchResources(azSearchIndexType, cancellationToken);

            // Assert
            result.Should().BeTrue();
            mockRepository.VerifyAll();
        }

        /// <summary>
        /// Creates the or replace az search resources ds exists index indexer not exist ds not replaced index indexer created a fresh should return true.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task CreateOrReplaceAzSearchResources_DSExistsIndexIndexerNotExist_DSNotReplacedIndexIndexerCreatedAFresh_ShouldReturnTrue()
        {
            // Arrange
            AzSearchIndexTypeEnum azSearchIndexType = AzSearchIndexTypeEnum.LoggedIn_Parent;
            CancellationToken cancellationToken = CancellationToken.None;


            mockAzSearchCosmosServices.Setup(x => x.GetContainerName(azSearchIndexType)).Returns("containerName");
            mockAzSearchCosmosServices.Setup(x => x.GetCosmosQuery(azSearchIndexType)).Returns("cosmosQuery");
            mockAzSearchCosmosServices.Setup(x => x.GetIndexFieldType(azSearchIndexType)).Returns(typeof(LoggedInParentAzSearchModel));
            mockAzSearchCosmosServices.Setup(x => x.GetIndexTypesForThumbPrint(azSearchIndexType)).Returns(new Type[] { typeof(LoggedInParentAzSearchModel), typeof(LoggedInCalculation) });

            mockAzSearchThumbPrintServices.Setup(x => x.AzIndexThumbPrint(It.IsAny<List<string>>(), It.IsAny<Type[]>())).Returns(123);

            // Refer how to mock Azure Search Clients https://learn.microsoft.com/en-us/dotnet/azure/sdk/unit-testing-mocking?tabs=moq
            var fakeSearchIndexerDataSourceConnection = new SearchIndexerDataSourceConnection(
                "Datasource Name",
                SearchIndexerDataSourceType.CosmosDb,
                "Connection String",
                new SearchIndexerDataContainer("Container Name"))
            {
                Description = @$"
* Important: The description is used by VYF DataApi to recreate search resources!! Don't delete or change any of the description contents including this line!

Thumb Print             - 123
Az Search Index Type    - {AzSearchIndexTypeEnum.LoggedIn_Parent}
Cosmos Container Name   - containerName
Length of Cosmos Query  - {"cosmosQuery".Length}
",
            };

            var fakeIndexer = new SearchIndexer("Fake Indexer Name", "Datasource Name", "Index Name");
            var fakeIndex = new SearchIndex("Fake Index Name");

            // Get Mocks
            mockSearchIndexerClient
                .Setup(a => a.GetDataSourceConnectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeSearchIndexerDataSourceConnection, Mock.Of<Response>(a => a.IsError == false)));

            mockSearchIndexerClient
                .Setup(a => a.GetIndexerAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Azure.RequestFailedException((int)HttpStatusCode.NotFound, "Sample Error"));


            mockSearchIndexClient
                .Setup(a => a.GetIndexAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Azure.RequestFailedException((int)HttpStatusCode.NotFound, "Sample Error"));


            // Create Mocks
            mockSearchIndexerClient
                .Setup(a => a.CreateIndexerAsync(It.IsAny<SearchIndexer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeIndexer, Mock.Of<Response>(a => a.IsError == false)));

            mockSearchIndexClient
                .Setup(a => a.CreateIndexAsync(It.IsAny<SearchIndex>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(fakeIndex, Mock.Of<Response>(a => a.IsError == false)));

            // Act
            var result = await azSearchManagingServices.CreateOrReplaceAzSearchResources(azSearchIndexType, cancellationToken);

            // Assert
            result.Should().BeTrue();
            mockRepository.VerifyAll();
        }
    }
}