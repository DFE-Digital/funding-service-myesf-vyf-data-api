namespace PDS.VYF.Data.Services.Implementations.InfraServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Azure.Search.Documents.Indexes;
    using PDS.VYF.Data.Services.Abstracts.InfraServices;

    /// <summary>
    /// The Azure search thumb print services.
    /// </summary>
    public class AzSearchThumbPrintServices : IAzSearchThumbPrintServices
    {
        /// <summary>
        /// Azs the index thumb print.
        /// </summary>
        /// <param name="keyParams">The key parameters.</param>
        /// <param name="types">The types.</param>
        /// <returns>The Thumb print.</returns>
        public int AzIndexThumbPrint(List<string> keyParams, params Type[] types)
        {
            var sb = this.GetClassDefinitions(types);

            sb.Append(string.Join(",", keyParams.OrderBy(param => param)));

            return this.GetDeterministicHashCode(sb.ToString());
        }

        private StringBuilder GetClassDefinitions(params Type[] types)
        {
            var sb = new StringBuilder();

            foreach (var type in types)
            {
                foreach (var property in type.GetProperties().OrderBy(prop => prop.Name))
                {
                    var attributes = property.GetCustomAttributes(false);
                    var attributeList = new List<string>();

                    foreach (var attribute in attributes)
                    {
                        if (attribute is SearchableFieldAttribute searchableField)
                        {
                            attributeList.Add($"{nameof(SearchableFieldAttribute)}_{searchableField.IsKey}_{searchableField.IsFilterable}_{searchableField.AnalyzerName}");
                        }
                        else if (attribute is SimpleFieldAttribute simpleField)
                        {
                            attributeList.Add($"{nameof(SimpleFieldAttribute)}_{simpleField.IsFilterable}_{simpleField.IsSortable}");
                        }
                        else
                        {
                            attributeList.Add(attribute?.ToString() ?? string.Empty);
                        }
                    }

                    sb.Append($"{property.PropertyType.FullName}_{property.Name}_{string.Join(",", attributeList.OrderBy(attribute => attribute))}");
                }
            }

            return sb;
        }

        /// <summary>
        /// Get a deterministic hash / thumbprint for a string.
        /// https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/.
        /// </summary>
        /// <param name="stringToHash">The string we want to hash.</param>
        /// <returns>A thumbprint hash.</returns>
        private int GetDeterministicHashCode(string stringToHash)
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
