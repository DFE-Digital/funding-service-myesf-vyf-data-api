using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.VYF.Data.Services.Enums;
using PDS.VYF.Data.Services.Implementations.AppServices;
using PDS.VYF.Data.Services.Models.AzSearchModels;
using PDS.VYF.Data.Services.Models.RequestModels;
using PDS.VYF.Data.Services.Models.ResponseModels;
using System;
using System.Data;

namespace PDS.VYF.Data.Services.Tests.Implementations.AppServices
{
    [TestClass]
    public class ComparisonServicesTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private ComparisonServices CreateComparisonServices()
        {
            return new ComparisonServices();
        }

        [TestMethod]
        [DataRow("GAG-AC-2526-12345678-4_0", true)]
        [DataRow("GAG-AC-2526-12345678-3_0", false)]
        [DataRow("GAG-AC-2425-12345678-8_0", false)]
        public void IsLatestProviderFunding_MultipleScenarios_AsExpected(string currentProviderFundingId, bool expectedResult)
        {
            // Arrange
            var comparisonServices = this.CreateComparisonServices();
            IEnumerable<LoggedInChildAzSearchModel> childFundings = new List<LoggedInChildAzSearchModel>
            {
                new LoggedInChildAzSearchModel()
                {
                    Id = "GAG-AC-2526-12345678-4_0",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 4,
                    StatusChangedDate = new DateTime(2025, 1, 1),
                    StatusChangedDateOnly = new DateTime(2025, 1, 1),
                    InYearOpener = false,
                    IsIndicative = false,
                },
                new LoggedInChildAzSearchModel()
                {
                    Id = "GAG-AC-2526-12345678-3_0",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(2024, 12, 1),
                    StatusChangedDateOnly = new DateTime(2024, 12, 1),
                    InYearOpener = false,
                    IsIndicative = false,
                },
                new LoggedInChildAzSearchModel()
                {
                    Id = "GAG-AC-2425-12345678-8_0",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(2024, 12, 1),
                    InYearOpener = false,
                    IsIndicative = false,
                },
            };

            // Act
            var result = comparisonServices.IsLatestProviderFunding(
                currentProviderFundingId,
                childFundings);

            // Assert
            result.Should().Be(expectedResult);
            this.mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(true, true, true, true, true)]
        [DataRow(true, false, true, false, false)]
        [DataRow(false, true, false, true, false)]
        [DataRow(true, true, true, false, false)]
        public void GetPreviousStatementCurrentYear_CurrentStatementIsUpdated_AsExpected(bool currentIsIYO, bool previousIsIYO, bool currentIsIndicative, bool previousIsIndicative, bool IsReturnValue)
        {
            // Arrange
            var comparisonServices = this.CreateComparisonServices();
            string fundingStreamCode = "GAG";
            ChildComparisonRequest childComparisonRequest = new ChildComparisonRequest()
            {
                ChildUKPRN = "GAG-AC-2526-12345678-4_0",
                CurrentFundingStreamPeriodCode = "GAG-AC-2526",
                FundingStreamCode = "GAG",
                StatusChangedDateOnly = "1-1-2025",
                ParentUKPRN = null
            };
            LoggedInChildAzSearchModel currentChildFunding = new LoggedInChildAzSearchModel()
            {
                Id = "GAG-AC-2526-12345678-4_0",
                FundingStreamPeriod = "GAG-AC-2526",
                FundingVersionInt = 4,
                StatusChangedDate = new DateTime(2025, 1, 1),
                StatusChangedDateOnly = new DateTime(2025, 1, 1),
                InYearOpener = currentIsIYO,
                IsIndicative = currentIsIndicative,
            };

            LoggedInChildAzSearchModel previousChildFunding = new LoggedInChildAzSearchModel()
            {
                Id = "GAG-AC-2526-12345678-3_0",
                FundingStreamPeriod = "GAG-AC-2526",
                FundingVersionInt = 3,
                StatusChangedDate = new DateTime(2024, 12, 1),
                StatusChangedDateOnly = new DateTime(2024, 12, 1),
                InYearOpener = previousIsIYO,
                IsIndicative = previousIsIndicative,
            };

            IEnumerable<LoggedInChildAzSearchModel> childFundings = new List<LoggedInChildAzSearchModel>
            {
                currentChildFunding,
                previousChildFunding,
                new LoggedInChildAzSearchModel()
                {
                    Id = "GAG-AC-2425-12345678-8_0",
                    FundingStreamPeriod = "GAG-AC-2526",
                    FundingVersionInt = 3,
                    StatusChangedDate = new DateTime(2024, 12, 1),
                    InYearOpener = false,
                    IsIndicative = false,
                },
            };

            // Act
            var result = comparisonServices.GetPreviousStatementCurrentYear(
                fundingStreamCode,
                childComparisonRequest,
                currentChildFunding,
                childFundings);

            // Assert
            if (IsReturnValue)
            {
                result.Should().BeEquivalentTo(new ChildComparisonResponse
                {
                    ProviderFundingId = previousChildFunding.Id,
                    ComparisonType = ComparisonTypeEnum.PreviousStatementCurrentYear,
                    FundingStreamPeriodCode = previousChildFunding.FundingStreamPeriod,
                    StatusChangedDateOnly = previousChildFunding.StatusChangedDateOnly!.Value,
                });
            }
            else
            {
                result.Should().BeNull();
            }

            this.mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(true, true, true, true, true)]
        [DataRow(true, false, true, false, false)]
        [DataRow(false, true, false, true, false)]
        [DataRow(true, true, true, false, false)]
        public void GetFinalStatementPreviousYear_StateUnderTest_ExpectedBehavior(bool currentIsIYO, bool previousYearIsIYO, bool currentIsIndicative, bool previousYearIsIndicative, bool IsReturnValue)
        {
            // Arrange
            var comparisonServices = this.CreateComparisonServices();
            string fundingStreamCode = "GAG";
            ChildComparisonRequest childComparisonRequest = new ChildComparisonRequest()
            {
                ChildUKPRN = "GAG-AC-2526-12345678-4_0",
                CurrentFundingStreamPeriodCode = "GAG-AC-2526",
                FundingStreamCode = "GAG",
                StatusChangedDateOnly = "1-1-2025",
                ParentUKPRN = null
            };
            LoggedInChildAzSearchModel currentChildFunding = new LoggedInChildAzSearchModel()
            {
                Id = "GAG-AC-2526-12345678-4_0",
                FundingStreamPeriod = "GAG-AC-2526",
                FundingVersionInt = 4,
                StatusChangedDate = new DateTime(2025, 1, 1),
                StatusChangedDateOnly = new DateTime(2025, 1, 1),
                InYearOpener = currentIsIYO,
                IsIndicative = currentIsIndicative,
                StatementType = "New"
            };

            LoggedInChildAzSearchModel previousYearChildFunding = new LoggedInChildAzSearchModel()
            {
                Id = "GAG-AC-2425-12345678-8_0",
                FundingStreamPeriod = "GAG-AC-2425",
                FundingVersionInt = 8,
                StatusChangedDate = new DateTime(2024, 12, 1),
                StatusChangedDateOnly = new DateTime(2024, 12, 1),
                InYearOpener = previousYearIsIYO,
                IsIndicative = previousYearIsIndicative,
                IsLatest = true
            };

            IEnumerable<LoggedInChildAzSearchModel> childFundings = new List<LoggedInChildAzSearchModel>
            {
                currentChildFunding,
                previousYearChildFunding,
                new LoggedInChildAzSearchModel()
                {
                    Id = "GAG-AC-2425-12345678-7_0",
                    FundingStreamPeriod = "GAG-AC-2425",
                    FundingVersionInt = 7,
                    StatusChangedDate = new DateTime(2024, 12, 1),
                    StatusChangedDateOnly = new DateTime(2024, 12, 1),
                    InYearOpener = false,
                    IsIndicative = false
                },
            };

            // Act
            var result = comparisonServices.GetFinalStatementPreviousYear(
                fundingStreamCode,
                childComparisonRequest,
                currentChildFunding,
                childFundings);

            // Assert
            if (IsReturnValue)
            {
                result.Should().BeEquivalentTo(new ChildComparisonResponse
                {
                    ProviderFundingId = previousYearChildFunding.Id,
                    ComparisonType = ComparisonTypeEnum.FinalStatementPreviousYear,
                    FundingStreamPeriodCode = previousYearChildFunding.FundingStreamPeriod,
                    StatusChangedDateOnly = previousYearChildFunding.StatusChangedDateOnly!.Value,
                });
            }
            else
            {
                result.Should().BeNull();
            }

            this.mockRepository.VerifyAll();
        }
    }
}
