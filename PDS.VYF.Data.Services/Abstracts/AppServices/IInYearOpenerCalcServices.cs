namespace PDS.VYF.Data.Services.Abstracts.AppServices
{
    using PDS.VYF.Data.Services.Models.AzSearchModels;

    /// <summary>
    /// The service class for In-Year Opener Calculator.
    /// </summary>
    public interface IInYearOpenerCalcServices
    {
        /// <summary>
        /// Determines whether [is in year opener] [the specified logged in child az search model].
        /// </summary>
        /// <param name="loggedInChildAzSearchModel">The logged in child az search model.</param>
        /// <returns>
        ///   <c>true</c> if [is in year opener] [the specified logged in child az search model]; otherwise, <c>false</c>.
        /// </returns>
        bool IsInYearOpener(LoggedInChildAzSearchModel loggedInChildAzSearchModel);
    }
}
