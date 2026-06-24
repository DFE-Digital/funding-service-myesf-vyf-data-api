namespace PDS.ViewYourFunding.Data.Core
{
    /// <summary>
    /// The LogLevel class.
    /// </summary>
    public class LogLevel
    {
        /// <summary>
        /// Gets or sets the default log level.
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// Gets or sets the Microsoft default log level.
        /// </summary>
        public string Microsoft { get; set; }

        /// <summary>
        /// Gets or sets the microsoft hosting lifetime.
        /// </summary>
        /// <value>
        /// The microsoft hosting lifetime.
        /// </value>
        public string MicrosoftHostingLifetime { get; set; }
    }
}
