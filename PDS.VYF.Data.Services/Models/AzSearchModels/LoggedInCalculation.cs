namespace PDS.VYF.Data.Services.Models.AzSearchModels
{
    using Azure.Search.Documents.Indexes;

    /// <summary>
    /// The LoggedInCalculation.class.
    /// </summary>
    public class LoggedInCalculation
    {
        /// <summary>
        /// Gets or sets the template calculation identifier.
        /// </summary>
        /// <value>
        /// The template calculation identifier.
        /// </value>
        [SimpleField]
        public int TemplateCalculationId { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [SimpleField]
        public string? Value { get; set; }
    }
}
