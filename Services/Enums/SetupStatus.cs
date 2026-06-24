namespace PDS.ViewYourFunding.Data.Services.Enums
{
    /// <summary>
    /// The status of the index/indexer/data source's setup.
    /// </summary>
    internal enum SetupStatus
    {
        /// <summary>
        /// Index is not yet created or at least state is not yet known.
        /// </summary>
        NotSetup = 1,

        /// <summary>
        /// Index is currently being created.
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// Index is setup and ready.
        /// </summary>
        Setup = 3
    }
}
