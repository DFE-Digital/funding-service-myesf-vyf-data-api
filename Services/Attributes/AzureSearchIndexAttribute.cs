using System;

namespace PDS.ViewYourFunding.Data.Services.Attributes
{
    /// <summary>
    /// Indicates that the class encapsulates the fields of an Azure Search index.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AzureSearchIndexAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchIndexAttribute"/> class.
        /// </summary>
        /// <param name="aliasName">The alias name.</param>
        public AzureSearchIndexAttribute(string aliasName)
        {
            AliasName = aliasName;
        }

        /// <summary>
        /// Gets the alias name that should be used as a name prefix when building the Azure Search index.
        /// </summary>
        public string AliasName { get; }
    }
}
