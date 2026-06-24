using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.ViewYourFunding.Data.Core;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using PDS.ViewYourFunding.Data.Services;
using PDS.ViewYourFunding.Data.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Tests.Unit.Services
{
    /// <summary>
    /// The AzureFunding Search service tests.
    /// </summary>
    [TestClass]
    public class AzureFundingsSearchServiceTests
    {
        #region Test Constants

        /// <summary>
        /// The azure search maximum results.
        /// </summary>
        private const int AzureSearchMaxResults = 1000;

        /// <summary>
        /// The top.
        /// </summary>
        private const int Top = 1000;

        /// <summary>
        /// The provider funding rebuild URL.
        /// </summary>
        private const string ProviderFundingRebuildUrl = "http://PROVIDER-FUNDING.REBUILD/";

        /// <summary>
        /// The funding rebuild URL.
        /// </summary>
        private const string FundingRebuildUrl = "http://FUNDING.REBUILD/";

        /// <summary>
        /// The default mock document configuration.
        /// </summary>
        private static readonly MockDocumentConfiguration[] DefaultMockDocumentConfiguration = new[] { new MockDocumentConfiguration(0, 1) };

        /// <summary>
        /// The mock configuration service.
        /// </summary>
        private readonly ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration();

        /// <summary>
        /// The mock search service manager.
        /// </summary>
        private readonly Mock<IAzureSearchServiceManager> mockSearchServiceManager = new Mock<IAzureSearchServiceManager>();

        /// <summary>
        /// Mocks the background task queue.
        /// </summary>
        /// <returns>the IBackgroundTaskQueue.</returns>
        public IBackgroundTaskQueue MockBackgroundTaskQueue()
        {
            var mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();

            // For the mock implementation, we want the queued function to be invoked immediately and awaited so that we are simulating the case when the search index is setup.
            mockBackgroundTaskQueue
                .Setup(q => q.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()))
                .Callback(async (Func<CancellationToken, Task> function) => await function(CancellationToken.None));

            return mockBackgroundTaskQueue.Object;
        }

        #endregion


        #region GetFunding Tests

        /// <summary>
        /// Gets the funding uses fundings index client.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task GetFunding_UsesFundingsIndexClient()
        {
            // Arrange
            var mockProviderFundingIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var clientManager = GetMockClientManager(mockProviderFundingIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.GetFunding("search", false);

            // Assert
            clientManager.Verify(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()), Times.AtLeastOnce);
            clientManager.Verify(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()), Times.Never);
        }

        #endregion


        #region GetProviderFundingForId Tests

        /// <summary>
        /// Gets the provider funding uses provider fundings index client.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task GetProviderFundingForId_UsesFundingsIndexClient()
        {
            // Arrange
            var mockProviderFundingIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var clientManager = GetMockClientManager(mockProviderFundingIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.GetProviderFundingForId("id", false);

            // Assert
            clientManager.Verify(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()), Times.AtLeastOnce);
            clientManager.Verify(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()), Times.Never);
        }

        #endregion


        #region GetProviderFunding Tests

        /// <summary>
        /// Gets the provider funding uses fundings index client.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task GetProviderFunding_UsesFundingsIndexClient()
        {
            // Arrange
            var mockProviderFundingIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var clientManager = GetMockClientManager(mockProviderFundingIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.GetProviderFunding("search", false);

            // Assert
            clientManager.Verify(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()), Times.AtLeastOnce);
            clientManager.Verify(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()), Times.Never);
        }

        #endregion


        #region SearchFundings Tests

        /// <summary>
        /// Searches the fundings uses fundings index client.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_UsesFundingsIndexClient()
        {
            // Arrange
            var mockProviderFundingIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var clientManager = GetMockClientManager(mockProviderFundingIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.SearchFunding(It.IsAny<FundingStreamParameters[]>(), "search", false);

            // Assert
            clientManager.Verify(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()), Times.AtLeastOnce);
            clientManager.Verify(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()), Times.Never);
        }

        /// <summary>
        /// Searches the funding check parameters.
        /// </summary>
        /// <param name="fundingsFirstSearchSize">Size of the fundings first search.</param>
        /// <param name="periodCode">The period code.</param>
        /// <param name="expectedSearchSize">Expected size of the search.</param>
        /// <param name="expectedFilter">The expected filter.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        [DataRow(null, "AY1819", 1000, "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY1819'))")]
        [DataRow(null, null, 1000, "(StatusChangedDate le {0})")]
        [DataRow(-1, "AY1819", 1000, "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY1819'))")]
        [DataRow(-1, null, 1000, "(StatusChangedDate le {0})")]
        [DataRow(0, "AY1819", 0, "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY1819'))")]
        [DataRow(0, null, 0, "(StatusChangedDate le {0})")]
        [DataRow(1, "AY0102", 1, "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY0102'))")]
        [DataRow(1, null, 1, "(StatusChangedDate le {0})")]
        [DataRow(10, "AY1112", 10, "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY1112'))")]
        [DataRow(10, null, 10, "(StatusChangedDate le {0})")]
        [DataRow(999, "AY1819", 999, "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY1819'))")]
        [DataRow(999, null, 999, "(StatusChangedDate le {0})")]
        [DataRow(1000, "AY1819", 1000, "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY1819'))")]
        [DataRow(1000, null, 1000, "(StatusChangedDate le {0})")]
        [DataRow(1001, "AY1819", 1000, "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY1819'))")]
        [DataRow(1001, null, 1000, "(StatusChangedDate le {0})")]
        [DataRow(9999, "AY1819", 1000, "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY1819'))")]
        [DataRow(9999, null, 1000, "(StatusChangedDate le {0})")]
        public async Task SearchFunding_CheckParameters(
            int? fundingsFirstSearchSize,
            string periodCode,
            int expectedSearchSize,
            string expectedFilter)
        {
            // Arrange
            var mockProviderFundingIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(new[]
            {
                new MockDocumentConfiguration(0, expectedSearchSize >= 1 ? 1 : 0),
                new MockDocumentConfiguration(expectedSearchSize, 0)
            });

            var clientManager = GetMockClientManager(mockProviderFundingIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());
            var beforeDateTime = new DateTime(2002, 6, 24, 3, 37, 49);
            var beforeDateTimeOffset = new DateTimeOffset(beforeDateTime.Date.AddDays(1).AddSeconds(-1)).ToUniversalTime().ToString("O");
            var periods = periodCode == null ? null : new string[] { periodCode };

            var requestObj = new List<FundingStreamParameters>
            {
                new FundingStreamParameters
                {
                    PeriodCodes = periods,
                    BeforeDateTime = beforeDateTime
                }
            };

            // Act
            var result = await searchService.SearchFunding(requestObj.ToArray(), "search", false);

            // Assert
            mockFundingsIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                It.IsAny<string>(),
                It.Is<SearchOptions>(p =>
                    p.Size == Top
                    && p.QueryType == SearchQueryType.Full
                    && p.Filter == string.Format(expectedFilter, beforeDateTimeOffset))),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// Searches the fundings check search text.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="expectedSearchText">The expected search text.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        [DataRow(null, "/.*/")]
        [DataRow("", "/.*/")]
        [DataRow(" ", "/.*/")]
        [DataRow("_", "/.*/")]
        [DataRow("-", "/.*/")]
        [DataRow("-_\u2013 ~#@'!£$%^&*()[]{}/\\`¬¦|?><,.:; ", "/.*/")]
        [DataRow("search", "/.*search.*/")]
        [DataRow("$earch", "/.*earch.*/")]
        [DataRow("test-search", "/.*test_search.*/")]
        [DataRow("test $-search_", "/.*test_search.*/")]
        [DataRow(" Test $-search _", "/.*Test_search.*/")]
        public async Task SearchFundings_CheckSearchText(string searchText, string expectedSearchText)
        {
            // Arrange
            var mockFundingsProviderIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var mockFundingIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var clientManager = GetMockClientManager(mockFundingsProviderIndexClient, mockFundingIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.SearchFunding(It.IsAny<FundingStreamParameters[]>(), searchText, false);

            // Assert
            mockFundingIndexClient.Verify(
                c => c.SearchDocumentsAsync(expectedSearchText, It.IsAny<SearchOptions>()),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// Searches the fundings only gets first page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_OnlyGetsFirstPage()
        {
            // Arrange
            var mockFundingsProviderIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 9) });
            var clientManager = GetMockClientManager(mockFundingsProviderIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.SearchFunding(It.IsAny<FundingStreamParameters[]>(), "search", false);

            // Assert
            mockFundingsIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(p =>
                        p.Size == Top
                        && (!p.Skip.HasValue || p.Skip.Value == 0))),
                Times.Once);

            mockFundingsIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(p => p.Skip > 0)),
                Times.Never);

            Assert.AreEqual(9, result?.Documents?.Count());
        }

        /// <summary>
        /// Searches the fundings get all pages.
        /// </summary>
        /// <returns>A<see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_GetAllPages()
        {
            var totalDocs = 3000;

            // Arrange
            var mockProviderFundingIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var mockFundingIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(new[]
            {
                new MockDocumentConfiguration(0, AzureSearchMaxResults, totalDocs),
                new MockDocumentConfiguration(AzureSearchMaxResults, AzureSearchMaxResults, totalDocs),
                new MockDocumentConfiguration(AzureSearchMaxResults * 2, AzureSearchMaxResults - 1, totalDocs)
            });

            var clientManager = GetMockClientManager(mockProviderFundingIndexClient, mockFundingIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.SearchFunding(It.IsAny<FundingStreamParameters[]>(), "search", false);

            // Assert
            mockFundingIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(p =>
                        p.Size == AzureSearchMaxResults)),
                Times.Exactly(3));

            Assert.AreEqual((3 * AzureSearchMaxResults) - 1, result.Documents.Count());
        }

        /// <summary>
        /// Searches the fundings get all pages when only one la group.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_GetAllPagesWhenOnlyOneLaGroup()
        {
            var docCount = 1000;
            var totalDocs = 3000;

            // Arrange
            var mockProviderFundingIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var mockFundingIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(new[]
            {
                new MockDocumentConfiguration(0, docCount, totalDocs),
                new MockDocumentConfiguration(docCount, AzureSearchMaxResults, totalDocs),
                new MockDocumentConfiguration(docCount + AzureSearchMaxResults, AzureSearchMaxResults - 1, totalDocs),
            });

            var clientManager = GetMockClientManager(mockProviderFundingIndexClient, mockFundingIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.SearchFunding(It.IsAny<FundingStreamParameters[]>(), "search", false);

            // Assert
            mockFundingIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(p =>
                        p.Size == Top)),
                Times.AtLeast(2));
        }

        /// <summary>
        /// Searches the fundings ukprn contains37.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchFundings_UkprnContains37()
        {
            // Arrange
            var mockProviderFundingIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(new[]
            {
                new MockDocumentConfiguration(0, AzureSearchMaxResults),
                new MockDocumentConfiguration(AzureSearchMaxResults, AzureSearchMaxResults),
                new MockDocumentConfiguration(AzureSearchMaxResults * 2, AzureSearchMaxResults - 1),
            });
            var clientManager = GetMockClientManager(mockProviderFundingIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.SearchFunding(It.IsAny<FundingStreamParameters[]>(), "search", false);

            // Assert
            mockFundingsIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(p =>
                        p.Size == AzureSearchMaxResults)),
                Times.AtLeastOnce);

            Assert.IsTrue(result.Documents.Any(d => d.GroupUkprn.Contains("37")));
        }

        #endregion


        #region SearchFundingProviders Tests

        /// <summary>
        /// Searches the provider funding uses provider index client.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchProviderFunding_UsesProviderIndexClient()
        {
            // Arrange
            var mockProviderFundingIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var mockFundingIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var clientManager = GetMockClientManager(mockProviderFundingIndexClient, mockFundingIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.SearchProviderFunding(new FundingStreamParameters[0], "search", false);

            // Assert
            clientManager.Verify(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()), Times.Never);
            clientManager.Verify(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Searches the provider funding check parameters.
        /// </summary>
        /// <param name="periodCode">The period code.</param>
        /// <param name="expectedFilter">The expected filter.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        [DataRow("AY-1819", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY-1819'))")]
        [DataRow(null, "(StatusChangedDate le {0})")]
        [DataRow("ABC", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'ABC'))")]
        [DataRow("FY-1819", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'FY-1819'))")]
        [DataRow("FY-0102", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'FY-0102'))")]
        [DataRow("AY-1819", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY-1819'))")]
        [DataRow("AY-1920", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY-1920'))")]
        public async Task SearchProviderFunding_CheckParameters(string periodCode, string expectedFilter)
        {
            // Arrange
            var mockFundingsProviderIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(DefaultMockDocumentConfiguration);

            var clientManager = GetMockClientManager(mockFundingsProviderIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());
            var beforeDateTime = new DateTime(2002, 6, 24, 3, 37, 49);
            var beforeDateTimeOffset = new DateTimeOffset(beforeDateTime.Date.AddDays(1).AddSeconds(-1)).ToUniversalTime().ToString("O");
            var periodCodes = periodCode == null ? null : new string[] { periodCode };

            var requestObj = new List<FundingStreamParameters>
            {
                new FundingStreamParameters
                {
                    PeriodCodes = periodCodes,
                    BeforeDateTime = beforeDateTime
                }
            };

            // Act
            var result = await searchService.SearchProviderFunding(requestObj.ToArray(), "search", false);

            // Assert
            mockFundingsProviderIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(p =>
                        (!p.Skip.HasValue || p.Skip.Value == 0)
                        && p.QueryType == SearchQueryType.Full
                        && p.Filter == string.Format(expectedFilter, beforeDateTimeOffset))),
                Times.Once);
        }

        /// <summary>
        /// Searches the provider funding check search text.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="expectedSearchText">The expected search text.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        [DataRow(null, "/.*/")]
        [DataRow("", "/.*/")]
        [DataRow(" ", "/.*/")]
        [DataRow("_", "/.*/")]
        [DataRow("-", "/.*/")]
        [DataRow("-_\u2013 ~#@'!£$%^&*()[]{}/\\`¬¦|?><,.:; ", "/.*/")]
        [DataRow("search", "/.*search.*/")]
        [DataRow("$earch", "/.*earch.*/")]
        [DataRow("test-search", "/.*test_search.*/")]
        [DataRow("test $-search_", "/.*test_search.*/")]
        [DataRow(" Test $-search _", "/.*Test_search.*/")]
        public async Task SearchProviderFunding_CheckSearchText(string searchText, string expectedSearchText)
        {
            // Arrange
            var mockFundingsProviderIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var clientManager = GetMockClientManager(mockFundingsProviderIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            var requestObj = new List<FundingStreamParameters>();

            // Act
            var result = await searchService.SearchProviderFunding(requestObj.ToArray(), searchText, false);

            // Assert
            mockFundingsProviderIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    expectedSearchText,
                    It.IsAny<SearchOptions>()),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// Searches the provider funding gets all pages.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchProviderFunding_GetsAllPages()
        {
            // Arrange
            var mockFundingsProviderIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 2000) });
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(DefaultMockDocumentConfiguration);

            var clientManager = GetMockClientManager(mockFundingsProviderIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.SearchProviderFunding(
                It.IsAny<FundingStreamParameters[]>(), It.IsAny<string>(), false);

            // Assert
            mockFundingsProviderIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(p =>
                        p.Size == AzureSearchMaxResults)),
                Times.Exactly(2));
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow("GAG", false)]
        [DataRow("DSG", false)]
        [DataRow("PSG", false)]
        public async Task SearchFunding_ForRestrictedStreams_HasCorrectFilters(
            string fundingStreamCode,
            bool shouldHaveFilter)
        {
            // Arrange
            var mockProviderFundingIndexClient =
                GetMockSearchClient<AzureProviderFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var mockFundingsIndexClient =
                GetMockSearchClient<AzureFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var appConfig = new ApplicationConfiguration
            {
                RestrictedFundingStreamCodes = "GAG,1619",
                RestrictedVariationReasons = "FundingUpdated",
                FilterOnFundingVersion = true
            };


            var clientManager = GetMockClientManager(mockProviderFundingIndexClient, mockFundingsIndexClient);

            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                appConfig,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            var beforeDateTime = new DateTime(2002, 6, 24, 3, 37, 49);

            var requestObj = new List<FundingStreamParameters>
            {
                new FundingStreamParameters
                {
                    BeforeDateTime = beforeDateTime,
                    FundingStreamCode = fundingStreamCode
                }
            };

            var restrictedStreamFilterString =
                "and (fundingVersion eq '1_0' or variationReasons/any(r: r eq 'FundingUpdated'))";

            // Act
            await searchService.SearchFunding(requestObj.ToArray(), "search", false);

            // Assert
            mockFundingsIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(p =>
                        p.Size == Top
                        && p.QueryType == SearchQueryType.Full
                        && p.Filter.Contains(restrictedStreamFilterString) == shouldHaveFilter)),
                Times.AtLeastOnce);
        }

        #endregion

        #region SearchLatestFundingProviders Tests

        [TestMethod, TestCategory("Unit")]
        [DataRow("GAG", "AC-2425", true)]
        [DataRow("DSG", "FY-2324", false)]
        [DataRow("PSG", "AY-2324", false)]
        public async Task SearchLatestProviderFunding_ForRestrictedStreams_HasCorrectFilters(
            string fundingStreamCode,
            string periodCode,
            bool shouldHaveFilter)
        {
            // Arrange
            var mockFundingsProviderIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(DefaultMockDocumentConfiguration);

            var appConfig = new ApplicationConfiguration
            {
                RestrictedFundingStreamCodes = "GAG",
                RestrictedVariationReasons = "FundingUpdated",
                FilterOnFundingVersion = true
            };


            var clientManager = GetMockClientManager(mockFundingsProviderIndexClient, mockFundingsIndexClient);

            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                appConfig,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            var beforeDateTime = new DateTime(2023, 6, 24, 3, 37, 49);

            var periodCodes = periodCode == null ? null : new string[] { periodCode };

            var requestObj = new List<FundingStreamParameters>
            {
                new FundingStreamParameters
                {
                    PeriodCodes = periodCodes,
                    BeforeDateTime = beforeDateTime,
                    FundingStreamCode = fundingStreamCode
                }
            };

            var restrictedStreamFilterString =
                "and (FundingVersion eq '1_0' or VariationReasons/any(r: r eq 'FundingUpdated'))";

            // Act
            await searchService.SearchLatestProviderFunding(requestObj.ToArray(), "search", false);

            // Assert
            mockFundingsProviderIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(p =>
                        p.Size == Top
                        && p.QueryType == SearchQueryType.Full
                        && p.Filter.Contains(restrictedStreamFilterString) == shouldHaveFilter)),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// Searches the provider funding uses provider index client.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task SearchLatestProviderFunding_UsesProviderIndexClient()
        {
            // Arrange
            var mockProviderFundingIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var mockFundingIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var clientManager = GetMockClientManager(mockProviderFundingIndexClient, mockFundingIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            // Act
            var result = await searchService.SearchLatestProviderFunding(new FundingStreamParameters[0], "search", false);

            // Assert
            clientManager.Verify(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>()), Times.Never);
            clientManager.Verify(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Searches the provider funding check parameters.
        /// </summary>
        /// <param name="periodCode">The period code.</param>
        /// <param name="expectedFilter">The expected filter.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        [DataRow("AY-1819", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY-1819'))")]
        [DataRow(null, "(StatusChangedDate le {0})")]
        [DataRow("ABC", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'ABC'))")]
        [DataRow("FY-1819", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'FY-1819'))")]
        [DataRow("FY-0102", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'FY-0102'))")]
        [DataRow("AY-1819", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY-1819'))")]
        [DataRow("AY-1920", "(StatusChangedDate le {0} and (FundingPeriodCode eq 'AY-1920'))")]
        public async Task SearchLatestProviderFunding_CheckParameters(string periodCode, string expectedFilter)
        {
            // Arrange
            var mockFundingsProviderIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(DefaultMockDocumentConfiguration);

            var clientManager = GetMockClientManager(mockFundingsProviderIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());
            var beforeDateTime = new DateTime(2002, 6, 24, 3, 37, 49);
            var beforeDateTimeOffset = new DateTimeOffset(beforeDateTime.Date.AddDays(1).AddSeconds(-1)).ToUniversalTime().ToString("O");
            var periodCodes = periodCode == null ? null : new string[] { periodCode };

            var requestObj = new List<FundingStreamParameters>
            {
                new FundingStreamParameters
                {
                    PeriodCodes = periodCodes,
                    BeforeDateTime = beforeDateTime
                }
            };

            // Act
            var result = await searchService.SearchLatestProviderFunding(requestObj.ToArray(), "search", false);

            // Assert
            mockFundingsProviderIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    It.IsAny<string>(),
                    It.Is<SearchOptions>(p =>
                        (!p.Skip.HasValue || p.Skip.Value == 0)
                        && p.QueryType == SearchQueryType.Full
                        && p.Filter == string.Format(expectedFilter, beforeDateTimeOffset))),
                Times.Once);
        }

        /// <summary>
        /// Searches the provider funding check search text.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="expectedSearchText">The expected search text.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod, TestCategory("Unit")]
        [DataRow(null, "/.*/")]
        [DataRow("", "/.*/")]
        [DataRow(" ", "/.*/")]
        [DataRow("_", "/.*/")]
        [DataRow("-", "/.*/")]
        [DataRow("-_\u2013 ~#@'!£$%^&*()[]{}/\\`¬¦|?><,.:; ", "/.*/")]
        [DataRow("search", "/.*search.*/")]
        [DataRow("$earch", "/.*earch.*/")]
        [DataRow("test-search", "/.*test_search.*/")]
        [DataRow("test $-search_", "/.*test_search.*/")]
        [DataRow(" Test $-search _", "/.*Test_search.*/")]
        public async Task SearchLatestProviderFunding_CheckSearchText(string searchText, string expectedSearchText)
        {
            // Arrange
            var mockFundingsProviderIndexClient = GetMockSearchClient<AzureProviderFundingSearchDocument>(new[] { new MockDocumentConfiguration(0, 1) });
            var mockFundingsIndexClient = GetMockSearchClient<AzureFundingSearchDocument>(DefaultMockDocumentConfiguration);
            var clientManager = GetMockClientManager(mockFundingsProviderIndexClient, mockFundingsIndexClient);
            var searchService = new AzureFundingSearchService(
                clientManager.Object,
                applicationConfiguration,
                mockSearchServiceManager.Object,
                null,
                MockBackgroundTaskQueue());

            var requestObj = new List<FundingStreamParameters>();

            // Act
            var result = await searchService.SearchLatestProviderFunding(requestObj.ToArray(), searchText, false);

            // Assert
            mockFundingsProviderIndexClient.Verify(
                c => c.SearchDocumentsAsync(
                    expectedSearchText,
                    It.IsAny<SearchOptions>()),
                Times.AtLeastOnce);
        }

        #endregion


        #region Helper Class & Methods

        /// <summary>
        /// The MockDocumentConfiguration.
        /// </summary>
        private class MockDocumentConfiguration
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MockDocumentConfiguration"/> class.
            /// </summary>
            /// <param name="skip">The skip.</param>
            /// <param name="returnedDocumentCount">The returned document count.</param>
            public MockDocumentConfiguration(int skip, int returnedDocumentCount)
            {
                Skip = skip;
                ReturnedDocumentCount = returnedDocumentCount;
                DocumentsTotal = returnedDocumentCount;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MockDocumentConfiguration"/> class.
            /// </summary>
            /// <param name="skip">The skip.</param>
            /// <param name="documentCount">The document count.</param>
            /// <param name="documentsTotal">The documents total.</param>
            public MockDocumentConfiguration(int skip, int documentCount, int documentsTotal)
            {
                Skip = skip;
                ReturnedDocumentCount = documentCount;
                DocumentsTotal = documentsTotal;
            }

            /// <summary>
            /// Gets or sets the skip.
            /// </summary>
            /// <value>
            /// The skip.
            /// </value>
            public int Skip { get; set; }

            /// <summary>
            /// Gets or sets the returned document count.
            /// </summary>
            /// <value>
            /// The returned document count.
            /// </value>
            public int ReturnedDocumentCount { get; set; }

            /// <summary>
            /// Gets or sets the documents total.
            /// </summary>
            /// <value>
            /// The documents total.
            /// </value>
            public int DocumentsTotal { get; set; }
        }

        /// <summary>
        /// Gets the mock client manager.
        /// </summary>
        /// <param name="mockFundingsProviderIndexClient">The mock fundings provider index client.</param>
        /// <param name="mockFundingsIndexClient">The mock fundings index client.</param>
        /// <returns>the IAzureSearchAliasClientManager.</returns>
        private Mock<IAzureSearchAliasClientManager> GetMockClientManager(
            Mock<IAzureSearchAliasClient<AzureProviderFundingSearchDocument>> mockFundingsProviderIndexClient,
            Mock<IAzureSearchAliasClient<AzureFundingSearchDocument>> mockFundingsIndexClient)
        {
            var mockClientManager = new Mock<IAzureSearchAliasClientManager>();

            mockClientManager.Setup(m => m.GetSearchAliasClient<AzureProviderFundingSearchDocument>(It.IsAny<int>())).Returns(mockFundingsProviderIndexClient.Object);
            mockClientManager.Setup(m => m.GetSearchAliasClient<AzureFundingSearchDocument>(It.IsAny<int>())).Returns(mockFundingsIndexClient.Object);

            return mockClientManager;
        }

        /// <summary>
        /// Gets the mock search client.
        /// </summary>
        /// <typeparam name="T">The Document type,.</typeparam>
        /// <param name="resultsConfigs">The results configs.</param>
        /// <returns>the IAzureSearchAliasClient.</returns>
        private Mock<IAzureSearchAliasClient<T>> GetMockSearchClient<T>(MockDocumentConfiguration[] resultsConfigs, int maxVersion = 4)
            where T : class
        {
            var mockClient = new Mock<IAzureSearchAliasClient<T>>();

            foreach (var resultsConfig in resultsConfigs)
            {
                var fundingDocuments = GetMockDocuments<T>(resultsConfig.Skip, resultsConfig.ReturnedDocumentCount);

                var results = fundingDocuments.Select(d => SearchModelFactory.SearchResult<T>(d, 0.0, null)).ToList();

                mockClient
                    .Setup(c => c.SearchDocumentsAsync(
                        It.IsAny<string>(),
                        It.Is<SearchOptions>(p =>
                            p.Size == Top &&
                            (p.Skip == resultsConfig.Skip || (resultsConfig.Skip == 0 && p.Skip == null)) &&
                                p.Filter != $"(statusChangedDate le 0001-01-01T23:59:59.0000000+00:00 and fundingVersion eq '{maxVersion}_0')")))
                    .ReturnsAsync(SearchModelFactory.SearchResults<T>(
                        results,
                        resultsConfig.DocumentsTotal,
                        null,
                        null,
                        null));

                mockClient
                        .Setup(c => c.SearchDocumentsAsync(
                            It.IsAny<string>(),
                            It.Is<SearchOptions>(p =>
                                p.Size == Top &&
                                (p.Skip == resultsConfig.Skip || (resultsConfig.Skip == 0 && p.Skip == null)) &&
                                p.Filter == $"(statusChangedDate le 0001-01-01T23:59:59.0000000+00:00 and fundingVersion eq '{maxVersion}_0')")))
                        .ReturnsAsync(SearchModelFactory.SearchResults<T>(
                            new List<SearchResult<T>>(),
                            0,
                            null,
                            null,
                            null));
            }

            return mockClient;
        }

        /// <summary>
        /// Gets the mock documents.
        /// </summary>
        /// <typeparam name="T">The list item type.</typeparam>
        /// <param name="skip">The skip.</param>
        /// <param name="numberRequired">The number required.</param>
        /// <returns>The list of items.</returns>
        private List<T> GetMockDocuments<T>(int skip, int numberRequired)
            where T : class
        {
            var result = new List<T>(numberRequired);

            if (typeof(T) == typeof(AzureFundingSearchDocument))
            {
                for (var i = 1; i <= numberRequired; i++)
                {
                    T document = new AzureFundingSearchDocument
                    {
                        GroupUkprn = (skip + i).ToString()
                    }

                    as T;

                    result.Add(document);
                }
            }

            if (typeof(T) == typeof(AzureProviderFundingSearchDocument))
            {
                for (var i = 1; i <= numberRequired; i++)
                {
                    T document = new AzureProviderFundingSearchDocument
                    {
                        OrganisationUkprn = (skip + i).ToString()
                    }

                    as T;

                    result.Add(document);
                }
            }

            return result;
        }


        #endregion
    }
}