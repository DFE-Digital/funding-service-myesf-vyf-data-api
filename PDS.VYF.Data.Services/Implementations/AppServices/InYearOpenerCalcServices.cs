namespace PDS.VYF.Data.Services.Implementations.AppServices
{
    using PDS.VYF.Data.Services.Abstracts.AppServices;
    using PDS.VYF.Data.Services.Models.AzSearchModels;

    /// <summary>
    /// The Service class for In Year Opener Calculator.
    /// </summary>
    /// <seealso cref="PDS.VYF.Data.Services.Abstracts.AppServices.IInYearOpenerCalcServices" />
    public class InYearOpenerCalcServices : IInYearOpenerCalcServices
    {
        private const int DaysInFullYear = 733;
        private const int DaysOpenInYear = 567;

        /// <summary>
        /// Determines whether [is in year opener]. Required fields from LoggedInChildAzSearchModel are below,
        ///  - OpenReason
        ///  - Calculations / CalculationsForSummary.
        ///  - FundingPeriodCode
        ///  - DateOpened.
        ///  - CloseReason.
        ///  - DateClosed.
        /// </summary>
        /// <param name="loggedInChildAzSearchModel">The logged in child az search model.</param>
        /// <returns>
        ///   <c>true</c> if [is in year opener] [the specified logged in child az search model]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInYearOpener(LoggedInChildAzSearchModel loggedInChildAzSearchModel)
        {
            if (loggedInChildAzSearchModel.IsIndicative == true)
            {
                return true;
            }
            else
            {
                var (yearFrom, yearTo) = (loggedInChildAzSearchModel.YearFrom, loggedInChildAzSearchModel.YearTo);
                var dateOpened = loggedInChildAzSearchModel.DateOpened;

                // isOpenDaysEqualToFullYearDays Calculations
                var fullYearDays = this.GetCalcValue(loggedInChildAzSearchModel, DaysInFullYear);
                var daysOpen = this.GetCalcValue(loggedInChildAzSearchModel, DaysOpenInYear);

                var isOpenDaysNotEqualToFullYearDays = fullYearDays != daysOpen;

                // isPreviousYearPostAprilOpener calculations
                var previousYearPostAprilStartDate = new DateTime(yearFrom!.Value, 4, 1);
                var previousYearPostAprilEndDate = new DateTime(yearFrom!.Value, 8, 31);

                var isPreviousYearPostAprilOpener = dateOpened >= previousYearPostAprilStartDate && dateOpened <= previousYearPostAprilEndDate;

                // isAcademicYearInYearOpener Calculations
                var iyoStartDate = new DateTime(yearFrom!.Value, 8, 31);
                var iyoEndDate = new DateTime(yearTo!.Value, 7, 31);

                var isAcademicYearInYearOpener = dateOpened >= iyoStartDate && dateOpened <= iyoEndDate;

                // isReBrokerageOpeningReason Calculations
                var isReBrokerageOpeningReason = loggedInChildAzSearchModel?.OpenReason?.Equals("Fresh Start", StringComparison.OrdinalIgnoreCase) == true;

                // isReBrokerageCloseReason Calculations
                var isReBrokerageCloseReason = loggedInChildAzSearchModel?.CloseReason?.Equals("Fresh Start", StringComparison.OrdinalIgnoreCase) == true
                                                && loggedInChildAzSearchModel?.DateClosed != DateTime.MinValue;

                return (isOpenDaysNotEqualToFullYearDays || isPreviousYearPostAprilOpener || isAcademicYearInYearOpener)
                    && !(isReBrokerageOpeningReason || isReBrokerageCloseReason);
            }
        }

        /// <summary>
        /// Gets the calculate value.
        /// </summary>
        /// <param name="loggedInChildAzSearchModel">The logged in child az search model.</param>
        /// <param name="calcId">The calculate identifier.</param>
        /// <returns>The value.</returns>
        internal int GetCalcValue(LoggedInChildAzSearchModel loggedInChildAzSearchModel, int calcId)
        {
            var fundingLines = loggedInChildAzSearchModel.Calculations ?? loggedInChildAzSearchModel.CalculationsForSummary;

            var value = fundingLines?.FirstOrDefault(a => a.TemplateCalculationId == calcId)?.Value ?? "0";

            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }

            return 0;
        }
    }
}
