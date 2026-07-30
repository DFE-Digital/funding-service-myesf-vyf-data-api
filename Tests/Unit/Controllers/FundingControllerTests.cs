using FluentAssertions;
using FundingApi.Extentions;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.ViewYourFunding.Data.API.Controllers;
using PDS.ViewYourFunding.Data.API.DTOs;
using PDS.ViewYourFunding.Data.API.Interfaces;
using PDS.ViewYourFunding.Data.Core;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using PDS.ViewYourFunding.Data.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Tests.Unit.Controllers
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
        /// The mock funding search service.
        /// </summary>
        private readonly Mock<IFundingSearchService> _mockFundingSearchService = new Mock<IFundingSearchService>(MockBehavior.Strict);

        /// <summary>
        /// The mock search service manager.
        /// </summary>
        private readonly Mock<IAzureSearchServiceManager> _mockSearchServiceManager = new Mock<IAzureSearchServiceManager>(MockBehavior.Strict);

        /// <summary>
        /// The mock configuration service.
        /// </summary>
        private readonly Mock<IOptions<ApplicationConfiguration>> _mockConfigurationService = new Mock<IOptions<ApplicationConfiguration>>(MockBehavior.Strict);

        /// <summary>
        /// The mock logger service.
        /// </summary>
        private readonly Mock<ILogger<FundingController>> _mockLoggerService = new Mock<ILogger<FundingController>>(MockBehavior.Loose);

        #region Initialization

        /// <summary>
        /// Classes the initialize.
        /// </summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TypeAdapterConfig config = new TypeAdapterConfig();
            config.Configure();
            _mapper = new Mapper(config);
        }

        #endregion


        #region GetFunding Tests

        /// <summary>
        /// Gets the funding returns one funding.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task GetFunding_ReturnsOneFunding()
        {
            var id = Guid.NewGuid().ToString();

            // Arrange
            _mockFundingSearchService.Setup(o => o.GetFunding(id, false)).ReturnsAsync(GetRawSearchResult(id));
            _mockFundingSearchService.Setup(o => o.GetProviderFunding(id, false)).ReturnsAsync(new AzureSearchResult<IProviderFundingSearchDocument>());
            var fundingController = GetFundingController();

            IFundingApiSearchFunding expected = new FundingApiSearchFunding
            {
                Id = id,
                ProviderFundings = Enumerable.Empty<string>(),
                ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
            };

            // Act
            var result = await fundingController.GetFunding(id);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// Gets the funding returns zero fundings.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task GetFunding_ReturnsZeroFundings()
        {
            var id = Guid.NewGuid().ToString();

            // Arrange
            _mockFundingSearchService.Setup(o => o.GetFunding(id, false)).ReturnsAsync(new AzureSearchResult<IFundingSearchDocument>());
            _mockFundingSearchService.Setup(o => o.GetProviderFunding(id, false)).ReturnsAsync(new AzureSearchResult<IProviderFundingSearchDocument>());

            var fundingController = GetFundingController();

            IFundingApiSearchFunding expected = null;

            // Act
            var result = await fundingController.GetFunding(id);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion


        #region GetProviderFunding Tests

        /// <summary>
        /// Gets the provider funding.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task GetProviderFunding()
        {
            // Arrange
            var id = "Provider funding id";
            var expected = new FundingApiSearchProviderFunding
            {
                Id = id,
                OrganisationName = "Organisation Name",
                ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
            };

            _mockFundingSearchService.Setup(o => o.GetProviderFundingForId(id, false)).ReturnsAsync(new AzureProviderFundingSearchDocument
            {
                Id = id,
                OrganisationName = "Organisation Name",
                ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
            });

            var fundingController = GetFundingController();

            // Act
            var result = await fundingController.GetProviderFunding(id);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion


        #region SearchFundings Tests

        /// <summary>
        /// Searches the fundings passes through parameters.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_PassesThroughParameters()
        {
            // Arrange
            var inputs = new
            {
                FundingStreamCodes = new string[] { "DSG" },
                PeriodCodes = new string[] { "AY-1920" },
                SearchTerm = "St. Mary's",
                BeforeDateTime = new DateTime(2002, 6, 3, 4, 25, 57),
                GroupingType = "Provider"
            };

            var requestObj = new List<FundingApiSearchFundingStreamParameters>
            {
                new FundingApiSearchFundingStreamParameters
                {
                    FundingStreamCode = inputs.FundingStreamCodes.First(),
                    PeriodCodes = inputs.PeriodCodes,
                    BeforeDateTime = inputs.BeforeDateTime,
                    GroupingType = inputs.GroupingType,
                    Filters = null
                }
            };

            _mockFundingSearchService
                .Setup(s => s.SearchFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(It.IsAny<ISearchResult<IFundingSearchDocument>>())
                .Verifiable();

            var fundingController = GetFundingController();

            // Act
            var result = await fundingController.SearchFunding(
                new FundingApiSearchRequest
                {
                    FundingStreams = requestObj.ToArray(),
                    SearchTerm = inputs.SearchTerm,
                    WaitForIndexBuild = false
                });

            // Assert
            _mockFundingSearchService.Verify();
        }

        /// <summary>
        /// Searches the funding filters used.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFunding_FiltersUsed()
        {
            // Arrange
            var inputs = new
            {
                FundingStreamCodes = new string[] { "DSG" },
                PeriodCodes = new string[] { "AY-1920" },
                SearchTerm = "St. Mary's",
                BeforeDateTime = new DateTime(2002, 6, 3, 4, 25, 57),
                GroupingType = "Provider"
            };

            var filters = new FundingApiSearchFilterParameters[]
            {
                new FundingApiSearchFilterParameters
                {
                    PropertyName = FundingApiSearchFilterParameters.FilterPropertyName.ParentPrimaryIdentifier,
                    PropertyValue = "B"
                }
            };

            var requestObj = new List<FundingApiSearchFundingStreamParameters>
            {
                new FundingApiSearchFundingStreamParameters
                {
                    FundingStreamCode = inputs.FundingStreamCodes.First(),
                    PeriodCodes = inputs.PeriodCodes,
                    BeforeDateTime = inputs.BeforeDateTime,
                    GroupingType = inputs.GroupingType,
                    Filters = It.Is<FundingApiSearchFilterParameters[]>(x =>
                        x.Length == 1
                        && x[0].PropertyName == filters[0].PropertyName
                        && x[0].PropertyValue == filters[0].PropertyValue)
                }
            };

            _mockFundingSearchService
                .Setup(s => s.SearchFunding(It.IsAny<FundingStreamParameters[]>(), inputs.SearchTerm, false))
                .ReturnsAsync(It.IsAny<ISearchResult<IFundingSearchDocument>>())
                .Verifiable();

            var fundingController = GetFundingController();

            // Act
            var result = await fundingController.SearchFunding(new FundingApiSearchRequest
            {
                SearchTerm = inputs.SearchTerm,
                WaitForIndexBuild = false,
                FundingStreams = requestObj.ToArray()
            });

            // Assert
            _mockFundingSearchService.Verify();
        }

        /// <summary>
        /// Searches the fundings handles empty result.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_HandlesEmptyResult()
        {
            // Arrange
            _mockFundingSearchService
                .Setup(s => s.SearchFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), false))
                .ReturnsAsync((ISearchResult<IFundingSearchDocument>)null);

            var fundingController = GetFundingController();

            // Act
            var result = await fundingController.SearchFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            Assert.AreEqual(result.Funding.Count(), 0);
        }

        /// <summary>
        /// Searches the fundings handles null documents.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_HandlesNullDocuments()
        {
            // Arrange
            _mockFundingSearchService
                .Setup(s => s.SearchFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), false))
                .ReturnsAsync(new AzureSearchResult<IFundingSearchDocument>());

            var fundingController = GetFundingController();

            // Act
            var result = await fundingController.SearchFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Funding.Count());
        }

        /// <summary>
        /// Searches the fundings one provider with one version.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_OneProviderWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureFundingSearchDocument>
            {
                new AzureFundingSearchDocument
                {
                    Id = "TEST1",
                    FundingValue = "TEST",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            var expectedResult = new FundingApiSearchFundingResponse
            {
                Funding = new List<FundingApiSearchFunding>
                {
                    new FundingApiSearchFunding
                    {
                        Id = "TEST1",
                        FundingValue = "TEST",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockFundingSearchService.Setup(o => o.GetProviderFunding("TEST1", false)).ReturnsAsync(new AzureSearchResult<IProviderFundingSearchDocument>());

            _mockFundingSearchService
                .Setup(s => s.SearchFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), false))
                .ReturnsAsync(new AzureSearchResult<IFundingSearchDocument>
                {
                    Documents = documents
                });

            var fundingController = GetFundingController();

            // Act
            var actualResult = await fundingController.SearchFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the fundings one provider with multiple versions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_OneProviderWithMultipleVersions()
        {
            var periodCode = "AY-1920";
            var fundingStreamCode = "DSG";

            // Arrange
            var documents = new List<AzureFundingSearchDocument>
            {
                new AzureFundingSearchDocument
                {
                    Id = "v1",
                    FundingVersion = "1.0",
                    GroupUkprn = "12345678",
                    FundingPeriodCode = periodCode,
                    FundingStreamCode = fundingStreamCode,
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    Id = "v2",
                    FundingVersion = "2.0",
                    GroupUkprn = "12345678",
                    FundingPeriodCode = periodCode,
                    FundingStreamCode = fundingStreamCode,
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    Id = "v3",
                    FundingVersion = "3.0",
                    GroupUkprn = "12345678",
                    FundingPeriodCode = periodCode,
                    FundingStreamCode = fundingStreamCode,
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            var expectedResult = new FundingApiSearchFundingResponse
            {
                Funding = new List<FundingApiSearchFunding>
                {
                    new FundingApiSearchFunding
                    {
                        Id = "v3",
                        FundingVersion = "3.0",
                        GroupUkprn = "12345678",
                        FundingPeriodCode = periodCode,
                        FundingStreamCode = fundingStreamCode,
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockFundingSearchService.Setup(o => o.GetProviderFunding("v3", false)).ReturnsAsync(new AzureSearchResult<IProviderFundingSearchDocument>());

            _mockFundingSearchService
                .Setup(s => s.SearchFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), false))
                .ReturnsAsync(new AzureSearchResult<IFundingSearchDocument>
                {
                    Documents = documents
                });

            var fundingController = GetFundingController();

            // Act
            var actualResult = await fundingController.SearchFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the fundings multiple providers each with one version.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_MultipleProvidersEachWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureFundingSearchDocument>
            {
                new AzureFundingSearchDocument
                {
                    FundingVersion = "1.0",
                    GroupUkprn = "1",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    FundingVersion = "1.0",
                    GroupUkprn = "2",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    FundingVersion = "1.0",
                    GroupUkprn = "3",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            var expectedResult = new FundingApiSearchFundingResponse
            {
                Funding = new List<FundingApiSearchFunding>
                {
                    new FundingApiSearchFunding
                    {
                        FundingVersion = "1.0",
                        GroupUkprn = "1",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchFunding
                    {
                        FundingVersion = "1.0",
                        GroupUkprn = "2",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchFunding
                    {
                        FundingVersion = "1.0",
                        GroupUkprn = "3",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockFundingSearchService
                .Setup(s => s.SearchFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), false))
                .ReturnsAsync(new AzureSearchResult<IFundingSearchDocument>
                {
                    Documents = documents
                });

            var fundingController = GetFundingController();

            // Act
            var actualResult = await fundingController.SearchFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the fundings multiple providers each with multiple versions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_MultipleProvidersEachWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureFundingSearchDocument>
            {
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "10000001",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "10000002",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "10000003",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "10000001",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "10000002",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "10000003",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureFundingSearchDocument
                {
                    GroupUkprn = "10000003",
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
                        GroupUkprn = "10000001",
                        FundingVersion = "2.0",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchFunding
                    {
                        GroupUkprn = "10000002",
                        FundingVersion = "2.0",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchFunding
                    {
                        GroupUkprn = "10000003",
                        FundingVersion = "3.0",
                        ProviderFundings = Enumerable.Empty<string>(),
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockFundingSearchService
                .Setup(s => s.SearchFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), false))
                .ReturnsAsync(new AzureSearchResult<IFundingSearchDocument>
                {
                    Documents = documents
                });

            var fundingController = GetFundingController();

            // Act
            var actualResult = await fundingController.SearchFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion


        #region SearchFundingProviders Tests

        /// <summary>
        /// Searches the funding providers passes through parameters.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundingProviders_PassesThroughParameters()
        {
            // Arrange
            var inputs = new
            {
                FundingStreamCodes = new string[] { "DSG" },
                PeriodCodes = new string[] { "AY-1920" },
                SearchTerm = "St. Mary's",
                BeforeDateTime = new DateTime(2002, 6, 3, 4, 25, 57),
            };

            _mockFundingSearchService
                .Setup(s => s.SearchProviderFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(It.IsAny<ISearchResult<IProviderFundingSearchDocument>>())
                .Verifiable();

            var fundingController = GetFundingController();

            // Act
            var result = await fundingController.SearchProviderFunding(new FundingApiSearchRequest
            {
                FundingStreams = new FundingApiSearchFundingStreamParameters[1]
                {
                    new FundingApiSearchFundingStreamParameters
                    {
                        BeforeDateTime = inputs.BeforeDateTime,
                        FundingStreamCode = inputs.FundingStreamCodes.First(),
                        PeriodCodes = inputs.PeriodCodes
                    }
                },
                SearchTerm = inputs.SearchTerm,
                WaitForIndexBuild = false
            });

            // Assert
            _mockFundingSearchService.Verify();
        }

        /// <summary>
        /// Searches the funding providers handles null result.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundingProviders_HandlesNullResult()
        {
            // Arrange
            _mockFundingSearchService
                .Setup(s => s.SearchProviderFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((ISearchResult<IProviderFundingSearchDocument>)null);

            var fundingController = GetFundingController();

            // Act
            var result = await fundingController.SearchProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Searches the funding providers handles null documents.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundingProviders_HandlesNullDocuments()
        {
            // Arrange
            _mockFundingSearchService
                .Setup(s => s.SearchProviderFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new AzureSearchResult<IProviderFundingSearchDocument>());

            var fundingController = GetFundingController();

            // Act
            var result = await fundingController.SearchProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ProviderFunding);
        }

        /// <summary>
        /// Searches the funding providers one provider with one version.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundingProviders_OneProviderWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>
            {
                new AzureProviderFundingSearchDocument
                {
                   OrganisationUkprn = "1",
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
                        OrganisationUkprn = "1",
                        FundingVersion = "1.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockFundingSearchService
                .Setup(s => s.SearchProviderFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new AzureSearchResult<IProviderFundingSearchDocument>
                {
                    Documents = documents
                });

            var fundingController = GetFundingController();

            // Act
            var actualResult = await fundingController.SearchProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the funding providers one provider with multiple versions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundingProviders_OneProviderWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>
            {
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "1",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "1",
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
                        OrganisationUkprn = "1",
                        FundingVersion = "2.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockFundingSearchService
                .Setup(s => s.SearchProviderFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new AzureSearchResult<IProviderFundingSearchDocument>
                {
                    Documents = documents
                });

            var fundingController = GetFundingController();

            // Act
            var actualResult = await fundingController.SearchProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the funding providers multiple providers each with one version.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundingProviders_MultipleProvidersEachWithOneVersion()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>
            {
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "1",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "2",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                }
            };

            var expectedResult = new FundingApiSearchProviderFundingResponse
            {
                ProviderFunding = new List<FundingApiSearchProviderFunding>
                {
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "1",
                        FundingVersion = "1.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "2",
                        FundingVersion = "2.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockFundingSearchService
                .Setup(s => s.SearchProviderFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new AzureSearchResult<IProviderFundingSearchDocument>
                {
                    Documents = documents
                });

            var fundingController = GetFundingController();

            // Act
            var actualResult = await fundingController.SearchProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the funding providers multiple providers each with multiple versions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundingProviders_MultipleProvidersEachWithMultipleVersions()
        {
            // Arrange
            var documents = new List<AzureProviderFundingSearchDocument>
            {
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "1",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "2",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "3",
                    FundingVersion = "1.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "1",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "2",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "3",
                    FundingVersion = "2.0",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
                new AzureProviderFundingSearchDocument
                {
                    OrganisationUkprn = "3",
                    FundingVersion = "3.0",
                    OpenReason = "Fresh Start",
                    ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                },
            };

            var expectedResult = new FundingApiSearchProviderFundingResponse
            {
                ProviderFunding = new List<FundingApiSearchProviderFunding>
                {
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "1",
                        FundingVersion = "2.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "2",
                        FundingVersion = "2.0",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    },
                    new FundingApiSearchProviderFunding
                    {
                        OrganisationUkprn = "3",
                        FundingVersion = "3.0",
                        OpenReason = "Fresh Start",
                        ChannelVersions = Enumerable.Empty<ChannelVersionModel>()
                    }
                }
            };

            _mockFundingSearchService
                .Setup(s => s.SearchProviderFunding(It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new AzureSearchResult<IProviderFundingSearchDocument>
                {
                    Documents = documents
                });

            var fundingController = GetFundingController();

            // Act
            var actualResult = await fundingController.SearchProviderFunding(It.IsAny<FundingApiSearchRequest>());

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Gets the raw search result.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The IFundingSearchDocument.</returns>
        private static ISearchResult<IFundingSearchDocument> GetRawSearchResult(string id)
        {
            return new AzureSearchResult<IFundingSearchDocument>
            {
                Documents = new List<IFundingSearchDocument>
                {
                    new AzureFundingSearchDocument
                    {
                        Id = id
                    }
                }
            };
        }

        /// <summary>
        /// Gets the funding controller.
        /// </summary>
        /// <returns>The FundingController.</returns>
        private FundingController GetFundingController()
        {
            SetupMockServices();
            return new FundingController(_mockFundingSearchService.Object, _mockLoggerService.Object, _mapper);
        }

        /// <summary>
        /// Setups the mock services.
        /// </summary>
        private void SetupMockServices()
        {
            _mockConfigurationService.Setup(x => x.Value).Returns(new ApplicationConfiguration());
        }

        #endregion
    }
}