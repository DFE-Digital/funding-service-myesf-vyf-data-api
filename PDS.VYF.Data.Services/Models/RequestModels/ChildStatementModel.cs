namespace PDS.VYF.Data.Services.Models.RequestModels
{
    /// <summary>
    /// The class for ChildStatementModel.
    /// </summary>
    public class ChildStatementModel
    {
        /// <summary>
        /// Gets or sets the child identifier.
        /// </summary>
        /// <value>
        /// The child identifier.
        /// </value>
        public string ChildId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the type of the statement.
        /// </summary>
        /// <value>
        /// The type of the statement.
        /// </value>
        public string StatementType { get; set; } = default!;
    }
}
