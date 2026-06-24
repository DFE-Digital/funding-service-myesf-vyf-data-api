using System;

namespace PDS.ViewYourFunding.Data.Services.Models
{
    /// <summary>
    /// Class representing the result of an Azure Search index (re)build.
    /// </summary>
    public class AzureSearchIndexBuildResult
    {
        /// <summary>
        /// Gets or sets the name of the index that was built.
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// Gets or sets the name of the indexer.
        /// </summary>
        public string IndexerName { get; set; }

        /// <summary>
        /// Gets or sets the name of the data source.
        /// </summary>
        public string DataSourceName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether whether or not the indexer ran successfully.
        /// </summary>
        public bool IndexerSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error that occurred, if applicable.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether whether or not there was an error.
        /// </summary>
        public bool HasError { get; set; }
    }
}
