using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PDS.ViewYourFunding.Data.Core;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Interfaces.DTOs;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using PDS.ViewYourFunding.Data.Services.Enums;
using PDS.ViewYourFunding.Data.Services.Exceptions;
using PDS.ViewYourFunding.Data.Services.Extensions;
using PDS.ViewYourFunding.Data.Services.Helpers;
using PDS.ViewYourFunding.Data.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Services
{
    /// <summary>
    /// An implementation of IFundingSearchService that uses Azure Search.
    /// </summary>
    public class AzureFundingSearchService : IFundingSearchService
    {
        #region Fields & Constants

        /// <summary>
        /// This is a hard limit on the number of results that Azure Search will return per request.
        /// </summary>
        private const int MaximumResultsPerAzureSearch = 1000;
        private const string DefaultSearchPattern = "/.*/";
        private const string RestrictedVersionValue = "1_0";

        // Property names
        private const string FundingVersionPropertyName = "FundingVersion";
        private const string VariationReasonPropertyName = "VariationReasons";

        // Regular Expressions for processing the search text
        private static readonly Regex _spacingCharacters = new Regex(@"[\s\u2212\u2013\u2014\u2010-]+", RegexOptions.Compiled);
        private static readonly Regex _disallowedCharacters = new Regex(@"[^\w]+", RegexOptions.Compiled);
        private static readonly Regex _multipleDashes = new Regex(@"_{2,}", RegexOptions.Compiled);

        private readonly IAzureSearchAliasClientManager _searchAliasClientManager;
        private readonly ApplicationConfiguration _configuration;
        private readonly IAzureSearchServiceManager _searchServiceManager;
        private readonly ILogger<AzureFundingSearchService> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IEnumerable<string> _restrictedFundingStreamCodes;
        private readonly string _restrictedVariationReasons;


        private SetupStatus _fundingIndexSetup = SetupStatus.NotSetup;
        private SetupStatus _providerFundingIndexSetup = SetupStatus.NotSetup;

        private int? _fundingIndexClassHash;
        private int? _providerFundingIndexClassHash;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureFundingSearchService"/> class.
        /// </summary>
        /// <param name="searchAliasClientManager">The Search Index Client manager to use.</param>
        /// <param name="configuration">The configuration service to use to look up key/values.</param>
        /// <param name="searchServiceManager">The search service manager to use.</param>
        /// <param name="logger">Logging service to use.</param>
        /// <param name="taskQueue">Queue for background tasks.</param>
        public AzureFundingSearchService(
            IAzureSearchAliasClientManager searchAliasClientManager,
            ApplicationConfiguration configuration,
            IAzureSearchServiceManager searchServiceManager,
            ILogger<AzureFundingSearchService> logger,
            IBackgroundTaskQueue taskQueue)
        {
            _searchAliasClientManager = searchAliasClientManager;
            _configuration = configuration;
            _searchServiceManager = searchServiceManager;
            _logger = logger;
            _taskQueue = taskQueue;
            _restrictedFundingStreamCodes = _configuration.RestrictedFundingStreamCodes.Split(',');
            _restrictedVariationReasons = _configuration.RestrictedVariationReasons;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Search for provider funding.
        /// </summary>
        /// <param name="fundingStreamParameters">The funding streams to look up.</param>
        /// <param name="searchTerm">The search term to match.</param>
        /// <param name="waitForIndexBuild">Should we wait for the index to (re)build.</param>
        /// <returns>A response object containing the list of matching funding providers.</returns>
        public async Task<ISearchResult<IProviderFundingSearchDocument>> SearchLatestProviderFunding(FundingStreamParameters[] fundingStreamParameters, string searchTerm, bool waitForIndexBuild)
        {
            _logger?.LogInformation($"{nameof(SearchLatestProviderFunding)} for {JsonConvert.SerializeObject(fundingStreamParameters)}");

            await SetupIndexesIfRequired<AzureProviderFundingSearchDocument>(waitForIndexBuild, false);
            var searchQueryClient = _searchAliasClientManager.GetSearchAliasClient<AzureProviderFundingSearchDocument>(GetClassHash(false));

            var cleanSearchTerm = GetCleanSearchTerm(searchTerm);
            try
            {
                var options = GetSearchOptions(fundingStreamParameters, true, byFundingVersion: true);
                var azureSearchResult = await GetFirstResultSet(null, cleanSearchTerm, searchQueryClient, options);
                var maxVersion = azureSearchResult.GetResults()?.FirstOrDefault()?.Document?.FundingVersion;

                _logger?.LogInformation($"Max version for search is {maxVersion}");

                if (maxVersion != null)
                {
                    var maxVersionValue = int.Parse(maxVersion.Substring(0, maxVersion.IndexOf("_")));

                    var groupedDocuments = new Dictionary<(string OrganisationUkprn, string ParentProviderType, string GroupingReason), IProviderFundingSearchDocument>();
                    var ukprns = new List<string>();

                    for (int fundingVersion = maxVersionValue; fundingVersion > 0; fundingVersion--)
                    {
                        azureSearchResult = null;

                        foreach (var fundingStreamParameter in fundingStreamParameters ?? Enumerable.Empty<FundingStreamParameters>())
                        {
                            var filters = fundingStreamParameter.Filters.ToList();
                            var fundingVersionFilter = filters.SingleOrDefault(filter => filter.PropertyName == SearchFilterParameters.FilterPropertyName.FundingVersion);
                            if (fundingVersionFilter != null)
                            {
                                filters.Remove(fundingVersionFilter);
                            }

                            filters.Add(new SearchFilterParameters { PropertyName = SearchFilterParameters.FilterPropertyName.FundingVersion, PropertyValue = $"{fundingVersion}_0" });
                            fundingStreamParameter.Filters = filters.ToArray();
                        }

                        var searchOptions = GetSearchOptions(fundingStreamParameters, true, ukprns);

                        _logger?.LogInformation($"Retrieving data with search options as {JsonConvert.SerializeObject(searchOptions)}");

                        azureSearchResult = await GetFirstResultSet(null, cleanSearchTerm, searchQueryClient, searchOptions);
                        var filteredResults = await GetFilteredDocuments(azureSearchResult, null, cleanSearchTerm, searchQueryClient, searchOptions);

                        _logger?.LogInformation($"Filtered Results documents count : {filteredResults.Documents?.Count()} and RetrievedAllData state is: {filteredResults.RetrievedAllData}");

                        var result = filteredResults.Documents
                            ?.OrderBy(g => g.StatusChangedDate)
                            ?.GroupBy(g => (g.OrganisationUkprn, g.ParentProviderType, g.GroupingReason))
                            ?.ToDictionary(g => g.Key, g => g.First());

                        if (result != null && result.Any())
                        {
                            groupedDocuments = DictionaryExtensions.Merge(
                                new List<Dictionary<(string OrganisationUkprn, string ParentProviderType, string GroupingReason), IProviderFundingSearchDocument>>() { groupedDocuments, result });
                        }

                        ukprns.AddRange(groupedDocuments.Keys.Select(k => k.OrganisationUkprn));

                        if (!filteredResults.RetrievedAllData)
                        {
                            fundingVersion++;

                            // Removing last element as we might not have retrieved all parent information
                            var lastUkprn = ukprns.Last();
                            ukprns.Remove(lastUkprn);
                        }
                    }

                    return new AzureSearchResult<IProviderFundingSearchDocument>
                    {
                        Documents = groupedDocuments.Values
                    };
                }

                return new AzureSearchResult<IProviderFundingSearchDocument>
                {
                    Documents = Enumerable.Empty<IProviderFundingSearchDocument>()
                };
            }
            catch (NoMatchingIndexException nmex)
            {
                _logger?.LogError(nmex, string.Empty);
                ResetIndexStatus(false);

                throw new Exception("Provider funding index not ready", nmex);
            }
            catch (RequestFailedException rfex)
            {
                _logger?.LogError(rfex, string.Empty);

                if (rfex.Status == (int)HttpStatusCode.NotFound)
                {
                    ResetIndexStatus(false);

                    throw new Exception("Provider funding index not ready (rethrow)", rfex);
                }

                throw;
            }
        }

        /// <summary>
        /// Search for provider funding.
        /// </summary>
        /// <param name="fundingStreamParameters">The funding streams to look up.</param>
        /// <param name="searchTerm">The search term to match.</param>
        /// <param name="waitForIndexBuild">Should we wait for the index to (re)build.</param>
        /// <returns>A response object containing the list of matching funding providers.</returns>
        public async Task<ISearchResult<IProviderFundingSearchDocument>> SearchProviderFunding(FundingStreamParameters[] fundingStreamParameters, string searchTerm, bool waitForIndexBuild)
        {
            await SetupIndexesIfRequired<AzureProviderFundingSearchDocument>(waitForIndexBuild, false);
            var searchQueryClient = _searchAliasClientManager.GetSearchAliasClient<AzureProviderFundingSearchDocument>(GetClassHash(false));

            var cleanSearchTerm = GetCleanSearchTerm(searchTerm);
            try
            {
                var azureSearchResult = await GetFirstResultSet(fundingStreamParameters, cleanSearchTerm, searchQueryClient);
                return new AzureSearchResult<IProviderFundingSearchDocument>
                {
                    Documents = (await GetFilteredDocuments(azureSearchResult, fundingStreamParameters, cleanSearchTerm, searchQueryClient)).Documents
                };
            }
            catch (NoMatchingIndexException nmex)
            {
                _logger?.LogError(nmex, string.Empty);
                ResetIndexStatus(false);

                throw new Exception("Provider funding index not ready", nmex);
            }
            catch (RequestFailedException rfex)
            {
                _logger?.LogError(rfex, string.Empty);

                if (rfex.Status == (int)HttpStatusCode.NotFound)
                {
                    ResetIndexStatus(false);

                    throw new Exception("Provider funding index not ready (rethrow)", rfex);
                }

                throw;
            }
        }

        /// <summary>
        /// Search for funding by group.
        /// </summary>
        /// <param name="fundingStreamParameters">The funding streams to look up.</param>
        /// <param name="searchTerm">The search term to match.</param>
        /// <param name="waitForIndexBuild">Should we wait for the index to (re)build.</param>
        /// <returns>A response object containing the list of matching groups. If only one group is found, the response also contains all of the
        /// fundings for provider fundings under that group.</returns>
        public async Task<ISearchResult<IFundingSearchDocument>> SearchFunding(FundingStreamParameters[] fundingStreamParameters, string searchTerm, bool waitForIndexBuild)
        {
            await SetupIndexesIfRequired<AzureFundingSearchDocument>(waitForIndexBuild, true);
            var searchQueryClient = _searchAliasClientManager.GetSearchAliasClient<AzureFundingSearchDocument>(GetClassHash(true));
            var searchOptions = GetSearchOptions(fundingStreamParameters, false);

            var resultsPerPage = searchOptions.Size;
            var cleanSearchTerm = GetCleanSearchTerm(searchTerm);

            try
            {
                var azureSearchResult = await searchQueryClient.SearchDocumentsAsync(cleanSearchTerm, searchOptions);

                var documents = azureSearchResult?.GetResults()?.Select(result => result.Document).ToList();

                var totalCount = azureSearchResult?.TotalCount;
                var batchCount = (int)Math.Ceiling((totalCount ?? 1) / (double)resultsPerPage);

                documents.AddRange((await SearchDocs(batchCount, fundingStreamParameters, resultsPerPage, cleanSearchTerm, searchQueryClient, false)).Documents);

                return new AzureSearchResult<IFundingSearchDocument>
                {
                    Documents = documents
                };
            }
            catch (NoMatchingIndexException nmex)
            {
                _logger?.LogError(nmex, string.Empty);
                ResetIndexStatus(true);

                throw new Exception("Funding index not ready", nmex);
            }
            catch (RequestFailedException rfex)
            {
                _logger?.LogError(rfex, string.Empty);

                if (rfex.Status == (int)HttpStatusCode.NotFound)
                {
                    ResetIndexStatus(true);
                    throw new Exception("Funding index not ready (via RequestFailedException)", rfex);
                }

                throw;
            }
        }

        /// <summary>
        /// Get a single instance of funding by its id.
        /// </summary>
        /// <param name="id">A funding id to lookup.</param>
        /// <param name="waitForIndexBuild">Should we await the index (re)build (if it necessary), or cancel with an error.</param>
        /// <returns>A single instance of funding, with its associated provider funding - or null.</returns>
        public async Task<ISearchResult<IFundingSearchDocument>> GetFunding(string id, bool waitForIndexBuild)
        {
            await SetupIndexesIfRequired<AzureFundingSearchDocument>(waitForIndexBuild, true);
            var searchQueryClient = _searchAliasClientManager.GetSearchAliasClient<AzureFundingSearchDocument>(GetClassHash(true));

            var searchOptions = new SearchOptions
            {
                Filter = $"{nameof(AzureFundingSearchDocument.Id)} eq '{id}'",
                QueryType = SearchQueryType.Full,
                Size = MaximumResultsPerAzureSearch,
                IncludeTotalCount = true
            };

            try
            {
                var azureSearchResult = await searchQueryClient.SearchDocumentsAsync("*", searchOptions);

                var documents = azureSearchResult?.GetResults()?.Select(result => result.Document).ToList();

                return new AzureSearchResult<IFundingSearchDocument>
                {
                    Documents = documents
                };
            }
            catch (NoMatchingIndexException nmex)
            {
                _logger?.LogError(nmex, string.Empty);
                ResetIndexStatus(true);

                throw new Exception("Funding index not ready", nmex);
            }
            catch (RequestFailedException rfex)
            {
                _logger?.LogError(rfex, string.Empty);

                if (rfex.Status == (int)HttpStatusCode.NotFound)
                {
                    ResetIndexStatus(true);

                    throw new Exception("Funding index not ready (via RequestFailedException)", rfex);
                }

                throw;
            }
        }

        /// <summary>
        /// Get a single instance of provider funding by its id.
        /// </summary>
        /// <param name="id">A provider funding id to lookup.</param>
        /// <param name="waitForIndexBuild">Should we await the index (re)build (if it necessary), or cancel with an error.</param>
        /// <returns>A single instance of provider funding.</returns>
        public async Task<IProviderFundingSearchDocument> GetProviderFundingForId(string id, bool waitForIndexBuild)
        {
            await SetupIndexesIfRequired<AzureProviderFundingSearchDocument>(waitForIndexBuild, false);
            var searchQueryClient = _searchAliasClientManager.GetSearchAliasClient<AzureProviderFundingSearchDocument>(GetClassHash(false));

            var searchOptions = new SearchOptions
            {
                Filter = $"{nameof(AzureProviderFundingSearchDocument.Id)} eq '{id}'",
                QueryType = SearchQueryType.Full,
                Size = MaximumResultsPerAzureSearch,
                IncludeTotalCount = true
            };

            try
            {
                var azureSearchResult = await searchQueryClient.SearchDocumentsAsync("*", searchOptions);

                var documents = azureSearchResult?.GetResults()?.Select(result => result.Document);

                return documents.FirstOrDefault();
            }
            catch (NoMatchingIndexException nmex)
            {
                _logger?.LogError(nmex, string.Empty);
                ResetIndexStatus(false);

                throw new Exception("Provider Funding index not ready", nmex);
            }
            catch (RequestFailedException rfex)
            {
                _logger?.LogError(rfex, string.Empty);

                if (rfex.Status == (int)HttpStatusCode.NotFound)
                {
                    ResetIndexStatus(false);

                    throw new Exception("Provider Funding index not ready (via RequestFailedException)", rfex);
                }

                throw;
            }
        }

        /// <summary>
        /// Get the provider funding for a specific funding.
        /// </summary>
        /// <param name="parentId">The if of parent funding id to get provider funding for.</param>
        /// <param name="waitForIndexBuild">Should we await the index (re)build (if it necessary), or cancel with an error.</param>
        /// <returns>Funding providers that make up the funding.</returns>
        public async Task<ISearchResult<IProviderFundingSearchDocument>> GetProviderFunding(string parentId, bool waitForIndexBuild)
        {
            await SetupIndexesIfRequired<AzureProviderFundingSearchDocument>(waitForIndexBuild, false);
            var searchQueryClient = _searchAliasClientManager.GetSearchAliasClient<AzureProviderFundingSearchDocument>(GetClassHash(false));

            var searchOptions = new SearchOptions
            {
                Filter = $"{nameof(AzureProviderFundingSearchDocument.ParentId)} eq '{parentId}'",
                QueryType = SearchQueryType.Full,
                Size = MaximumResultsPerAzureSearch,
                IncludeTotalCount = true
            };

            try
            {
                var azureSearchResult = await searchQueryClient.SearchDocumentsAsync("*", searchOptions);

                var documents = azureSearchResult?.GetResults()?.Select(result => result.Document).ToList();

                // Get the remaining pages of documents if any more exist:
                var currentSkip = 0;

                if (documents != null)
                {
                    while (azureSearchResult?.GetResults()?.Count() >= MaximumResultsPerAzureSearch)
                    {
                        currentSkip += azureSearchResult.GetResults().Count();

                        searchOptions.Skip = currentSkip;

                        azureSearchResult = await searchQueryClient.SearchDocumentsAsync("*", searchOptions);

                        documents.AddRange(azureSearchResult.GetResults().Select(result => result.Document));
                    }
                }

                return new AzureSearchResult<IProviderFundingSearchDocument>
                {
                    Documents = documents
                };
            }
            catch (NoMatchingIndexException nmex)
            {
                _logger?.LogError(nmex, string.Empty);

                if (_providerFundingIndexSetup == SetupStatus.Setup)
                {
                    _providerFundingIndexSetup = SetupStatus.NotSetup;
                }

                throw new Exception("Provider funding index not ready", nmex);
            }
            catch (RequestFailedException rfex)
            {
                _logger?.LogError(rfex, string.Empty);

                if (rfex.Status == (int)HttpStatusCode.NotFound)
                {
                    if (_providerFundingIndexSetup == SetupStatus.Setup)
                    {
                        _providerFundingIndexSetup = SetupStatus.NotSetup;
                    }

                    throw new Exception("Provider funding index not ready (via RequestFailedException)", rfex);
                }

                throw;
            }
        }

        #endregion


        #region Private Methods

        private async Task<(IEnumerable<IProviderFundingSearchDocument> Documents, bool RetrievedAllData)> GetFilteredDocuments(
            SearchResults<AzureProviderFundingSearchDocument> azureSearchResult,
            FundingStreamParameters[] fundingStreamParameters,
            string cleanSearchTerm,
            IAzureSearchAliasClient<AzureProviderFundingSearchDocument> searchQueryClient,
            SearchOptions searchParameters = null)
        {
            var documents = azureSearchResult?.GetResults().Select(result => result.Document).ToList();

            var totalCount = azureSearchResult.TotalCount;
            _logger?.LogInformation($"Azure search Result count is {totalCount}");
            var batchCount = (int)Math.Ceiling((totalCount ?? 1) / (double)MaximumResultsPerAzureSearch);
            _logger?.LogInformation($"Azure search batch count is {batchCount}");
            if (documents != null)
            {
                var result = await SearchDocs(batchCount, fundingStreamParameters, MaximumResultsPerAzureSearch, cleanSearchTerm, searchQueryClient, true, searchParameters);
                documents.AddRange(result.Documents);

                return (documents, result.RetrievedAllData);
            }

            return (Enumerable.Empty<IProviderFundingSearchDocument>(), true);
        }

        private async Task<SearchResults<AzureProviderFundingSearchDocument>> GetFirstResultSet(
            FundingStreamParameters[] fundingStreamParameters,
            string cleanSearchTerm,
            IAzureSearchAliasClient<AzureProviderFundingSearchDocument> searchQueryClient,
            SearchOptions searchOptions = null)
        {
            var options = searchOptions ?? GetSearchOptions(fundingStreamParameters, true);
            return await searchQueryClient.SearchDocumentsAsync(cleanSearchTerm, options);
        }

        /// <summary>
        /// Make the call to the underlying search service to search documents using params and search term.
        /// </summary>
        /// <typeparam name="T">The return type and the underlying document type.</typeparam>
        /// <param name="batchCount">How many batches are they to look through.</param>
        /// <param name="fundingStreamParameters">The funding streams to look up.</param>
        /// <param name="resultsPerPage">How many results are on a single page.</param>
        /// <param name="cleanSearchTerm">The search term to use (cleaned up previously).</param>
        /// <param name="searchQueryClient">The implementation of search to use.</param>
        /// <param name="isProviderFunding">Is Provider funding.</param>
        /// <param name="searchOptions">The search options as SearchOptions.</param>
        /// <returns>A list of matching documents.</returns>
        private async Task<(List<T> Documents, bool RetrievedAllData)> SearchDocs<T>(
            int batchCount,
            FundingStreamParameters[] fundingStreamParameters,
            int? resultsPerPage,
            string cleanSearchTerm,
            IAzureSearchAliasClient<T> searchQueryClient,
            bool isProviderFunding,
            SearchOptions searchOptions = null)
            where T : class
        {
            var tasks = new List<Task<SearchResults<T>>>();

            const int MAX_CONCURRENT_TASKS = 5;
            const int DELAY_MS = 500;
            var retrievedAllData = true;

            _logger?.LogInformation($"{nameof(SearchDocs)} for {JsonConvert.SerializeObject(fundingStreamParameters)}");

            for (var idx = 1; idx < batchCount; idx++)
            {
                var parameters = searchOptions ?? GetSearchOptions(fundingStreamParameters, isProviderFunding);
                var skipValue = idx * resultsPerPage;

                if (skipValue > 100000)
                {
                    retrievedAllData = false;
                    break;
                }

                parameters.Skip = skipValue;

                tasks.Add(searchQueryClient.SearchDocumentsAsync(cleanSearchTerm, parameters));
                _logger?.LogInformation($"Task SearchDocumentsAsync created with skip {skipValue}, loop index: {idx}");

                // PSG has around 36 pages, and without limiting how many we kick off at once, it generally timed out
                if (idx % MAX_CONCURRENT_TASKS == 0)
                {
                    await Task.Delay(DELAY_MS);
                }
            }

            var documents = new List<T>();

            foreach (var task in tasks)
            {
                var azureSearchResult = await task;

                if (azureSearchResult?.GetResults() == null)
                {
                    break;
                }

                documents.AddRange(azureSearchResult.GetResults().Select(result => result.Document));
            }

            return (documents, retrievedAllData);
        }

        /// <summary>
        /// Set up the required index if its necessary.
        /// </summary>
        /// <param name="waitForIndexBuild">Should we await the index (re)build (if it necessary), or cancel with an error.</param>
        /// <param name="isFunding">Is it the funding index (implicitly, its the provider funding if not).</param>
        /// <typeparam name="T">The type of the index class.</typeparam>
        private async Task SetupIndexesIfRequired<T>(bool waitForIndexBuild, bool isFunding)
            where T : class
        {
            // If we are setup, we are golden
            if (GetSetupStatus(isFunding) == SetupStatus.Setup)
            {
                return;
            }

            var typeString = isFunding ? "Funding" : "ProviderFunding";
            var cosmosDbConnectionString = _configuration.Repositories.CosmosDb.ConnectionString;
            var cosmosDbCollection = isFunding ? _configuration.Repositories.CosmosDb.FundingCollection
                : _configuration.Repositories.CosmosDb.ProviderFundingCollection;
            var cosmosDbQuery = isFunding ? _configuration.Repositories.CosmosDb.FundingQuery
                : _configuration.Repositories.CosmosDb.ProviderFundingQuery;

            var classHash = AzureSearchHelper.GetClassThumbprint<T>(new List<string>
            {
                cosmosDbConnectionString,
                cosmosDbCollection,
                cosmosDbQuery
            });

            StoreClassHash(classHash, isFunding);

            if (GetSetupStatus(isFunding) == SetupStatus.InProgress)
            {
                if (await _searchServiceManager.HasIndexerEverRan<T>(classHash))
                {
                    SetIndexStatus(SetupStatus.Setup, isFunding);
                    return;
                }

                throw new Exception($"{typeString} index creation in progress via another request");
            }

            if (!await _searchServiceManager.DoesIndexAndIndexerExist<T>(classHash))
            {
                await RequestRebuild<T>(
                    classHash,
                    waitForIndexBuild,
                    isFunding,
                    cosmosDbConnectionString,
                    cosmosDbCollection,
                    cosmosDbQuery);

                // If we had waited, this might now be set
                if (GetSetupStatus(isFunding) == SetupStatus.Setup)
                {
                    return;
                }

                throw new Exception($"{typeString} index not ready - rebuild just requested");
            }

            SetIndexStatus(SetupStatus.Setup, isFunding);
        }

        /// <summary>
        /// Clean up the search term to match how its kept in the search index.
        /// </summary>
        /// <param name="originalSearchTerm">The original search term to clean up.</param>
        /// <returns>A cleaned up search term.</returns>
        private string GetCleanSearchTerm(string originalSearchTerm)
        {
            if (string.IsNullOrEmpty(originalSearchTerm))
            {
                return DefaultSearchPattern;
            }

            var cleanSearchTerm = _spacingCharacters.Replace(originalSearchTerm, "_");
            cleanSearchTerm = cleanSearchTerm.Replace(".", "_");
            cleanSearchTerm = _disallowedCharacters.Replace(cleanSearchTerm, string.Empty);
            cleanSearchTerm = _multipleDashes.Replace(cleanSearchTerm, "_");
            cleanSearchTerm = cleanSearchTerm.Trim(new[] { '_' });

            if (cleanSearchTerm == string.Empty)
            {
                return DefaultSearchPattern;
            }

            // Lucene regex pattern with wildcards at start and end of the search string:
            return $"/.*{cleanSearchTerm}.*/";
        }

        /// <summary>
        /// Generate the search options for the index search.
        /// </summary>
        /// <param name="fundingStreamParameters">The funding streams to look up.</param>
        /// <param name="isProviderFunding">Are we going to be querying the funding provider index.</param>
        /// <returns>A search options object to be passed to Azure Search service.</returns>
        private SearchOptions GetSearchOptions(FundingStreamParameters[] fundingStreamParameters, bool isProviderFunding, List<string> ukprns = null, bool byFundingVersion = false)
        {
            var id = byFundingVersion ? $"{FundingVersionPropertyName} desc" : (isProviderFunding ? $"{nameof(AzureProviderFundingSearchDocument.UniqueId)}" : $"{nameof(AzureFundingSearchDocument.Id)}");
            var result = new SearchOptions
            {
                Filter = BuildFilter(fundingStreamParameters, isProviderFunding, ukprns),
                QueryType = SearchQueryType.Full,
                Size = MaximumResultsPerAzureSearch,
                IncludeTotalCount = true
            };
            result.OrderBy.Add(id);

            return result;
        }

        /// <summary>
        /// Get a filter string composed of the filters we wish to apply.
        /// </summary>
        /// <param name="fundingStreamParameters">The funding streams to look up.</param>
        /// <param name="isProviderFunding">Are we going to be querying the funding provider index.</param>
        /// <returns>A filter string.</returns>
        private string BuildFilter(FundingStreamParameters[] fundingStreamParameters, bool isProviderFunding, List<string> excludedUkprns = null)
        {
            const string OrJoinString = " or ";

            var filterBuilder = new StringBuilder();
            var firstStream = true;

            if (fundingStreamParameters == null)
            {
                return filterBuilder.ToString();
            }

            foreach (var fundingStream in fundingStreamParameters)
            {
                if (!firstStream)
                {
                    filterBuilder.Append(OrJoinString);
                }

                filterBuilder.Append("(");

                // Return results until the end of the specified day:
                fundingStream.BeforeDateTime = fundingStream.BeforeDateTime.Date.AddDays(1).AddSeconds(-1);
                var beforeDateTimeOffset = new DateTimeOffset(fundingStream.BeforeDateTime).ToUniversalTime().ToString("O");

                var statusChangedDatePropertyName = isProviderFunding ?
                                   nameof(AzureProviderFundingSearchDocument.StatusChangedDate) : nameof(AzureFundingSearchDocument.StatusChangedDate);

                filterBuilder.Append($"{statusChangedDatePropertyName} le {beforeDateTimeOffset}");

                if (!string.IsNullOrEmpty(fundingStream.GroupingType) && !isProviderFunding)
                {
                    filterBuilder.Append($" and {nameof(AzureFundingSearchDocument.GroupingType)} eq '{fundingStream.GroupingType}'");
                }

                if (!string.IsNullOrEmpty(fundingStream.GroupingType) && isProviderFunding)
                {
                    filterBuilder.Append($" and {nameof(AzureProviderFundingSearchDocument.ParentProviderType)} eq '{fundingStream.GroupingType}'");
                }

                if (isProviderFunding && _configuration.FilterOnFundingVersion && _restrictedFundingStreamCodes.Contains(fundingStream.FundingStreamCode))
                {
                    filterBuilder.Append($" and ({FundingVersionPropertyName} eq '{RestrictedVersionValue}'");
                    filterBuilder.Append($" or {VariationReasonPropertyName}/any(r: {_restrictedVariationReasons.GetCosmosListExpression("or", "eq")}))");
                }

                if (fundingStream.Filters != null)
                {
                    foreach (var searchFilter in fundingStream.Filters)
                    {
                        // This is an invalid situation, so the obvious choice is to filter on the primary identifier itself
                        if (!isProviderFunding && searchFilter.PropertyName == SearchFilterParameters.FilterPropertyName.ParentPrimaryIdentifier)
                        {
                            searchFilter.PropertyName = SearchFilterParameters.FilterPropertyName.PrimaryIdentifier;
                        }

                        switch (searchFilter.PropertyName)
                        {
                            case SearchFilterParameters.FilterPropertyName.PrimaryIdentifier:
                                var primaryIdPropertyName = isProviderFunding ?
                                    nameof(AzureProviderFundingSearchDocument.OrganisationUkprn) : nameof(AzureFundingSearchDocument.GroupCode);
                                filterBuilder.Append($" and {primaryIdPropertyName} eq '{searchFilter.PropertyValue}'");

                                break;

                            case SearchFilterParameters.FilterPropertyName.FundingVersion:
                                filterBuilder.Append($" and {FundingVersionPropertyName} eq '{searchFilter.PropertyValue}'");

                                break;

                            case SearchFilterParameters.FilterPropertyName.ParentPrimaryIdentifier:
                                filterBuilder.Append($" and {nameof(AzureProviderFundingSearchDocument.ParentPrimaryIdentifier)} eq '{searchFilter.PropertyValue}'");

                                break;
                            case SearchFilterParameters.FilterPropertyName.GroupingReason:
                                var groupingReasonPropertyName = isProviderFunding ?
                                    nameof(AzureProviderFundingSearchDocument.GroupingReason) : nameof(AzureFundingSearchDocument.GroupingReason);
                                filterBuilder.Append($" and {groupingReasonPropertyName} eq '{searchFilter.PropertyValue}'");

                                break;
                            case SearchFilterParameters.FilterPropertyName.Ukprn:
                                var ukprnPropertyName = isProviderFunding ?
                                    nameof(AzureProviderFundingSearchDocument.OrganisationUkprn) : nameof(AzureFundingSearchDocument.GroupUkprn);
                                filterBuilder.Append($" and {ukprnPropertyName} eq '{searchFilter.PropertyValue}'");

                                break;

                            case SearchFilterParameters.FilterPropertyName.PrimaryIdentifierList:
                                var primaryIdentifierListPropertyName = isProviderFunding ?
                                    nameof(AzureProviderFundingSearchDocument.OrganisationUkprn) : nameof(AzureFundingSearchDocument.GroupCode);
                                filterBuilder.Append($" and search.in({primaryIdentifierListPropertyName}, '{searchFilter.PropertyValue}', '|')");

                                break;
                            case SearchFilterParameters.FilterPropertyName.Id:
                                var idPropertyName = isProviderFunding ?
                                    nameof(AzureProviderFundingSearchDocument.ParentId) : nameof(AzureFundingSearchDocument.Id);
                                filterBuilder.Append($" and {idPropertyName} eq '{searchFilter.PropertyValue}'");

                                break;
                            case SearchFilterParameters.FilterPropertyName.GroupName:
                                var groupNamePropertyName = isProviderFunding ?
                                    nameof(AzureProviderFundingSearchDocument.ParentName) : nameof(AzureFundingSearchDocument.GroupName);
                                filterBuilder.Append($" and {groupNamePropertyName} eq '{searchFilter.PropertyValue}'");

                                break;
                        }
                    }
                }

                if (fundingStream.PeriodCodes?.Length > 0)
                {
                    filterBuilder.Append(" and (");
                    var first = true;

                    var fundingPeriodPropertyName = isProviderFunding ?
                        nameof(AzureProviderFundingSearchDocument.FundingPeriodCode) : nameof(AzureFundingSearchDocument.FundingPeriodCode);

                    foreach (var periodCode in fundingStream.PeriodCodes)
                    {
                        if (!first)
                        {
                            filterBuilder.Append(OrJoinString);
                        }

                        filterBuilder.Append($"{fundingPeriodPropertyName} eq '{periodCode}'");
                        first = false;
                    }

                    filterBuilder.Append(")");
                }

                if (fundingStream.FundingStreamCode != null)
                {
                    filterBuilder.Append(" and (");
                    var first = true;

                    var fundingStreamPropertyName = isProviderFunding ?
                        nameof(AzureProviderFundingSearchDocument.FundingStreamCode) : nameof(AzureFundingSearchDocument.FundingStreamCode);

                    if (!first)
                    {
                        filterBuilder.Append(OrJoinString);
                    }

                    filterBuilder.Append($"{fundingStreamPropertyName} eq '{fundingStream.FundingStreamCode}'");
                    first = false;

                    filterBuilder.Append(")");
                }

                if (excludedUkprns != null && excludedUkprns.Any())
                {
                    filterBuilder.Append(" and (");

                    var ukprnPropertyName = isProviderFunding ?
                                    nameof(AzureProviderFundingSearchDocument.OrganisationUkprn) : nameof(AzureFundingSearchDocument.GroupUkprn);

                    filterBuilder.Append($"not search.in({ukprnPropertyName},'{string.Join(",", excludedUkprns)}')");

                    filterBuilder.Append(")");
                }

                firstStream = false;
                filterBuilder.Append(")");
            }

            return filterBuilder.ToString();
        }

        /// <summary>
        /// Requests rebuild.
        /// </summary>
        /// <typeparam name="T">The type of the paramater.</typeparam>
        /// <param name="classHash"> The class hash value.</param>
        /// <param name="waitForIndexBuild">A flag to indicater whether run the index rebuild or queue it.</param>
        /// <param name="isFunding">A flag to indicate the index build is a funding index.</param>
        /// <param name="cosmosDbConnectionString">The connections string for the Cosmos database.</param>
        /// <param name="cosmosDbCollection">The collection to use in the Cosmos database.</param>
        /// <param name="cosmosDbQuery">The query to execute in the Cosmos database.</param>
        /// <returns>An async Task containing the result of the RebuildIndex().</returns>
        private async Task RequestRebuild<T>(
            int classHash,
            bool waitForIndexBuild,
            bool isFunding,
            string cosmosDbConnectionString,
            string cosmosDbCollection,
            string cosmosDbQuery)
                where T : class
        {
            if (waitForIndexBuild)
            {
                await RebuildIndex<T>(classHash, isFunding, cosmosDbConnectionString, cosmosDbCollection, cosmosDbQuery);
            }
            else
            {
                _taskQueue.QueueBackgroundWorkItem(async cancellationToken =>
                {
                    await RebuildIndex<T>(classHash, isFunding, cosmosDbConnectionString, cosmosDbCollection, cosmosDbQuery);
                });
            }
        }

        /// <summary>
        /// Resets the index status flag.
        /// </summary>
        /// <param name="isFunding">The isFunding flag.</param>
        private void ResetIndexStatus(bool isFunding)
        {
            var currentStatus = GetSetupStatus(isFunding);
            if (currentStatus != SetupStatus.Setup)
            {
                return;
            }

            SetIndexStatus(SetupStatus.NotSetup, isFunding);
        }

        /// <summary>
        /// Gets the hash value for the class.
        /// </summary>
        /// <param name="isFunding">Flag to indicate it's a funding hash.</param>
        /// <returns>The hash value of either the funding index of the provider index depending upon the value of the isFunding flag.</returns>
        private int GetClassHash(bool isFunding)
        {
            var hash = isFunding ? _fundingIndexClassHash : _providerFundingIndexClassHash;

            if (!hash.HasValue)
            {
                throw new Exception("Class hash not saved");
            }

            return hash.Value;
        }

        /// <summary>
        /// Gets the setup status.
        /// </summary>
        /// <param name="isFunding">Flag to indicate it's a funding hash.</param>
        /// <returns>The SetupStatus of either the funding index setup or the provider index setup depending upon the value of the isFunding flag.</returns>
        private SetupStatus GetSetupStatus(bool isFunding)
        {
            return isFunding ? _fundingIndexSetup : _providerFundingIndexSetup;
        }

        /// <summary>
        /// Sets the setup status.
        /// </summary>
        /// <param name="status">The status value to set.</param>
        /// <param name="isFunding">Flag to indicate it's a funding setup.</param>
        private void SetIndexStatus(SetupStatus status, bool isFunding)
        {
            if (isFunding)
            {
                _fundingIndexSetup = status;
            }
            else
            {
                _providerFundingIndexSetup = status;
            }
        }

        /// <summary>
        /// Stores the hash value for the class.
        /// </summary>
        /// <param name="classHash">The hash value to set.</param>
        /// <param name="isFunding">Flag to indicate it's a funding hash.</param>
        private void StoreClassHash(int classHash, bool isFunding)
        {
            if (isFunding)
            {
                _fundingIndexClassHash = classHash;
            }
            else
            {
                _providerFundingIndexClassHash = classHash;
            }
        }

        /// <summary>
        /// Rebuilds the index.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="classHash">The hash value for the class.</param>
        /// <param name="isFunding">The isFunding flag.</param>
        /// <param name="cosmosDbConnectionString">The connections string for the Cosmos database.</param>
        /// <param name="cosmosDbCollection">The collection to use in the Cosmos database.</param>
        /// <param name="cosmosDbQuery">The query to execute in the Cosmos database.</param>
        /// <returns>An async Task containing the result of the RebuildIndex().</returns>
        private async Task RebuildIndex<T>(
            int classHash,
            bool isFunding,
            string cosmosDbConnectionString,
            string cosmosDbCollection,
            string cosmosDbQuery)
                where T : class
        {
            try
            {
                if (await _searchServiceManager.DoesIndexAndIndexerExist<T>(classHash))
                {
                    var indexerHasRan = await _searchServiceManager.HasIndexerEverRan<T>(classHash);
                    SetIndexStatus(indexerHasRan ? SetupStatus.Setup : SetupStatus.InProgress, isFunding);

                    return;
                }

                SetIndexStatus(SetupStatus.InProgress, isFunding);

                await _searchServiceManager.RebuildIndex<T>(
                    new CosmosDbDataSourceParameters
                    {
                        ConnectionString = cosmosDbConnectionString,
                        CollectionName = cosmosDbCollection,
                        Query = cosmosDbQuery,
                        HashCode = classHash
                    });

                SetIndexStatus(SetupStatus.Setup, isFunding);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, exception.Message);
                SetIndexStatus(SetupStatus.NotSetup, isFunding);
            }
        }

        #endregion
    }
}