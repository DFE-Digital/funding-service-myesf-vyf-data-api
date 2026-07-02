using FluentAssertions;
using Moq;
using PDS.VYF.Data.Services.Abstracts.AppServices;
using PDS.VYF.Data.Services.Enums;
using PDS.VYF.Data.Services.Implementations.AppServices;
using PDS.VYF.Data.Services.Models.AzSearchModels;
using PDS.VYF.Data.Services.Models.RequestModels;
using PDS.VYF.Data.Services.Tests.Mocks.AppServices;
using PDS.VYF.Data.Services.Tests.Mocks.InfraServices;

namespace PDS.VYF.Data.Services.Tests.Implementations.AppServices
{
    /// <summary>
    /// The Search Child Tests.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class ChildSearchServicesTests
    {
        private readonly MockAzSearchSearchingServices mockAzSearchSearchingServices = new(false);
        private readonly MockInYearOpenerCalcServices mockInYearOpenerCalServices = new();
        private readonly ChildSearchServices childSearchServices;

        private readonly Mock<IComparisonServices> mockComparisonServices = new Mock<IComparisonServices>(MockBehavior.Strict);

        private readonly List<LoggedInChildAzSearchModel> mockSearchIndexResult = new()
        {
                new ()
                {
                    Id = "GAG-AC-2425-12345678-1_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new ()
                {
                    Id = "GAG-AC-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new ()
                {
                    Id = "GAG-AC-2425-12345678-3_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new ()
                {
                    Id = "GAG-AC-2526-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new ()
                {
                    Id = "1619-AS-2425-12345678-1_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new ()
                {
                    Id = "1619-AS-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                }
        };


        /// <summary>
        /// Initializes a new instance of the <see cref="ChildSearchServicesTests"/> class.
        /// </summary>
        public ChildSearchServicesTests()
        {
            childSearchServices = new ChildSearchServices(mockAzSearchSearchingServices.Object, mockInYearOpenerCalServices.Object, mockComparisonServices.Object);
        }

        /// <summary>
        /// Searches the child success.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        public async Task SearchChild_IYORemoval_False_HasToBeLatestFunding_True_IYOColNotRequested_Success()
        {
            // Arrange
            string[] inYearOperIds = new[] { "GAG-AC-2425-12345678-1_0", "GAG-AC-2425-12345678-2_0", "GAG-AC-2425-12345678-3_0" };
            mockInYearOpenerCalServices.SetupIsInYearOpener(inYearOperIds);

            var expectedResult = new List<LoggedInChildAzSearchModel>
            {
                new ()
                {
                    Id = "GAG-AC-2425-12345678-3_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new ()
                {
                    Id = "1619-AS-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new ()
                {
                    Id = "GAG-AC-2526-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, mockSearchIndexResult);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = true,
                HasIYOToBeRemoved = false,
            };

            // Act
            var result = await childSearchServices.SearchChild(childRequest, default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            mockInYearOpenerCalServices.VerifyIsInYearOpener(false, mockSearchIndexResult.Count, inYearOperIds);
        }

        /// <summary>
        /// Searches the child success.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task SearchChild_IYORemoval_True_HasToBeLatestFunding_True_IYOColNotRequested_Success()
        {
            // Arrange
            string[] inYearOperIds = new[] { "GAG-AC-2425-12345678-1_0", "GAG-AC-2425-12345678-2_0", "GAG-AC-2425-12345678-3_0" };
            mockInYearOpenerCalServices.SetupIsInYearOpener(inYearOperIds);

            var expectedResult = new List<LoggedInChildAzSearchModel>
            {
                new ()
                {
                    Id = "GAG-AC-2526-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = false
                },
                new ()
                {
                    Id = "1619-AS-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = false
                }
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, mockSearchIndexResult);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = true,
                HasIYOToBeRemoved = true,
            };

            // Act
            var result = await childSearchServices.SearchChild(childRequest, default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            mockInYearOpenerCalServices.VerifyIsInYearOpener(true, mockSearchIndexResult.Count, inYearOperIds);
        }

        /// <summary>
        /// Searches the child success.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task SearchChild_IYORemoval_True_HasToBeLatestFunding_True_LastVersionOnlyIYO_Success()
        {
            // Arrange
            string[] inYearOperIds = new[] { "GAG-AC-2425-12345678-3_0" };
            mockInYearOpenerCalServices.SetupIsInYearOpener(inYearOperIds);

            var expectedResult = new List<LoggedInChildAzSearchModel>
            {
                new ()
                {
                    Id = "GAG-AC-2526-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = false,
                },
                new ()
                {
                    Id = "1619-AS-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = false,
                },
                new ()
                {
                    Id = "GAG-AC-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = false,
                },
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, this.mockSearchIndexResult);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = true,
                HasIYOToBeRemoved = true,
            };

            // Act
            var result = await childSearchServices.SearchChild(childRequest, default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            mockInYearOpenerCalServices.VerifyIsInYearOpener(true, mockSearchIndexResult.Count, inYearOperIds);
        }

        /// <summary>
        /// Searches the child success.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task SearchChild_IYORemoval_False_HasToBeLatestFunding_True_IYOColRequested_Success()
        {
            // Arrange
            string[] inYearOperIds = new[] { "GAG-AC-2425-12345678-1_0", "GAG-AC-2425-12345678-2_0", "GAG-AC-2425-12345678-3_0" };
            mockInYearOpenerCalServices.SetupIsInYearOpener(inYearOperIds);

            var expectedResult = new List<LoggedInChildAzSearchModel>
            {
                new ()
                {
                    Id = "GAG-AC-2425-12345678-3_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = true,
                },
                new ()
                {
                    Id = "1619-AS-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = false,
                },
                new ()
                {
                    Id = "GAG-AC-2526-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = false,
                },
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, mockSearchIndexResult);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = true,
                HasIYOToBeRemoved = false,
            };

            childRequest.SetSelectFields(a => new { a.InYearOpener });

            // Act
            var result = await childSearchServices.SearchChild(childRequest, default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            mockInYearOpenerCalServices.VerifyIsInYearOpener(true, mockSearchIndexResult.Count, inYearOperIds);
        }

        /// <summary>
        /// Searches the child success.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task SearchChild_IYORemoval_False_HasToBeLatestFunding_False_IYOColRequested_Success()
        {
            // Arrange
            string[] inYearOperIds = new[] { "GAG-AC-2425-12345678-1_0", "GAG-AC-2425-12345678-2_0", "GAG-AC-2425-12345678-3_0" };
            mockInYearOpenerCalServices.SetupIsInYearOpener(inYearOperIds);

            var expectedResult = new List<LoggedInChildAzSearchModel>
            {
                new ()
                {
                    Id = "GAG-AC-2425-12345678-1_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = true,
                },
                new ()
                {
                    Id = "GAG-AC-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = true,
                },
                new ()
                {
                    Id = "GAG-AC-2425-12345678-3_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = true,
                },
                new ()
                {
                    Id = "GAG-AC-2526-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = false,
                },
                new ()
                {
                    Id = "1619-AS-2425-12345678-1_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = false,
                },
                new ()
                {
                    Id = "1619-AS-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                    InYearOpener = false,
                }
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, mockSearchIndexResult);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = false,
                HasIYOToBeRemoved = false,
            };

            childRequest.SetSelectFields(a => new { a.InYearOpener });

            // Act
            var result = await childSearchServices.SearchChild(childRequest, default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            mockInYearOpenerCalServices.VerifyIsInYearOpener(true, mockSearchIndexResult.Count, inYearOperIds);
        }

        /// <summary>
        /// Searches the child success.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        /// <param name="hasToBeLatestFunding">if set to <c>true</c> [has to be latest funding].</param>
        /// <param name="hasIYOToBeRemoved">if set to <c>true</c> [has iyo to be removed].</param>
        /// <param name="doFindInYearOpener">if set to <c>true</c> [do find in year opener].</param>
        /// <param name="doFindIsLatest">if set to <c>true</c> [do find is latest].</param>
        /// <param name="hasSelectFieldsSetByClient">if set to <c>true</c> [has select fields set by client].</param>
        /// <param name="isWholeFundingPeriodIYO">if set to <c>true</c> [is whole funding period iyo].</param>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO, true, false, false, false, false, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO, true, false, false, false, true, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO, true, false, false, true, false, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO, true, false, false, true, true, true)]

        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO, true, false, true, false, false, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO, true, false, true, false, true, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO, true, false, true, true, false, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO, true, false, true, true, true, true)]

        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO, true, true, false, false, false, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO, true, true, false, false, true, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO, true, true, false, true, false, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO, true, true, false, true, true, true)]

        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO, true, true, true, false, false, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO, true, true, true, false, true, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO, true, true, true, true, false, true)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO, true, true, true, true, true, true)]

        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO, true, false, false, false, false, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO, true, false, false, false, true, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO, true, false, false, true, false, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO, true, false, false, true, true, false)]

        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO, true, false, true, false, false, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO, true, false, true, false, true, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO, true, false, true, true, false, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO, true, false, true, true, true, false)]

        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO, true, true, false, false, false, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO, true, true, false, false, true, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO, true, true, false, true, false, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO, true, true, false, true, true, false)]

        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO, true, true, true, false, false, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO, true, true, true, false, true, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO, true, true, true, true, false, false)]
        [DataRow(SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO, true, true, true, true, true, false)]
        public async Task SearchChild_Success(
            SearchChildScenarioEnum scenarioName,
            bool hasToBeLatestFunding,
            bool hasIYOToBeRemoved,
            bool doFindInYearOpener,
            bool doFindIsLatest,
            bool hasSelectFieldsSetByClient,
            bool isWholeFundingPeriodIYO)
        {
            // Arrange
            string[] inYearOpenerIds = isWholeFundingPeriodIYO ? new string[] { "GAG-AC-2425-12345678-1_0", "GAG-AC-2425-12345678-2_0", "GAG-AC-2425-12345678-3_0" } : new string[] { "GAG-AC-2425-12345678-3_0" };

            var expectedResult = GetExpectedResult(scenarioName);
            mockInYearOpenerCalServices.SetupIsInYearOpener(inYearOpenerIds);
            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, mockSearchIndexResult);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = hasToBeLatestFunding,
                HasIYOToBeRemoved = hasIYOToBeRemoved,
                DoFindIsLatest = doFindIsLatest,
            };

            if (hasSelectFieldsSetByClient && doFindInYearOpener)
            {
                childRequest.SetSelectFields(a => new { a.InYearOpener });
            }
            else if (hasSelectFieldsSetByClient)
            {
                childRequest.SetSelectFields(a => new { a.Id, a.FundingStreamCode });
            }


            // Act
            var result = await childSearchServices.SearchChild(childRequest, default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);

            // mockAzSearchSearchingServices.VerifySearchDocumentAsync<LoggedInChildAzSearchModel>(AzSearchIndexTypeEnum.LoggedIn_Child, false, GetExpectedSelectFields(scenarioName));
            mockInYearOpenerCalServices.VerifyIsInYearOpener(!hasSelectFieldsSetByClient || hasIYOToBeRemoved || doFindInYearOpener, mockSearchIndexResult.Count, inYearOpenerIds);
        }

        private List<LoggedInChildAzSearchModel>? GetExpectedResult(SearchChildScenarioEnum searchChildScenarioEnum)
        {
            switch (searchChildScenarioEnum)
            {
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO:
                    return new List<LoggedInChildAzSearchModel>
                    {
                        new ()
                        {
                            Id = "GAG-AC-2425-12345678-3_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "GAG-AC-2425",
                            FundingVersionInt = 3,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            InYearOpener = true,
                        },
                        new ()
                        {
                            Id = "1619-AS-2425-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "1619-AS-2425",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            InYearOpener = false,
                        },
                        new ()
                        {
                            Id = "GAG-AC-2526-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "GAG-AC-2526",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            InYearOpener = false,
                        },
                    };
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO:
                    return new List<LoggedInChildAzSearchModel>
                    {
                        new ()
                        {
                            Id = "1619-AS-2425-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "1619-AS-2425",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            InYearOpener = false,
                        },
                        new ()
                        {
                            Id = "GAG-AC-2526-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "GAG-AC-2526",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            InYearOpener = false,
                        },
                    };
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO:
                    return new List<LoggedInChildAzSearchModel>
                    {
                        new ()
                        {
                            Id = "GAG-AC-2425-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "GAG-AC-2425",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            InYearOpener = false,
                        },
                        new ()
                        {
                            Id = "1619-AS-2425-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "1619-AS-2425",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            InYearOpener = false,
                        },
                        new ()
                        {
                            Id = "GAG-AC-2526-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "GAG-AC-2526",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            InYearOpener = false,
                        },
                    };
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO:
                    return new List<LoggedInChildAzSearchModel>
                    {
                        new ()
                        {
                            Id = "GAG-AC-2425-12345678-3_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "GAG-AC-2425",
                            FundingVersionInt = 3,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            InYearOpener = true,
                            IsLatest = true,
                        },
                        new ()
                        {
                            Id = "1619-AS-2425-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "1619-AS-2425",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            InYearOpener = false,
                            IsLatest = true,
                        },
                        new ()
                        {
                            Id = "GAG-AC-2526-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "GAG-AC-2526",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            InYearOpener = false,
                            IsLatest = true,
                        },
                    };
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO:
                    return new List<LoggedInChildAzSearchModel> { };
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO:
                    return new List<LoggedInChildAzSearchModel> { };

                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO:
                    return new List<LoggedInChildAzSearchModel>
                    {
                        new ()
                        {
                            Id = "GAG-AC-2425-12345678-3_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "GAG-AC-2425",
                            FundingVersionInt = 3,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                        },
                        new ()
                        {
                            Id = "1619-AS-2425-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "1619-AS-2425",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                        },
                        new ()
                        {
                            Id = "GAG-AC-2526-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "GAG-AC-2526",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                        },
                    };
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO:
                case SearchChildScenarioEnum.LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO:
                    return new List<LoggedInChildAzSearchModel>
                    {
                        new ()
                        {
                            Id = "GAG-AC-2425-12345678-3_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "GAG-AC-2425",
                            FundingVersionInt = 3,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            IsLatest = true,
                        },
                        new ()
                        {
                            Id = "1619-AS-2425-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "1619-AS-2425",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            IsLatest = true,
                        },
                        new ()
                        {
                            Id = "GAG-AC-2526-12345678-2_0",
                            OrganisationUkprn = "12345678",
                            FundingStreamPeriod = "GAG-AC-2526",
                            FundingVersionInt = 2,
                            StatusChangedDate = new DateTime(2024, 05, 18),
                            IsLatest = true,
                        },
                    };
                default:
                    return null;
            }
        }

        private List<string> GetExpectedSelectFields(string scenarioName)
        {
            if (scenarioName == "Latest Statements across Main/IYO - No IYO calc - No IsLatest Calc")
            {
                return new List<string> { "OrganisationUkprn", "FundingStreamPeriod", "FundingVersionInt", "StatusChangedDate", "OpenReason", "CalculationsForSummary", "FundingPeriodCode", "DateOpened", "IsIndicative", "YearFrom", "YearTo" };
            }
            else if (scenarioName == "Latest Statements across Main/IYO - No IYO calc - Calc IsLatest")
            {
                return new List<string> { };
            }
            else if (scenarioName == "Latest Statements across Main/IYO - calc IYO - No IsLatest Calc")
            {
                return new List<string> { };
            }
            else if (scenarioName == "Latest Statements across Main/IYO - calc IYO - Calc IsLatest")
            {
                return new List<string> { };
            }

            return new List<string> { };
        }


        /// <summary>
        /// The Search Child Scenario Enum.
        /// </summary>
        public enum SearchChildScenarioEnum
        {
            // Whole Funding Period is IYO (GAG-AC-2425)

            /// <summary>
            /// The latest statements iyo removal no iyo calculate no is latest calculate no select field set no whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate no is latest calculate no select field set yes whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate no is latest calculate yes select field set no whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate no is latest calculate yes select field set yes whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO,


            /// <summary>
            /// The latest statements iyo removal no iyo calculate yes is latest calculate no select field set no whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate yes is latest calculate no select field set yes whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate yes is latest calculate yes select field set no whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate yes is latest calculate yes select field set yes whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO,


            /// <summary>
            /// The latest statements iyo removal yes iyo calculate no is latest calculate no select field set no whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate no is latest calculate no select field set yes whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate no is latest calculate yes select field set no whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate no is latest calculate yes select field set yes whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate yes is latest calculate no select field set no whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate yes is latest calculate no select field set yes whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate yes is latest calculate yes select field set no whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_WholeFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate yes is latest calculate yes select field set yes whole fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_WholeFPIYO,

            // Partial Funding Period is IYO (last version of GAG-AC-2425)

            /// <summary>
            /// The latest statements iyo removal no iyo calculate no is latest calculate no select field set no partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate no is latest calculate no select field set yes partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate no is latest calculate yes select field set no partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate no is latest calculate yes select field set yes partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate yes is latest calculate no select field set no partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate yes is latest calculate no select field set yes partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate yes is latest calculate yes select field set no partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal no iyo calculate yes is latest calculate yes select field set yes partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_No_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO,


            /// <summary>
            /// The latest statements iyo removal yes iyo calculate no is latest calculate no select field set no partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate no is latest calculate no select field set yes partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate no is latest calculate yes select field set no partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate no is latest calculate yes select field set yes partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_No_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO,


            /// <summary>
            /// The latest statements iyo removal yes iyo calculate yes is latest calculate no select field set no partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_No_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate yes is latest calculate no select field set yes partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_No_SelectFieldSet_Yes_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate yes is latest calculate yes select field set no partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_No_PartialFPIYO,

            /// <summary>
            /// The latest statements iyo removal yes iyo calculate yes is latest calculate yes select field set yes partial fpiyo
            /// </summary>
            LatestStatements_IYORemoval_Yes_IYOCalc_Yes_IsLatestCalc_Yes_SelectFieldSet_Yes_PartialFPIYO,
        }

        /// <summary>
        /// Searches the children of a parent iyo latest.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task SearchChildrenOfAParent_IYO_Latest()
        {
            var parentUkprn = "10064281";

            var mockSearchIndexResult = new List<LoggedInParentAzSearchModel>
            {
                new LoggedInParentAzSearchModel
                {
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(0001, 01, 01),
                    FundingStreamGroupingTypeReason = "GAG-AcademyTrust-Payment",
                    GroupUkprn = "10064281",
                    IsParent = false,
                    ProviderFundings = new List<string>
                    {
                        "GAG-AC-2425-10081183-1_0",
                        "GAG-AC-2425-10063092-1_0",
                        "GAG-AC-2425-10061924-1_0",
                        "GAG-AC-2425-10061926-1_0",
                        "GAG-AC-2425-10061930-1_0",
                        "GAG-AC-2425-10061932-1_0"
                    },
                    ChildUKPRNs = new List<string>
                    {
                        "10081183",
                        "10063092",
                        "10061924",
                        "10061926",
                        "10061930",
                        "10061932",
                    }
                },
            };

            var mockparentresult = new List<LoggedInChildAzSearchModel>
            {
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10081183-1_0",
                    OrganisationUkprn = "10081183",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10081183-2_0",
                    OrganisationUkprn = "10081183",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10063092-1_0",
                    OrganisationUkprn = "10063092",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10061926-1_0",
                    OrganisationUkprn = "10061926",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10061924-1_0",
                    OrganisationUkprn = "10061924",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10061930-1_0",
                    OrganisationUkprn = "10061930",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10061932-1_0",
                    OrganisationUkprn = "10061932",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                }
            };

            string[] inYearOpenerIds = new string[]
            {
               "GAG-AC-2425-10081183-2_0"
            };

            var expectedresult = new List<LoggedInChildAzSearchModel>
            {
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10081183-1_0",
                    OrganisationUkprn = "10081183",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                    InYearOpener = false
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10063092-1_0",
                    OrganisationUkprn = "10063092",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                    InYearOpener = false
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10061924-1_0",
                    OrganisationUkprn = "10061924",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                    InYearOpener = false
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10061926-1_0",
                    OrganisationUkprn = "10061926",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                    InYearOpener = false
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10061930-1_0",
                    OrganisationUkprn = "10061930",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                    InYearOpener = false
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10061932-1_0",
                    OrganisationUkprn = "10061932",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 03, 15),
                    InYearOpener = false
                }
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Parent, mockSearchIndexResult);
            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, mockparentresult);
            mockInYearOpenerCalServices.SetupIsInYearOpener(inYearOpenerIds);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = true,
                HasIYOToBeRemoved = true,
                FundingStreamPeriods = new() { "GAG-AC-2425" },
                SelectFields = new() { "Id", "StatementType", "OrganisationUkprn", "ParentInfo/ParentId" }
            };

            var result = await childSearchServices.SearchChildrenOfAParent(parentUkprn, childRequest, default);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedresult);
        }

        /// <summary>
        /// Searches the children of a parent non iyo latest.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task SearchChildrenOfAParent_nonIYO_Latest()
        {
            var parentUkprn = "10094589";

            var mockSearchIndexResult = new List<LoggedInParentAzSearchModel>
            {
                new LoggedInParentAzSearchModel
                {
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(0001, 01, 01),
                    FundingStreamGroupingTypeReason = "GAG-AcademyTrust-Payment",
                    GroupUkprn = "10094589",
                    IsParent = false,
                    ProviderFundings = new List<string>
                    {
                        "GAG-AC-2425-10094559-1_0",
                        "GAG-AC-2425-10095254-1_0",
                        "GAG-AC-2425-10094509-1_0",
                        "GAG-AC-2425-10094560-1_0"
                    },
                    ChildUKPRNs = new List<string>
                    {
                        "10094559",
                        "10095254",
                        "10094509",
                        "10094560"
                    }
                },
            };

            var mockparentresult = new List<LoggedInChildAzSearchModel>
            {
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10095254-1_0",
                    OrganisationUkprn = "10095254",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 25),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094560-1_0",
                    OrganisationUkprn = "10094560",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 09),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094559-1_0",
                    OrganisationUkprn = "10094559",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 07),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094509-1_0",
                    OrganisationUkprn = "10094509",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 09),
                }
            };

            string[] inYearOpenerIds = new string[]
            {
            };

            var expectedresult = new List<LoggedInChildAzSearchModel>
            {
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10095254-1_0",
                    OrganisationUkprn = "10095254",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 25),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094560-1_0",
                    OrganisationUkprn = "10094560",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 09),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094559-1_0",
                    OrganisationUkprn = "10094559",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 07),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094509-1_0",
                    OrganisationUkprn = "10094509",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 09),
                }
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Parent, mockSearchIndexResult);
            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, mockparentresult);
            mockInYearOpenerCalServices.SetupIsInYearOpener(inYearOpenerIds);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = true,
                HasIYOToBeRemoved = false,
                FundingStreamPeriods = new() { "GAG-AC-2425" },
                SelectFields = new() { "Id", "StatementType", "OrganisationUkprn", "ParentInfo/ParentId" }
            };

            var result = await childSearchServices.SearchChildrenOfAParent(parentUkprn, childRequest, default);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedresult);
        }

        /// <summary>
        /// Searches the children of a parent iyo not latest.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task SearchChildrenOfAParent_IYO_NotLatest()
        {
            var parentUkprn = "10094589";

            var mockSearchIndexResult = new List<LoggedInParentAzSearchModel>
            {
                new LoggedInParentAzSearchModel
                {
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(0001, 01, 01),
                    FundingStreamGroupingTypeReason = "GAG-AcademyTrust-Payment",
                    GroupUkprn = "10094589",
                    IsParent = false,
                    ProviderFundings = new List<string>
                    {
                        "GAG-AC-2425-10094559-1_0",
                        "GAG-AC-2425-10095254-1_0",
                        "GAG-AC-2425-10094509-1_0",
                        "GAG-AC-2425-10094560-1_0"
                    },
                    ChildUKPRNs = new List<string>
                    {
                        "10094559",
                        "10095254",
                        "10094509",
                        "10094560",
                    }
                },
            };

            var mockparentresult = new List<LoggedInChildAzSearchModel>
            {
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10095254-1_0",
                    OrganisationUkprn = "10095254",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 25),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094560-1_0",
                    OrganisationUkprn = "10094560",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 09),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094559-1_0",
                    OrganisationUkprn = "10094559",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 07),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094509-1_0",
                    OrganisationUkprn = "10094509",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 09),
                }
            };

            string[] inYearOpenerIds = new string[] { "GAG-AC-2425-10094560-1_0", "GAG-AC-2425-10094559-1_0", "GAG-AC-2425-10094509-1_0" };

            var expectedresult = new List<LoggedInChildAzSearchModel>
            {
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10095254-1_0",
                    OrganisationUkprn = "10095254",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 25),
                    InYearOpener = false
                },
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Parent, mockSearchIndexResult);
            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, mockparentresult);
            mockInYearOpenerCalServices.SetupIsInYearOpener(inYearOpenerIds);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = false,
                HasIYOToBeRemoved = true,
                FundingStreamPeriods = new() { "GAG-AC-2425" },
                SelectFields = new() { "Id", "StatementType", "OrganisationUkprn", "ParentInfo/ParentId" }
            };

            var result = await childSearchServices.SearchChildrenOfAParent(parentUkprn, childRequest, default);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedresult);
        }

        /// <summary>
        /// Searches the children of a parent non iyo not latest.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task SearchChildrenOfAParent_NonIYO_NotLatest()
        {
            var parentUkprn = "10094589";

            var mockSearchIndexResult = new List<LoggedInParentAzSearchModel>
            {
                new LoggedInParentAzSearchModel
                {
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(0001, 01, 01),
                    FundingStreamGroupingTypeReason = "GAG-AcademyTrust-Payment",
                    GroupUkprn = "10094589",
                    IsParent = false,
                    ProviderFundings = new List<string>
                    {
                        "GAG-AC-2425-10094559-1_0",
                        "GAG-AC-2425-10095254-1_0",
                        "GAG-AC-2425-10094509-1_0",
                        "GAG-AC-2425-10094560-1_0"
                    },
                    ChildUKPRNs = new List<string>
                    {
                        "10094559",
                        "10095254",
                        "10094509",
                        "10094560",
                    }
                },
            };

            var mockparentresult = new List<LoggedInChildAzSearchModel>
            {
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10095254-1_0",
                    OrganisationUkprn = "10095254",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 25),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094560-1_0",
                    OrganisationUkprn = "10094560",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 09),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094559-1_0",
                    OrganisationUkprn = "10094559",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 07),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094509-1_0",
                    OrganisationUkprn = "10094509",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 09),
                }
            };

            string[] inYearOpenerIds = new string[] { "GAG-AC-2425-10094560-1_0", "GAG-AC-2425-10094559-1_0", "GAG-AC-2425-10094509-1_0" };

            var expectedresult = new List<LoggedInChildAzSearchModel>
            {
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10095254-1_0",
                    OrganisationUkprn = "10095254",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 25),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094560-1_0",
                    OrganisationUkprn = "10094560",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 09),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094559-1_0",
                    OrganisationUkprn = "10094559",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 07),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-10094509-1_0",
                    OrganisationUkprn = "10094509",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 04, 09),
                }
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Parent, mockSearchIndexResult);
            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, mockparentresult);
            mockInYearOpenerCalServices.SetupIsInYearOpener(inYearOpenerIds);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = false,
                HasIYOToBeRemoved = false,
                FundingStreamPeriods = new() { "GAG-AC-2425" },
                SelectFields = new() { "Id", "StatementType", "OrganisationUkprn", "ParentInfo/ParentId" }
            };

            var result = await childSearchServices.SearchChildrenOfAParent(parentUkprn, childRequest, default);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedresult);
        }

        /// <summary>
        /// Searches the latest child success.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task IsLatestStatement_True_Success()
        {
            var mockSearchIndexResult = new List<LoggedInChildAzSearchModel>
            {
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-12345678-1_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-12345678-3_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2526-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "1619-AS-2425-12345678-1_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "1619-AS-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                }
            };


            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, mockSearchIndexResult);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = true,
                HasIYOToBeRemoved = false,
            };
            string id = "GAG-AC-2425-12345678-3_0";

            var result = await childSearchServices.IsLatestStatement(id, childRequest, default);

            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// Determines whether [is latest statement false success].
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task IsLatestStatement_False_Success()
        {
            var mockSearchIndexResult = new List<LoggedInChildAzSearchModel>
            {
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-12345678-1_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2425-12345678-3_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "GAG-AC-2526-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "1619-AS-2425-12345678-1_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                },
                new LoggedInChildAzSearchModel
                {
                    Id = "1619-AS-2425-12345678-2_0",
                    OrganisationUkprn = "12345678",
                    FundingStreamPeriod = "1619-AS-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 05, 18),
                }
            };


            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Child, mockSearchIndexResult);

            ChildRequest childRequest = new()
            {
                HasToBeLatestFunding = true,
                HasIYOToBeRemoved = false,
            };
            string id = "1619-AS-2425-12345678-1_0";

            var result = await childSearchServices.IsLatestStatement(id, childRequest, default);

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>
        /// Returns empty list when no results found.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task GetCurrentChildUkprnsForParent_NoResults_ReturnsEmpty()
        {
            // Arrange
            mockAzSearchSearchingServices.SetupSearchDocumentAsync(
                AzSearchIndexTypeEnum.LoggedIn_Child,
                new List<LoggedInChildAzSearchModel>());

            // Act
            var result = await childSearchServices.GetCurrentChildUkprnsForParent(
                "10000001",
                new List<string> { "20000001", "12345678" },
                default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        /// <summary>
        /// Returns only children linked to the given parent.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task GetCurrentChildUkprnsForParent_FiltersByParentUkprn()
        {
            // Arrange
            var searchResults = new List<LoggedInChildAzSearchModel>
            {
                new ()
                {
                    OrganisationUkprn = "20000001",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 01, 01),
                    ParentInfo = new()
                    {
                        new () { ParentUKPRN = "10000001" }
                    }
                },
                new ()
                {
                    OrganisationUkprn = "20000002",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 01, 01),
                    ParentInfo = new()
                    {
                        new () { ParentUKPRN = "99999999" }
                    }
                }
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(
                AzSearchIndexTypeEnum.LoggedIn_Child,
                searchResults);

            // Act
            var result = await childSearchServices.GetCurrentChildUkprnsForParent(
                "10000001",
                new List<string> { "20000001", "20000002" },
                default);

            // Assert
            result.Should().ContainSingle();
            result.Should().Contain("20000001");
        }

        /// <summary>
        /// Picks latest record per child by StatusChangedDate then FundingVersionInt.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task GetCurrentChildUkprnsForParent_PicksLatestChildRecord()
        {
            // Arrange
            var searchResults = new List<LoggedInChildAzSearchModel>
            {
                new ()
                {
                    OrganisationUkprn = "20000001",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 01, 01),
                    ParentInfo = new() { new() { ParentUKPRN = "10000001" } }
                },
                new ()
                {
                    OrganisationUkprn = "20000001",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 02, 01),
                    ParentInfo = new() { new() { ParentUKPRN = "10000001" } }
                }
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(
                AzSearchIndexTypeEnum.LoggedIn_Child,
                searchResults);

            // Act
            var result = await childSearchServices.GetCurrentChildUkprnsForParent(
                "10000001",
                new List<string> { "20000001" },
                default);

            // Assert
            result.Should().HaveCount(1);
            result[0].Should().Be("20000001");
        }

        /// <summary>
        /// Returns distinct child UKPRNs even when multiple rows exist.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task GetCurrentChildUkprnsForParent_ReturnsDistinctUkprns()
        {
            // Arrange
            var searchResults = new List<LoggedInChildAzSearchModel>
            {
                new ()
                {
                    OrganisationUkprn = "20000001",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 01, 01),
                    ParentInfo = new() { new() { ParentUKPRN = "10000001" } }
                },
                new ()
                {
                    OrganisationUkprn = "20000001",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(2024, 01, 02),
                    ParentInfo = new() { new() { ParentUKPRN = "10000001" } }
                }
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(
                AzSearchIndexTypeEnum.LoggedIn_Child,
                searchResults);

            // Act
            var result = await childSearchServices.GetCurrentChildUkprnsForParent(
                "10000001",
                new List<string> { "20000001" },
                default);

            // Assert
            result.Should().HaveCount(1);
            result.Should().Contain("20000001");
        }

        /// <summary>
        /// Ignores children with null ParentInfo.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task GetCurrentChildUkprnsForParent_IgnoresNullParentInfo()
        {
            // Arrange
            var searchResults = new List<LoggedInChildAzSearchModel>
            {
                new ()
                {
                    OrganisationUkprn = "20000001",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(2024, 01, 01),
                    ParentInfo = null
                }
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(
                AzSearchIndexTypeEnum.LoggedIn_Child,
                searchResults);

            // Act
            var result = await childSearchServices.GetCurrentChildUkprnsForParent(
                "10000001",
                new List<string> { "20000001" },
                default);

            // Assert
            result.Should().BeEmpty();
        }
    }
}