using PDS.ViewYourFunding.Data.Interfaces.Models;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.Services.Models
{
    /// <summary>
    /// A class representing the results of an Azure Search.
    /// </summary>
    /// <typeparam name="T">The document type.</typeparam>
    /// <seealso cref="Interfaces.Models.ISearchResult{T}" />
    public class AzureSearchResult<T> : ISearchResult<T>
    {
        /// <summary>
        /// Gets or sets the collection of matching documents.
        /// </summary>
        public IEnumerable<T> Documents { get; set; }
    }
}
