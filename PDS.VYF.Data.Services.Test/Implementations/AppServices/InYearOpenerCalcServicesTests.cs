using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.VYF.Data.Services.Implementations.AppServices;
using PDS.VYF.Data.Services.Models.AzSearchModels;
using System;

namespace PDS.VYF.Data.Services.Tests.Implementations.AppServices
{
    /// <summary>
    /// The Test classes for InYearOpenerCalcServices.
    /// </summary>
    [TestClass]
    public class InYearOpenerCalcServicesTests
    {
        private const int DaysInFullYear = 733;
        private const int DaysOpenInYear = 567;

        private readonly InYearOpenerCalcServices inYearOpenerCalcServices = new InYearOpenerCalcServices();

        /// <summary>
        /// Initializes a new instance of the <see cref="InYearOpenerCalcServicesTests"/> class.
        /// </summary>
        public InYearOpenerCalcServicesTests()
        {
        }

        /// <summary>
        /// Determines whether [is in year opener should return false when is re brokerage opening reason is true].
        /// </summary>
        [TestMethod]
        public void IsInYearOpener_ShouldReturnFalse_WhenIsReBrokerageOpeningReasonIsTrue()
        {
            // Arrange
            var model = new LoggedInChildAzSearchModel
            {
                IsIndicative = false,
                YearFrom = 2023,
                YearTo = 2024,
                DateOpened = new DateTime(2023, 9, 1),
                OpenReason = "Fresh Start",
                Calculations = new List<LoggedInCalculation> { new () { TemplateCalculationId = DaysInFullYear, Value = "365" }, new () { TemplateCalculationId = DaysOpenInYear, Value = "365" } }
            };

            // Act
            var result = inYearOpenerCalcServices.IsInYearOpener(model);

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>
        /// Determines whether [is in year opener should return true when is open days not equal to full year days].
        /// </summary>
        [TestMethod]
        public void IsInYearOpener_ShouldReturnTrue_WhenIsOpenDaysNotEqualToFullYearDays()
        {
            // Arrange
            var model = new LoggedInChildAzSearchModel
            {
                IsIndicative = false,
                YearFrom = 2023,
                YearTo = 2024,
                DateOpened = new DateTime(2023, 9, 1),
                Calculations = new List<LoggedInCalculation> { new () { TemplateCalculationId = DaysInFullYear, Value = "365" }, new () { TemplateCalculationId = DaysOpenInYear, Value = "364" } }
            };

            // Act
            var result = inYearOpenerCalcServices.IsInYearOpener(model);

            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// Determines whether [is in year opener should return true when is previous year post april opener].
        /// </summary>
        [TestMethod]
        public void IsInYearOpener_ShouldReturnTrue_WhenIsPreviousYearPostAprilOpener()
        {
            // Arrange
            var model = new LoggedInChildAzSearchModel
            {
                IsIndicative = false,
                YearFrom = 2023,
                YearTo = 2024,
                DateOpened = new DateTime(2023, 5, 1),
                Calculations = new List<LoggedInCalculation> { new () { TemplateCalculationId = DaysInFullYear, Value = "365" }, new () { TemplateCalculationId = DaysOpenInYear, Value = "365" } }
            };

            // Act
            var result = inYearOpenerCalcServices.IsInYearOpener(model);

            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// Determines whether [is in year opener should return true when is academic year in year opener].
        /// </summary>
        [TestMethod]
        public void IsInYearOpener_ShouldReturnTrue_WhenIsAcademicYearInYearOpener()
        {
            // Arrange
            var model = new LoggedInChildAzSearchModel
            {
                IsIndicative = false,
                YearFrom = 2023,
                YearTo = 2024,
                DateOpened = new DateTime(2023, 9, 1),
                Calculations = new List<LoggedInCalculation> { new () { TemplateCalculationId = DaysInFullYear, Value = "365" }, new () { TemplateCalculationId = DaysOpenInYear, Value = "365" } }
            };

            // Act
            var result = inYearOpenerCalcServices.IsInYearOpener(model);

            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// Gets the calculate value should return correct value when calculation exists.
        /// </summary>
        [TestMethod]
        public void GetCalcValue_ShouldReturnCorrectValue_WhenCalculationExists()
        {
            // Arrange
            var model = new LoggedInChildAzSearchModel
            {
                Calculations = new List<LoggedInCalculation> { new () { TemplateCalculationId = DaysInFullYear, Value = "365" } }
            };

            // Act
            var result = inYearOpenerCalcServices.GetCalcValue(model, DaysInFullYear);

            // Assert
            result.Should().Be(365);
        }

        /// <summary>
        /// Gets the calculate value should return zero when calculation does not exist.
        /// </summary>
        [TestMethod]
        public void GetCalcValue_ShouldReturnZero_WhenCalculationDoesNotExist()
        {
            // Arrange
            var model = new LoggedInChildAzSearchModel
            {
                Calculations = new List<LoggedInCalculation> { new () { TemplateCalculationId = DaysInFullYear, Value = "365" } }
            };

            // Act
            var result = inYearOpenerCalcServices.GetCalcValue(model, DaysOpenInYear);

            // Assert
            result.Should().Be(0);
        }
    }
}
