namespace PDS.VYF.Data.Services.Extensions
{
    using PDS.VYF.Data.Services.Enums;

    /// <summary>
    /// The class which holds all the extension methods for AzSearchIndexTypeEnum Enum.
    /// </summary>
    public static class AzSearchIndexTypeExtension
    {
        /// <summary>
        /// Gets or sets the name of the base.
        /// </summary>
        /// <value>
        /// The name of the base.
        /// </value>
        public static string AzResourceBaseName { get; set; } = "vyf-fundingapi";

        /// <summary>
        /// Gets the name of the index.
        /// </summary>
        /// <param name="azSearchIndexTypeEnum">The az search index type enum.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>The Index name.</returns>
        public static string GetIndexName(this AzSearchIndexTypeEnum azSearchIndexTypeEnum, string containerName)
            => $"index-{GetIndexBaseName(azSearchIndexTypeEnum, containerName)}";

        /// <summary>
        /// Gets the name of the indexer.
        /// </summary>
        /// <param name="azSearchIndexTypeEnum">The az search index type enum.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>The Indexer name.</returns>
        public static string GetIndexerName(this AzSearchIndexTypeEnum azSearchIndexTypeEnum, string containerName)
            => $"indexer-{GetIndexBaseName(azSearchIndexTypeEnum, containerName)}";

        /// <summary>
        /// Gets the name of the datasource.
        /// </summary>
        /// <param name="azSearchIndexTypeEnum">The az search index type enum.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>The datasource Name.</returns>
        public static string GetDatasourceName(this AzSearchIndexTypeEnum azSearchIndexTypeEnum, string containerName)
            => $"datasource-{GetIndexBaseName(azSearchIndexTypeEnum, containerName)}";

        private static string GetIndexBaseName(AzSearchIndexTypeEnum azSearchIndexTypeEnum, string containerName)
            => $"{AzResourceBaseName}-{azSearchIndexTypeEnum}-{containerName}".FormatName();

        private static string FormatName(this string name) => name.ToLowerInvariant().Replace("_", "-");
    }
}
