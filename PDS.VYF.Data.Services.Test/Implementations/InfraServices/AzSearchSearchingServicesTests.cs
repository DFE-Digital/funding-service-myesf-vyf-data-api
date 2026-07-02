using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using FluentAssertions;
using Moq;
using PDS.VYF.Data.Services.Abstracts.InfraServices;
using PDS.VYF.Data.Services.Enums;
using PDS.VYF.Data.Services.Implementations.InfraServices;
using PDS.VYF.Data.Services.Models.AzSearchModels;

namespace PDS.ViewYourFunding.Data.Tests.Implementations.InfraServices
{
    /// <summary>
    /// The Azure Search Searching Services Tests.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class AzSearchSearchingServicesTests
    {
        private readonly MockRepository mockRepository;

        private readonly Mock<IAzSearchCosmosServices> mockAzSearchCosmosServices;
        private readonly Mock<SearchIndexClient> mockSearchIndexClient;
        private readonly Mock<SearchClient> mockSearchClient;

        private AzSearchSearchingServices azSearchServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzSearchSearchingServicesTests"/> class.
        /// </summary>
        public AzSearchSearchingServicesTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockAzSearchCosmosServices = this.mockRepository.Create<IAzSearchCosmosServices>();
            this.mockSearchIndexClient = this.mockRepository.Create<SearchIndexClient>();
            this.mockSearchClient = this.mockRepository.Create<SearchClient>();

            azSearchServices = new AzSearchSearchingServices(
                this.mockAzSearchCosmosServices.Object,
                this.mockSearchIndexClient.Object);
        }

        /// <summary>
        /// Searches the document asynchronous verify method call success.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task SearchDocumentAsync_VerifyMethodCall_Success()
        {
            // Arrange
            List<LoggedInChildAzSearchModel> expectedResult = new()
            {
                new () { Id = "GAG-AC-2425-12345678-1_0" },
                new () { Id = "GAG-AC-2425-12345678-2_0" },
            };

            var mockResults = SearchModelFactory.SearchResults(
                new[]
                    {
                        SearchModelFactory.SearchResult(new LoggedInChildAzSearchModel { Id = "GAG-AC-2425-12345678-1_0" }, 1.0, null, null),
                        SearchModelFactory.SearchResult(new LoggedInChildAzSearchModel { Id = "GAG-AC-2425-12345678-2_0" }, 0.9, null, null),
                    },
                2,
                null,
                null,
                Mock.Of<Response>(a => a.IsError == false));


            this.mockAzSearchCosmosServices.Setup(a => a.GetContainerName(It.IsAny<AzSearchIndexTypeEnum>())).Returns("containerName");


            this.mockSearchIndexClient.Setup(x => x.GetSearchClient(It.IsAny<string>())).Returns(this.mockSearchClient.Object);

            this.mockSearchClient
                .Setup(a => a.SearchAsync<LoggedInChildAzSearchModel>(It.IsAny<string>(), It.IsAny<SearchOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(mockResults, Mock.Of<Response>(a => a.IsError == false)));


            AzSearchIndexTypeEnum azSearchIndexType = AzSearchIndexTypeEnum.LoggedIn_Child;
            string filters = "OrganisationUkprn eq '10086466' and FundingStreamPeriod eq 'GAG-AC-2425'";
            string searchText = "*";
            string selectFields = "Id, StatementType";

            // Act
            var result = new List<LoggedInChildAzSearchModel>();

            await foreach (var update in azSearchServices.SearchDocumentAsync<LoggedInChildAzSearchModel>(
                                azSearchIndexType,
                                searchText,
                                filters,
                                selectFields))
            {
                result.Add(update);
            }

            // Assert
            this.mockRepository.VerifyAll();
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
