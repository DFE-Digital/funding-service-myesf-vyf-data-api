namespace PDS.ViewYourFunding.Data.Interfaces.DTOs
{
    /// <summary>
    /// Class representing the parameters required to configure a Cosmos DB data source.
    /// </summary>
    public class CosmosDbDataSourceParameters
    {
        /// <summary>
        /// Gets or sets the connection string for the Cosmos DB service. Must include the database name and account key.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the collection to query.
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Gets or sets the query to run.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the hash code for the class.
        /// </summary>
        public int HashCode { get; set; }
    }
}
