using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.Data.Services.Helpers;

namespace PDS.ViewYourFunding.Data.Tests.Unit.Helpers
{
    /// <summary>
    /// The String Extensions Tests.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class AzureSearchHelperTests
    {
        [DataRow("fundingUpdated, profilingUpdated", "or", "eq", "r eq 'fundingUpdated' or r eq 'profilingUpdated'")]
        [DataRow("fundingUpdated- profilingUpdated", "or", "eq", "r eq 'fundingUpdated- profilingUpdated'")]
        [DataRow("fundingUpdated, profilingUpdated", "and", "eq", "r eq 'fundingUpdated' and r eq 'profilingUpdated'")]
        [DataRow("fundingUpdated, profilingUpdated", "and", "ne", "r ne 'fundingUpdated' and r ne 'profilingUpdated'")]
        [TestMethod]
        public void GetCosmosListExpression_ExpectedResult(string commaSeparatedList, string joinKeyWord, string operatorKeyWord, string expectedResult)
        {
            // Act
            var result = commaSeparatedList.GetCosmosListExpression(joinKeyWord, operatorKeyWord);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
