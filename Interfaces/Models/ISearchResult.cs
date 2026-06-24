using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.Interfaces.Models
{
    /// <summary>
    /// An interface representing the result of a search.
    /// </summary>
    /// <typeparam name="T">The type of the search result documents.</typeparam>
    public interface ISearchResult<T>
    {
        /// <summary>
        /// Gets or sets the collection of matching documents.
        /// </summary>
        IEnumerable<T> Documents { get; set; }
    }
}
