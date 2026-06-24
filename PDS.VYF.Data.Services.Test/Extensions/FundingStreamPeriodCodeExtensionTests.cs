using FluentAssertions;
using PDS.VYF.Data.Services.Extensions;

namespace PDS.VYF.Data.Services.Tests.Extensions
{
    /// <summary>
    /// The Funding Stream Period Code Extension Tests.
    /// </summary>
    [TestClass]
    [TestCategory("Unit")]
    public class FundingStreamPeriodCodeExtensionTests
    {
        /// <summary>
        /// Gets the year from year to should return correct years when correct detailed provided.
        /// </summary>
        /// <param name="fundingStreamPeriodCode">The funding stream period code.</param>
        /// <param name="expectedYearFrom">The expected year from.</param>
        /// <param name="expectedYearTo">The expected year to.</param>
        [TestMethod]
        [DataRow("GAG-AC-2021", 2020, 2021)]
        [DataRow("1619-AY-2425", 2024, 2025)]
        [DataRow("LAREC-AS-2627", 2026, 2027)]
        public void GetYearFromYearTo_ShouldReturnCorrectYears_WhenCorrectDetailedProvided(string fundingStreamPeriodCode, int expectedYearFrom, int expectedYearTo)
        {
            // Act
            var result = fundingStreamPeriodCode.GetYearFromYearTo();

            // Assert
            result.Should().Be((expectedYearFrom, expectedYearTo));
        }

        /// <summary>
        /// Gets the previous funding stream period code should return correct previous period code when correct detailed provided.
        /// </summary>
        /// <param name="fundingStreamPeriodCode">The funding stream period code.</param>
        /// <param name="expectedPreviousPeriodCode">The expected previous period code.</param>
        [TestMethod]
        [DataRow("GAG-AC-2021", "GAG-AC-1920")]
        [DataRow("1619-AY-2425", "1619-AY-2324")]
        [DataRow("LAREC-AS-2627", "LAREC-AS-2526")]
        public void GetPreviousFundingStreamPeriodCode_ShouldReturnCorrectPreviousPeriodCode_WhenCorrectDetailedProvided(string fundingStreamPeriodCode, string expectedPreviousPeriodCode)
        {
            // Act
            var result = fundingStreamPeriodCode.GetPreviousFundingStreamPeriodCode();

            // Assert
            result.Should().Be(expectedPreviousPeriodCode);
        }

        /// <summary>
        /// Gets the maximum funding stream period code should return correct maximum funding stream period code when correct detailed provided.
        /// </summary>
        [TestMethod]
        public void GetMaxFundingStreamPeriodCode_ShouldReturnCorrectMaxFundingStreamPeriodCode_WhenCorrectDetailedProvided()
        {
            // Arrange
            var fundingStreamPeriodCodes = new List<string> { "GAG-AC-2021", "1619-AY-2425", "LAREC-AS-2627", "LAREC-AS-2425" };

            // Act
            var result = fundingStreamPeriodCodes.GetMaxFundingStreamPeriodCode();

            // Assert
            result.Should().Be("LAREC-AS-2627");
        }

        /// <summary>
        /// Determines whether [is latest funding stream period code should return true when correct detailed provided].
        /// </summary>
        public void IsLatestFundingStreamPeriodCode_ShouldReturnTrue_WhenCorrectDetailedProvided()
        {
            // Arrange
            var fundingStreamPeriodCodes = new List<string> { "GAG-AC-2021", "1619-AY-2425", "LAREC-AS-2627", "LAREC-AS-2425" };
            var fundingStreamPeriodCode = "LAREC-AS-2627";

            // Act
            var result = fundingStreamPeriodCode.IsLatestFundingStreamPeriodCode(fundingStreamPeriodCodes);

            // Assert
            result.Should().BeTrue();
        }
    }
}
