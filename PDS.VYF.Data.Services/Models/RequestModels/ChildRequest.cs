namespace PDS.VYF.Data.Services.Models.RequestModels
{
    using PDS.VYF.Data.Services.Models.AzSearchModels;

    /// <summary>
    /// The class for ChildRequest.
    /// </summary>
    /// <seealso cref="PDS.VYF.Data.Services.Models.RequestModels.RequestBase&lt;PDS.VYF.Data.Services.Models.AzSearchModels.LoggedInChildAzSearchModel&gt;" />
    public class ChildRequest : RequestBase<LoggedInChildAzSearchModel>
    {
        /// <summary>
        /// Gets or sets the status changed date only.
        /// </summary>
        /// <value>
        /// The status changed date only.
        /// </value>
        public DateTime? StatusChangedDateOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has iyo to be removed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has iyo to be removed; otherwise, <c>false</c>.
        /// </value>
        public bool HasIYOToBeRemoved { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether [do find is latest].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [do find is latest]; otherwise, <c>false</c>.
        /// </value>
        public bool DoFindIsLatest { get; set; } = false;
    }
}
