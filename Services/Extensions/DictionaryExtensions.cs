using System.Collections.Generic;
using System.Linq;

namespace PDS.ViewYourFunding.Data.Services.Extensions
{
    /// <summary>
    /// The extension for disctionaries.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Merges two dictionaries.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionaries">The dictionaries to be merged.</param>
        /// <returns>The merged dictionaries.</returns>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            return dictionaries.SelectMany(dict => dict)
                         .GroupBy(d => d.Key)
                         .ToDictionary(group => group.Key, group => group.First().Value);
        }
    }
}
