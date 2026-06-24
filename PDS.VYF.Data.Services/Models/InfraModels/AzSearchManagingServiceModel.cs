namespace PDS.VYF.Data.Services.Models.InfraModels
{
    using PDS.VYF.Data.Services.Enums;

    /// <summary>
    /// The model for Azure Search Managing Service.
    /// </summary>
    public class AzSearchManagingServiceModel
    {
        /// <summary>
        /// Gets or sets the type of the az search index.
        /// </summary>
        /// <value>
        /// The type of the az search index.
        /// </value>
        public AzSearchIndexTypeEnum AzSearchIndexType { get; set; }

        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        public string IndexName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the indexer.
        /// </summary>
        public string IndexerName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the datasource.
        /// </summary>
        public string DatasourceName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        public Type TypeOfField { get; set; } = default!;

        /// <summary>
        /// Gets or sets the types for thumbprint.
        /// </summary>
        public Type[] TypesForThumbPrint { get; set; } = default!;

        /// <summary>
        /// Gets or sets the thumbprint.
        /// </summary>
        public int ThumbPrint { get; set; }

        /// <summary>
        /// Gets the description with thumb print.
        /// </summary>
        public string DescriptionWithThumbPrint
            => @$"
* Important: The description is used by VYF DataApi to recreate search resources!! Don't delete or change any of the description contents including this line!

Thumb Print             - {this.ThumbPrint}
Az Search Index Type    - {this.AzSearchIndexType}
Cosmos Container Name   - {this.CosmosContainerName}
Length of Cosmos Query  - {this.CosmosQuery.Length}
";

        /// <summary>
        /// Gets the description with thumb print and create date time.
        /// </summary>
        public string DescriptionWithThumbPrintAndCreateDateTime
            => this.DescriptionWithThumbPrint + $"Created at              - {DateTime.UtcNow} UTC (GMT)";

        /// <summary>
        /// Gets or sets the name of the cosmos container.
        /// </summary>
        public string CosmosContainerName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the cosmos query.
        /// </summary>
        /// <value>
        /// The cosmos query.
        /// </value>
        public string CosmosQuery { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether this instance has to be deleted and recreated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has to be deleted and recreated; otherwise, <c>false</c>.
        /// </value>
        public bool HasToBeReplaced { get; set; }
    }
}
