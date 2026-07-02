using FluentAssertions;
using PDS.VYF.Data.Services.Enums;
using PDS.VYF.Data.Services.Implementations.AppServices;
using PDS.VYF.Data.Services.Models.AzSearchModels;
using PDS.VYF.Data.Services.Models.RequestModels;
using PDS.VYF.Data.Services.Tests.Mocks.InfraServices;

namespace PDS.VYF.Data.Services.Tests.Implementations.AppServices
{
    /// <summary>
    /// The ParentSearchServicesTests class.
    /// </summary>
    [TestClass]
    [TestCategory("Unit")]
    public class ParentSearchServicesTests
    {
        private readonly MockAzSearchSearchingServices mockAzSearchSearchingServices = new(false);
        private readonly ParentSearchServices parentSearchServices;

        private readonly List<LoggedInParentAzSearchModel> mockSearchIndexResult = new List<LoggedInParentAzSearchModel>
            {
                new LoggedInParentAzSearchModel
                {
                    Id = "GAG-AC-2425-Payment-AcademyTrust-10094589-2_0",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(0001, 01, 01),
                    GroupUkprn = "10094589",
                    IsParent = false,
                    ProviderFundings = new List<string>
                    {
                        "GAG-AC-2425-10095254-1_0",
                        "GAG-AC-2425-10094509-1_0",
                        "GAG-AC-2425-10094560-1_0"
                    },
                    GroupingType = "AcademyTrust",
                    GroupingReason = "Payment"
                },
                new LoggedInParentAzSearchModel
                {
                    Id = "GAG-AC-2425-Payment-AcademyTrust-10094589-3_0",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(0001, 01, 01),
                    GroupUkprn = "10094589",
                    IsParent = false,
                    ProviderFundings = new List<string>
                    {
                        "GAG-AC-2425-10094559-1_0",
                        "GAG-AC-2425-10095254-1_0",
                        "GAG-AC-2425-10094509-1_0",
                        "GAG-AC-2425-10094560-1_0"
                    },
                    GroupingType = "AcademyTrust",
                    GroupingReason = "Payment"
                },
                new LoggedInParentAzSearchModel
                {
                    Id = "GAG-AC-2425-Payment-AcademyTrust-10094589-1_0",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(0001, 01, 01),
                    GroupUkprn = "10094589",
                    IsParent = false,
                    ProviderFundings = new List<string>
                    {
                        "GAG-AC-2425-10094509-1_0",
                        "GAG-AC-2425-10094560-1_0"
                    },
                    GroupingType = "AcademyTrust",
                    GroupingReason = "Payment"
                }
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentSearchServicesTests"/> class.
        /// </summary>
        public ParentSearchServicesTests()
        {
            parentSearchServices = new ParentSearchServices(mockAzSearchSearchingServices.Object);
        }

        /// <summary>
        /// Searches the parent latest.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task SearchParent_Latest()
        {
            ParentRequest parentRequest = new()
            {
                HasToBeLatestFunding = true,
                FundingStreamPeriods = new() { "GAG-AC-2425" },
                SelectFields = new() { "Id", "ProviderFundings" },
                ListOfUKPRNs = new() { "10094589" }
            };

            var expectedResult = new List<LoggedInParentAzSearchModel>
            {
                new LoggedInParentAzSearchModel
                {
                    Id = "GAG-AC-2425-Payment-AcademyTrust-10094589-3_0",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(0001, 01, 01),
                    GroupUkprn = "10094589",
                    IsParent = false,
                    ProviderFundings = new List<string>
                    {
                        "GAG-AC-2425-10094559-1_0",
                        "GAG-AC-2425-10095254-1_0",
                        "GAG-AC-2425-10094509-1_0",
                        "GAG-AC-2425-10094560-1_0"
                    },
                    GroupingType = "AcademyTrust",
                    GroupingReason = "Payment"
                },
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Parent, mockSearchIndexResult);

            // Act
            var result = await parentSearchServices.SearchParent(parentRequest, default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Searches the parent not latest.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task SearchParent_Not_Latest()
        {
            ParentRequest parentRequest = new()
            {
                HasToBeLatestFunding = false,
                FundingStreamPeriods = new() { "GAG-AC-2425" },
                SelectFields = new() { "Id", "ProviderFundings" },
                ListOfUKPRNs = new() { "10094589" }
            };

            var expectedResult = new List<LoggedInParentAzSearchModel>
            {
                new LoggedInParentAzSearchModel
                {
                    Id = "GAG-AC-2425-Payment-AcademyTrust-10094589-3_0",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(0001, 01, 01),
                    GroupUkprn = "10094589",
                    IsParent = false,
                    ProviderFundings = new List<string>
                    {
                        "GAG-AC-2425-10094559-1_0",
                        "GAG-AC-2425-10095254-1_0",
                        "GAG-AC-2425-10094509-1_0",
                        "GAG-AC-2425-10094560-1_0"
                    },
                    GroupingType = "AcademyTrust",
                    GroupingReason = "Payment"
                },
                new LoggedInParentAzSearchModel
                {
                    Id = "GAG-AC-2425-Payment-AcademyTrust-10094589-2_0",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 2,
                    StatusChangedDate = new DateTime(0001, 01, 01),
                    GroupUkprn = "10094589",
                    IsParent = false,
                    ProviderFundings = new List<string>
                    {
                        "GAG-AC-2425-10095254-1_0",
                        "GAG-AC-2425-10094509-1_0",
                        "GAG-AC-2425-10094560-1_0"
                    },
                    GroupingType = "AcademyTrust",
                    GroupingReason = "Payment"
                },
                new LoggedInParentAzSearchModel
                {
                    Id = "GAG-AC-2425-Payment-AcademyTrust-10094589-1_0",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 1,
                    StatusChangedDate = new DateTime(0001, 01, 01),
                    GroupUkprn = "10094589",
                    IsParent = false,
                    ProviderFundings = new List<string>
                    {
                        "GAG-AC-2425-10094509-1_0",
                        "GAG-AC-2425-10094560-1_0"
                    },
                    GroupingType = "AcademyTrust",
                    GroupingReason = "Payment"
                }
            };

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Parent, mockSearchIndexResult);

            // Act
            var result = await parentSearchServices.SearchParent(parentRequest, default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }

        /// <summary>
        /// Determines whether [is parent true success].
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task IsParent_True_Success()
        {
            var parentUKPRN = "10094589";

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Parent, mockSearchIndexResult);

            // Act
            var result = await parentSearchServices.IsParent(parentUKPRN, new List<string> { "GAG-AC-2425" }, default);

            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// Determines whether [is parent false success].
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task IsParent_False_Success()
        {
            var parentUKPRN = "10081183";

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Parent, new List<LoggedInParentAzSearchModel>());

            // Act
            var result = await parentSearchServices.IsParent(parentUKPRN, new List<string> { "GAG-AC-2425" }, default);

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>
        /// Determines whether [is my child false success].
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task IsMyChild_False_Success()
        {
            var parentUKPRN = "10094589";
            var childUKPRN = "10081183";

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Parent, new List<LoggedInParentAzSearchModel>());

            // Act
            var result = await parentSearchServices.IsMyChild(parentUKPRN, childUKPRN, new List<string> { "GAG-AC-2425" }, default);

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>
        /// Determines whether [is my child true success].
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous unit test.
        /// </returns>
        [TestMethod]
        public async Task IsMyChild_True_Success()
        {
            var parentUKPRN = "10064281";
            var childUKPRN = "10081183";

            mockAzSearchSearchingServices.SetupSearchDocumentAsync(AzSearchIndexTypeEnum.LoggedIn_Parent, new List<LoggedInParentAzSearchModel>());

            // Act
            var result = await parentSearchServices.IsMyChild(parentUKPRN, childUKPRN, new List<string> { "GAG-AC-2425" }, default);

            // Assert
            result.Should().BeFalse();
        }
    }
}
