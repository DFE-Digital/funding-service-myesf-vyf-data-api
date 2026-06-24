using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.VYF.Data.Services.Implementations.InfraServices;
using PDS.VYF.Data.Services.Models.AzSearchModels;
using System;

namespace PDS.VYF.Data.Services.Tests.Implementations.InfraServices
{
    /// <summary>
    /// The Test classes for AzSearchThumbPrintServices.
    /// </summary>
    [TestClass]
    public class AzSearchThumbPrintServicesTests
    {
        /// <summary>
        /// Azs the index thumb print logged in child models with sample input success.
        /// </summary>
        [TestMethod]
        public void AzIndexThumbPrint_LoggedInChildModelsWithSampleInput_Success()
        {
            // Arrange
            var azSearchThumbPrintServices = this.CreateAzSearchThumbPrintServices();
            List<string> keyParams = new () { "<<sample connection string>>", "Sample Cosmos Container Name", "Sample Cosmos SQL query" };
            Type[] types = new Type[] { typeof(LoggedInChildAzSearchModel), typeof(LoggedInTemplateLine), typeof(LoggedInCalculation), typeof(LoggedInDistributionPeriod), typeof(LoggedInParentInfoModel) };
            var expectedValue = 654109341;

            // Act
            var result = azSearchThumbPrintServices.AzIndexThumbPrint(keyParams, types);

            // Assert
            result.Should().Be(expectedValue);
        }

        /// <summary>
        /// Azs the index thumb print logged in parent models with sample input success.
        /// Any changes in the Models (specified in the Types[] array)  will cause this test to fail, validate the changes in models are as expected and use the latest ThumbPrint for test not to be failed.
        /// </summary>
        [TestMethod]
        public void AzIndexThumbPrint_LoggedInParentModelsWithSampleInput_Success()
        {
            // Arrange
            var azSearchThumbPrintServices = this.CreateAzSearchThumbPrintServices();
            List<string> keyParams = new List<string>() { "<<sample connection string>>", "Sample Cosmos Container Name", "Sample Cosmos SQL query" };
            Type[] types = { typeof(LoggedInParentAzSearchModel), typeof(LoggedInTemplateLine), typeof(LoggedInCalculation), typeof(LoggedInDistributionPeriod) };
            var expectedValue = 534930097;

            // Act
            var result = azSearchThumbPrintServices.AzIndexThumbPrint(keyParams, types);

            // Assert
            result.Should().Be(expectedValue);
        }

        /// <summary>
        /// Creates the az search thumb print services.
        /// </summary>
        /// <returns>The instance of AzSearchThumbPrintServices.</returns>
        private AzSearchThumbPrintServices CreateAzSearchThumbPrintServices()
        {
            return new AzSearchThumbPrintServices();
        }
    }
}
