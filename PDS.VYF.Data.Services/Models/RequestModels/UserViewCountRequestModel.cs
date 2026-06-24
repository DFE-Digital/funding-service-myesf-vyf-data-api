namespace PDS.VYF.Data.Services.Models.RequestModels
{
    /// <summary>
    /// The request model for the user view count.
    /// </summary>
    public class UserViewCountRequestModel
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the child statements.
        /// </summary>
        /// <value>
        /// The child statements.
        /// </value>
        public List<ChildStatementModel>? ChildStatements { get; set; }
    }
}
