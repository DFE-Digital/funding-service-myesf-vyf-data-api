using System.Globalization;

namespace PDS.VYF.Data.Services.Constants
{
    /// <summary>
    /// Date/time constants.
    /// </summary>
    public static class DateConstants
    {
        /// <summary>
        /// The date format that is used in settings.
        /// </summary>
        public const string SettingDateFormat = "dd-M-yyyy";

        /// <summary>
        /// The cut off date format to use.
        /// </summary>
        public static string CutOffDateTimeFormat = "yyyy-MM-dd hh:mm:ss";

        /// <summary>
        /// The en-GB culture name.
        /// </summary>
        public const string EnGbCultureName = "en-GB";

        /// <summary>
        /// The en-GB culture information.
        /// </summary>
        public static CultureInfo EnGbCultureInfo = new CultureInfo(EnGbCultureName);
    }
}