using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using FluentAssertions;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.ViewYourFunding.Data.API.Controllers;
using PDS.ViewYourFunding.Data.API.DTOs;
using PDS.ViewYourFunding.Data.API.Extensions;
using PDS.ViewYourFunding.Data.API.Interfaces;
using PDS.ViewYourFunding.Data.Core;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using PDS.ViewYourFunding.Data.Services;
using PDS.ViewYourFunding.Data.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Tests.Integration
{
    /// <summary>
    /// The Funding controller tests.
    /// </summary>
    [TestClass]
    public class FundingControllerTests
    {
        /// <summary>
        /// The mapper.
        /// </summary>
        private static IMapper _mapper;

        /// <summary>
        /// The mock azure search funding alias client.
        /// </summary>
        private readonly Mock<IAzureSearchAliasClient<AzureFundingSearchDocument>> _mockAzureSearchFundingAliasClient
            = new Mock<IAzureSearchAliasClient<AzureFundingSearchDocument>>(MockBehavior.Strict);

        /// <summary>
        /// The mock azure search provider funding alias client.
        /// </summary>
        private readonly Mock<IAzureSearchAliasClient<AzureProviderFundingSearchDocument>> _mockAzureSearchProviderFundingAliasClient
            = new Mock<IAzureSearchAliasClient<AzureProviderFundingSearchDocument>>(MockBehavior.Strict);

        /// <summary>
        /// The mock azure search alias client manager.
        /// </summary>
        private readonly Mock<IAzureSearchAliasClientManager> _mockAzureSearchAliasClientManager
            = new Mock<IAzureSearchAliasClientManager>(MockBehavior.Strict);

        #region Initialization

        /// <summary>
        /// Initializes the class.
        /// </summary>
        /// <param name="context">The test context.</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TypeAdapterConfig config = new TypeAdapterConfig();
            config.Configure();
            _mapper = new Mapper(config);
        }

        #endregion

        /// <summary>
        /// Mocks the search service manager.
        /// </summary>
        /// <returns>The Mock Azure Search service manager.</returns>
        public Mock<IAzureSearchServiceManager> MockSearchServiceManager()
        {
            var returnItem = new Mock<IAzureSearchServiceManager>(MockBehavior.Strict);

            returnItem
                .Setup(item => item.DoesIndexAndIndexerExist<AzureFundingSearchDocument>(It.IsAny<int>()))
                .ReturnsAsync(true);

            returnItem
                .Setup(item => item.DoesIndexAndIndexerExist<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .ReturnsAsync(true);

            return returnItem;
        }

        /// <summary>
        /// Mocks the background task queue.
        /// </summary>
        /// <returns>The back ground task queue.</returns>
        public IBackgroundTaskQueue MockBackgroundTaskQueue() => new Mock<IBackgroundTaskQueue>().Object;

        #region GetFunding Tests

        /// <summary>
        /// Gets the funding empty.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task GetFunding_Empty()
        {
            // Arrange
            var documents = new List<AzureFundingSearchDocument>();
            IFundingApiSearchFunding expectedResult = null;

            _mockAzureSearchFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchFundingAliasClient.Object);
            var controller = GetFundingController();

            // Act
            var actualResult = await controller.GetFunding(It.IsAny<string>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Gets the funding one result.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task GetFunding_OneResult()
        {
            // Arrange
            var documents = new List<AzureFundingSearchDocument>
            {
                new AzureFundingSearchDocument
                {
                    Id = "1"
                }
            };

            IFundingApiSearchFunding expectedResult = new FundingApiSearchFunding
            {
                Id = "1",
                ProviderFundings = Enumerable.Empty<string>(),
                ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
            };

            _mockAzureSearchFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetSearchResult(documents));

            _mockAzureSearchProviderFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetProviderFundingSearchResult(null));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchFundingAliasClient.Object);

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.GetFunding(It.IsAny<string>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Gets the funding one result.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task GetFunding_OneResult_SpecialLAGrouping()
        {
            // Arrange
            var documents = new List<AzureFundingSearchDocument>
            {
                new AzureFundingSearchDocument
                {
                    Id = "1",
                    GroupingType = "LocalAuthorityMss",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            IFundingApiSearchFunding expectedResult = new FundingApiSearchFunding
            {
                Id = "1",
                ProviderFundings = Enumerable.Empty<string>(),
                GroupingType = "LocalAuthorityMss",
                ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
            };

            _mockAzureSearchFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetSearchResult(documents));

            _mockAzureSearchProviderFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetProviderFundingSearchResult(null));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchFundingAliasClient.Object);

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.GetFunding(It.IsAny<string>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion


        #region SearchFundings Tests

        /// <summary>
        /// Searches the fundings empty.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchFundings_Empty()
        {
            // Arrange
            var documents = new List<AzureFundingSearchDocument>();
            var expectedResult = new FundingApiSearchFundingResponse
            {
                Funding = new List<FundingApiSearchFunding>()
            };

            _mockAzureSearchFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the fundings one provider with one version.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchFundings_OneProviderWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureFundingSearchDocument>
            {
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "12345678",
                    FundingVersion = "1.0",
                    GroupingType = "LocalAuthority",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            var expectedResult = new FundingApiSearchFundingResponse
            {
                Funding = new List<FundingApiSearchFunding>
                {
                    new FundingApiSearchFunding
                    {
                        GroupUkprn = "12345678",
                        FundingVersion = "1.0",
                        GroupingType = "LocalAuthority",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockAzureSearchFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchFundingAliasClient.Object);

            _mockAzureSearchProviderFundingAliasClient
              .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
              .ReturnsAsync(GetProviderFundingSearchResult(null));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the fundings one provider with multiple versions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchFundings_OneProviderWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureFundingSearchDocument>
            {
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "12345678",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "12345678",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "12345678",
                    FundingVersion = "3.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            var expectedResult = new FundingApiSearchFundingResponse
            {
                Funding = new List<FundingApiSearchFunding>
                {
                    new FundingApiSearchFunding
                    {
                        GroupUkprn = "12345678",
                        FundingVersion = "3.0",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockAzureSearchFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchFundingAliasClient.Object);

            _mockAzureSearchProviderFundingAliasClient
              .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
              .ReturnsAsync(GetProviderFundingSearchResult(null));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the fundings multiple providers each with one version.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchFundings_MultipleProvidersEachWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureFundingSearchDocument>
            {
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "12345678",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "22345678",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupName = "A B C",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
            };

            var expectedResult = new FundingApiSearchFundingResponse
            {
                Funding = new List<FundingApiSearchFunding>
                {
                    new FundingApiSearchFunding
                    {
                        GroupUkprn = "12345678",
                        FundingVersion = "1.0",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchFunding
                    {
                        GroupUkprn = "22345678",
                        FundingVersion = "1.0",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchFunding
                    {
                        GroupName = "A B C",
                        FundingVersion = "1.0",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                },
            };

            _mockAzureSearchFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the fundings multiple providers each with multiple versions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchFundings_MultipleProvidersEachWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureFundingSearchDocument>
            {
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "12345678",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                     GroupUkprn = "22345678",
                     FundingVersion = "1.0",
                     ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                     GroupUkprn = "32345678",
                     FundingVersion = "1.0",
                     ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "12345678",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "22345678",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "32345678",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "32345678",
                    FundingVersion = "3.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
            };

            var expectedResult = new FundingApiSearchFundingResponse
            {
                Funding = new List<FundingApiSearchFunding>
                {
                    new FundingApiSearchFunding
                    {
                        GroupUkprn = "12345678",
                        FundingVersion = "2.0",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchFunding
                    {
                        GroupUkprn = "22345678",
                        FundingVersion = "2.0",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchFunding
                    {
                        GroupUkprn = "32345678",
                        FundingVersion = "3.0",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                },
            };

            _mockAzureSearchFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion


        #region SearchProviderFundings Tests

        /// <summary>
        /// Searches the provider fundings empty.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchProviderFundings_Empty()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>();
            var expectedResult = new FundingApiSearchProviderFundingResponse
            {
                ProviderFunding = Enumerable.Empty<IFundingApiSearchProviderFunding>()
            };

            _mockAzureSearchProviderFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetProviderFundingSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            _mockAzureSearchProviderFundingAliasClient
              .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
              .ReturnsAsync(GetProviderFundingSearchResult(null));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the provider fundings one provider with one version.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchProviderFundings_OneProviderWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>
            {
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "12345678",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            var expectedResult = new FundingApiSearchProviderFundingResponse
            {
                ProviderFunding = new List<FundingApiSearchProviderFunding>
                {
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "12345678",
                        FundingVersion = "1.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockAzureSearchProviderFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetProviderFundingSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the provider fundings one provider with multiple versions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchProviderFundings_OneProviderWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>
            {
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "12345678",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "12345678",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            var expectedResult = new FundingApiSearchProviderFundingResponse
            {
                ProviderFunding = new List<FundingApiSearchProviderFunding>
                {
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "12345678",
                        FundingVersion = "2.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockAzureSearchProviderFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetProviderFundingSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the provider fundings multiple providers each with one version.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchProviderFundings_MultipleProvidersEachWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>
            {
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "10000001",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "10000002",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            var expectedResult = new FundingApiSearchProviderFundingResponse
            {
                ProviderFunding = new List<FundingApiSearchProviderFunding>
                {
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "10000001",
                        FundingVersion = "2.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "10000002",
                        FundingVersion = "1.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockAzureSearchProviderFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetProviderFundingSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the provider fundings multiple providers each with multiple versions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchProviderFundings_MultipleProvidersEachWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>
            {
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "10000001",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "10000002",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "10000003",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "10000001",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "10000002",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "10000003",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "10000003",
                    FundingVersion = "3.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
            };

            var expectedResult = new FundingApiSearchProviderFundingResponse
            {
                ProviderFunding = new List<FundingApiSearchProviderFunding>
                {
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "10000001",
                        FundingVersion = "2.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "10000002",
                        FundingVersion = "2.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "10000003",
                        FundingVersion = "3.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockAzureSearchProviderFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetProviderFundingSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion

        #region SearchLatestProviderFundings Tests

        /// <summary>
        /// Searches the latest provider fundings empty.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchLatestProviderFundings_Empty()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>();

            _mockAzureSearchProviderFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetProviderFundingSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            _mockAzureSearchProviderFundingAliasClient
              .Setup(c => c.SearchDocumentsAsync(It.IsAny<string>(), It.IsAny<SearchOptions>()))
              .ReturnsAsync(GetProviderFundingSearchResult(null));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchLatestProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.ProviderFunding.Should().BeEmpty();
        }

        /// <summary>
        /// Searches the provider fundings one provider with one version.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchProviderLatestFundings_OneProviderWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>
            {
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "12345678",
                    FundingVersion = "1_0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            var expectedResult = new FundingApiSearchProviderFundingResponse
            {
                ProviderFunding = new List<FundingApiSearchProviderFunding>
                {
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "12345678",
                        FundingVersion = "1_0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockAzureSearchProviderFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetProviderFundingSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchLatestProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the provider fundings multiple providers each with one version.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task SearchLatestProviderFundings_MultipleProvidersEachWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>
            {
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "10000001",
                    FundingVersion = "2_0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "10000002",
                    FundingVersion = "1_0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            var expectedResult = new FundingApiSearchProviderFundingResponse
            {
                ProviderFunding = new List<FundingApiSearchProviderFunding>
                {
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "10000001",
                        FundingVersion = "2_0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "10000002",
                        FundingVersion = "1_0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockAzureSearchProviderFundingAliasClient
                .Setup(c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchOptions>()))
                .ReturnsAsync(GetProviderFundingSearchResult(documents));

            _mockAzureSearchAliasClientManager
                .Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()))
                .Returns(_mockAzureSearchProviderFundingAliasClient.Object);

            var controller = GetFundingController();

            // Act
            var actualResult = await controller.SearchLatestProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Mocks the configuration service.
        /// </summary>
        /// <returns>The mock configuration service.</returns>
        private Mock<IOptions<ApplicationConfiguration>> MockConfigurationService()
        {
            var mockConfiguration = new Mock<IOptions<ApplicationConfiguration>>(MockBehavior.Strict);

            mockConfiguration
                .Setup(c => c.Value)
                .Returns(new ApplicationConfiguration());

            return mockConfiguration;
        }

        /// <summary>
        /// Mocks the logger service.
        /// </summary>
        /// <returns>The mock logger service.</returns>
        private Mock<ILogger<FundingController>> MockFundingControllerLoggerService() => new Mock<ILogger<FundingController>>(MockBehavior.Loose);

        /// <summary>
        /// Mocks the search service logger service.
        /// </summary>
        /// <returns>The Mock logger.</returns>
        private Mock<ILogger<AzureFundingSearchService>> MockSearchServiceLoggerService() => new Mock<ILogger<AzureFundingSearchService>>(MockBehavior.Loose);

        /// <summary>
        /// Gets the funding controller.
        /// </summary>
        /// <returns>The Funding controller.</returns>
        private FundingController GetFundingController()
        {
            return new FundingController(GetAzureFundingSearchService(), MockFundingControllerLoggerService().Object, _mapper);
        }

        /// <summary>
        /// Gets the azure funding search service.
        /// </summary>
        /// <returns>The AzureFundingSearchService.</returns>
        private AzureFundingSearchService GetAzureFundingSearchService()
        {
            return new AzureFundingSearchService(
                _mockAzureSearchAliasClientManager.Object,
                MockConfigurationService().Object.Value,
                MockSearchServiceManager().Object,
                MockSearchServiceLoggerService().Object,
                MockBackgroundTaskQueue());
        }

        /// <summary>
        /// Gets the search result.
        /// </summary>
        /// <param name="documents">The documents.</param>
        /// <returns>The AzureFundingSearchDocument.</returns>
        private SearchResults<AzureFundingSearchDocument> GetSearchResult(List<AzureFundingSearchDocument> documents)
        {
            return SearchModelFactory.SearchResults<AzureFundingSearchDocument>(documents.Select(d => SearchModelFactory.SearchResult<AzureFundingSearchDocument>(d, 0.0, null)), null, null, null, null);
        }

        /// <summary>
        /// Gets the provider funding search result.
        /// </summary>
        /// <param name="documents">The documents.</param>
        /// <returns>The AzureProviderFundingSearchDocument.</returns>
        private SearchResults<AzureProviderFundingSearchDocument> GetProviderFundingSearchResult(List<AzureProviderFundingSearchDocument> documents)
        {
            return SearchModelFactory.SearchResults<AzureProviderFundingSearchDocument>(
                documents != null ?
                documents.Select(d => SearchModelFactory.SearchResult<AzureProviderFundingSearchDocument>(d, 0.0, null)).ToList()
                : new List<SearchResult<AzureProviderFundingSearchDocument>>(),
                null,
                null,
                null,
                null);
        }

        #endregion
    }
}