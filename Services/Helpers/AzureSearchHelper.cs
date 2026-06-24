using Azure.Search.Documents.Indexes;
using Newtonsoft.Json;
using PDS.ViewYourFunding.Data.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDS.ViewYourFunding.Data.Services.Helpers
{
    /// <summary>
    /// The Azure search helper.
    /// </summary>
    public static class AzureSearchHelper
    {
        /// <summary>
        /// Gets the alias name to use for the Azure Search index for the given document type.
        /// </summary>
        /// <typeparam name="T">The document type for which to get the alias name.</typeparam>
        /// <returns>The alias name to use for the Azure Search index for the given document type.</returns>
        public static string GetSearchIndexAliasNameForType<T>()
        {
            return GetSearchIndexAliasNameForType(typeof(T));
        }

        /// <summary>
        /// Get the date time component from an component (e.g. index or indexer) name.
        /// </summary>
        /// <param name="componentName">The name of the component (e.g. index or indexer).</param>
        /// <param name="componentNamePrefix">The prefix part of the component (e.g. index or indexer) name.</param>
        /// <returns>A date-time long in format 'yyyyMMddhhmmss'.</returns>
        public static long? GetDateTimeComponent(string componentName, string componentNamePrefix)
        {
            var hashAndAfter = componentName.Replace($"{componentNamePrefix}-", string.Empty);
            var parts = hashAndAfter.Split('-');

            return parts.Length > 0 && long.TryParse(parts[1], out var hash) ? hash : (long?)null;
        }

        /// <summary>
        /// Get the hash component from an component (e.g. index or indexer) name.
        /// </summary>
        /// <param name="componentName">The name of the component (e.g. index or indexer).</param>
        /// <param name="componentNamePrefix">The prefix part of the component (e.g. index or indexer) name.</param>
        /// <returns>A hash/thumprint as an int.</returns>
        public static int? GetHashComponent(string componentName, string componentNamePrefix)
        {
            var hashAndAfter = componentName.Replace($"{componentNamePrefix}-", string.Empty);
            var parts = hashAndAfter.Split('-');

            return int.TryParse(parts[0], out var hash) ? hash : (int?)null;
        }

        /// <summary>
        /// Gets the alias name to use for the Azure Search index for the given document type.
        /// </summary>
        /// <param name="type">The document type for which to get the alias name.</param>
        /// <returns>The alias name to use for the Azure Search index for the given document type.</returns>
        public static string GetSearchIndexAliasNameForType(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(AzureSearchIndexAttribute), false)
                .FirstOrDefault() as AzureSearchIndexAttribute;

            if (attribute?.AliasName != null)
            {
                return attribute.AliasName.ToLower();
            }

            return type.Name.ToLower();
        }

        /// <summary>
        /// Get a thumbprint/hash for the class.
        /// </summary>
        /// <param name="additionalInputs">Other strings to serialise into the hash.</param>
        /// <typeparam name="T">The type of class to get the thumbprint/hash for.</typeparam>
        /// <returns>A deterministic thumbprint/hash as an int.</returns>
        public static int GetClassThumbprint<T>(IList<string> additionalInputs)
            where T : class
        {
            var sb = new StringBuilder();

            foreach (var property in typeof(T).GetProperties().OrderBy(prop => prop.Name))
            {
                var attributes = property.GetCustomAttributes(false);
                var attributeList = new List<string>();

                foreach (var attribute in attributes)
                {
                    if (attribute is JsonPropertyAttribute)
                    {
                        var jsonAttribute = attribute as JsonPropertyAttribute;
                        attributeList.Add($"{nameof(JsonPropertyAttribute)}_{jsonAttribute.PropertyName}");
                    }
                    else if (attribute is SearchableFieldAttribute)
                    {
                        var searchableAttribute = attribute as SearchableFieldAttribute;
                        attributeList.Add($"{nameof(SearchableFieldAttribute)}_{searchableAttribute.IndexAnalyzerName}_{searchableAttribute.SearchAnalyzerName}");
                    }
                    else if (attribute is SimpleFieldAttribute)
                    {
                        var simpleFieldAttribute = attribute as SimpleFieldAttribute;
                        attributeList.Add($"{nameof(SimpleFieldAttribute)}_{simpleFieldAttribute.IsFilterable}_{simpleFieldAttribute.IsSortable}");
                    }
                    else
                    {
                        attributeList.Add(attribute.ToString());
                    }
                }

                sb.Append($"{property.PropertyType.FullName}_{property.Name}_{string.Join(",", attributeList.OrderBy(attribute => attribute))}");
            }

            foreach (var otherProperty in additionalInputs)
            {
                sb.Append(otherProperty);
            }

            return GetDeterministicHashCode(sb.ToString());
        }

        /// <summary>
        /// Gets the cosmos list expression.
        /// </summary>
        /// <param name="commaSeparatedList">The comma separated list.</param>
        /// <param name="joinKeyWord">The key word join.</param>
        /// <param name="operatorKeyWord">The operator key word.</param>
        /// <returns>A collection expression.</returns>
        public static string GetCosmosListExpression(this string commaSeparatedList, string joinKeyWord, string operatorKeyWord)
        {
            var listExpression = string.Empty;
            var firstKeyWordJoin = true;

            var list = commaSeparatedList.Split(',');

            if (list.Any())
            {
                list.ToList().ForEach(item =>
                {
                    var join = firstKeyWordJoin ? string.Empty : $" {joinKeyWord.Trim()} ";
                    listExpression += $"{join}r {operatorKeyWord} '{item.Trim()}'";
                    firstKeyWordJoin = false;
                });
            }

            return listExpression;
        }


        /// <summary>
        /// Get a deterministic hash / thumbprint for a string.
        /// https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/.
        /// </summary>
        /// <param name="stringToHash">The string we want to hash.</param>
        /// <returns>A thumbprint hash.</returns>
        private static int GetDeterministicHashCode(string stringToHash)
        {
            unchecked
            {
                var hash1 = (5381 << 16) + 5381;
                var hash2 = hash1;

                for (var i = 0; i < stringToHash.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ stringToHash[i];

                    if (i == stringToHash.Length - 1)
                    {
                        break;
                    }

                    hash2 = ((hash2 << 5) + hash2) ^ stringToHash[i + 1];
                }

                return Math.Abs(hash1 + (hash2 * 1566083941));
            }
        }
    }
}
