using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.Data.Services.Extensions;

namespace PDS.ViewYourFunding.Data.Tests.Unit.Helpers
{
    /// <summary>
    /// The String Extensions Tests.
    /// </summary>
    [TestClass]
    public class StringExtensionsTests
    {
        /// <summary>
        /// The string helper method to check if the funding is first version.
        /// </summary>
        /// <param name="fundingId">The funding id to check.</param>
        /// <param name="expectedResult">The expected result.</param>
        [DataRow("GAG_AY2021_12345678_1_0", true)]
        [DataRow("GAG_AY2021_12345678_2_0", false)]
        [DataRow("", false)]
        [TestMethod, TestCategory("Unit")]
        public void IsFirstVersionOfFunding_ChecksVersion(string fundingId, bool expectedResult)
        {
            // Act
            var result = fundingId.IsFirstVersionOfFunding();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
