namespace PDS.ViewYourFunding.Data.Core
{
    /// <summary>
    /// The Logging Class.
    /// </summary>
    public class LoggingConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the application insights role.
        /// </summary>
        /// <value>
        /// The name of the application insights role.
        /// </value>
        public string ApplicationInsightsRoleName { get; set; }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>
        /// The log level.
        /// </value>
        public LogLevel LogLevel { get; set; } = new LogLevel();
    }
}
