namespace PDS.VYF.Data.Services.Models.ResponseModels
{
    /// <summary>
    /// The response model for the user view count.
    /// </summary>
    public class UserViewCountResponse
    {
        /// <summary>
        /// Gets or sets the new count.
        /// </summary>
        /// <value>
        /// The new count.
        /// </value>
        public int NewCount { get; set; }

        /// <summary>
        /// Gets or sets the updated count.
        /// </summary>
        /// <value>
        /// The updated count.
        /// </value>
        public int UpdatedCount { get; set; }
    }
}
