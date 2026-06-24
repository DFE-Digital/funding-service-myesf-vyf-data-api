using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using PDS.ViewYourFunding.Data.Services.Helpers;

namespace PDS.ViewYourFunding.Data.Tests.Unit.Helpers
{
    /// <summary>
    /// The Funding Version Detail Extension Tests.
    /// </summary>
    [TestClass]
    public class StatementChannelVersionHelperTests
    {
        /// <summary>
        /// The string helper method to check if the funding is first version.
        /// </summary>
        /// <param name="fundingID">The funding Id to check.</param>
        /// <param name="statementChannelVersion">The statement channel version of funding ID to check.</param>
        /// <param name="expectedResult">The expected result.</param>
        [DataRow("GAG-AY2021-12345678-2_0", 1, true)]
        [DataRow("GAG-AY2021-12345678-3_0", 2, false)]
        [DataRow("GAG-AY2021-12345678-1_0", 0, true)]
        [DataRow("GAG-AY2021-12345678-3_0", 0, false)]
        [DataRow("GAG-AY2021-12345678-1_0", null, true)]
        [DataRow("GAG-AY2021-12345678-3_0", null, false)]
        [DataRow("GAG-AY2021-12345678-11_0", null, false)]
        [DataRow("GAG-AY2021-12345678-1_0", -1, true)]
        [DataRow("GAG-AY2021-12345678-3_0", -1, false)]
        [DataRow("", 0, false)]
        [DataRow("", null, false)]
        [DataRow("", -1, false)]
        [DataRow("", 1, true)]
        [TestMethod, TestCategory("Unit")]
        public void IsFirstVersionOfFunding_ChecksVersion(string fundingID, int? statementChannelVersion, bool expectedResult)
        {
            // Act
            var result = StatementChannelVersionHelper.IsFirstVersionOfFunding(fundingID, statementChannelVersion);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
