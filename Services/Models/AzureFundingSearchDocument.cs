using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Newtonsoft.Json;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using PDS.ViewYourFunding.Data.Services.Attributes;
using System;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.Services.Models
{
    /// <summary>
    /// A class representing a document from an Azure Funding Search index.
    /// </summary>
    [AzureSearchIndex("fundingapi-funding")]
    public class AzureFundingSearchDocument : IFundingSearchDocument
    {
        /// <summary>
        /// Gets or sets the unique id for the funding.
        /// </summary>
        [JsonProperty("id")]

        [SimpleField(IsFilterable = true, IsSortable = true, IsKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of grouping (e.g. LocalAuthoirty).
        /// </summary>
        [JsonProperty("groupingType")]

        [SimpleField(IsFilterable = true)]
        public string GroupingType { get; set; }

        /// <summary>
        /// Gets or sets the type of grouping (payment or information).
        /// </summary>
        [JsonProperty("groupingReason")]

        [SimpleField(IsFilterable = true)]
        public string GroupingReason { get; set; }

        /// <summary>
        /// Gets or sets the funding period code for this funding (e.g. AY-1920).
        /// </summary>
        [JsonProperty("fundingPeriodCode")]

        [SimpleField(IsFilterable = true)]
        public string FundingPeriodCode { get; set; }

        /// <summary>
        /// Gets or sets the funding stream code (e.g. DSG).
        /// </summary>
        [JsonProperty("fundingStreamCode")]

        [SimpleField(IsFilterable = true)]
        public string FundingStreamCode { get; set; }

        /// <summary>
        /// Gets or sets the date and time this allocation was published.
        /// </summary>
        [JsonProperty("statusChangedDate")]

        [SimpleField(IsFilterable = true)]
        public DateTime StatusChangedDate { get; set; }

        /// <summary>
        /// Gets or sets the version of this allocation.
        /// </summary>
        [JsonProperty("fundingVersion")]
        [SimpleField(IsFilterable = true)]
        public string FundingVersion { get; set; }

        /// <summary>
        /// Gets or sets the version of the schema.
        /// </summary>
        [JsonProperty("schemaVersion")]
        public string SchemaVersion { get; set; }

        /// <summary>
        /// Gets or sets the version of the template.
        /// </summary>
        [JsonProperty("templateVersion")]
        public string TemplateVersion { get; set; }

        /// <summary>
        /// Gets or sets the group name (e.g. East Midlands).
        /// </summary>
        [JsonProperty("groupName")]

        [SimpleField(IsFilterable = true)]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        [JsonProperty("searchableGroupName")]

        [SearchableField(IndexAnalyzerName = LexicalAnalyzerName.Values.StandardAsciiFoldingLucene, SearchAnalyzerName = LexicalAnalyzerName.Values.StandardAsciiFoldingLucene)]
        public string SearchableGroupName { get; set; }

        /// <summary>
        /// Gets or sets the provider establishment number.
        /// </summary>
        [JsonProperty("groupUKPRN")]

        [SearchableField, SimpleField(IsFilterable = true)]
        public string GroupUkprn { get; set; }

        /// <summary>
        /// Gets or sets a code to represent the code.
        /// </summary>
        [JsonProperty("groupCode")]

        [SearchableField, SimpleField(IsFilterable = true)]
        public string GroupCode { get; set; }

        /// <summary>
        /// Gets or sets the total amount of this funding.
        /// </summary>
        [JsonProperty("totalAmount")]
        public double TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets detail about the makeup of this funding.
        /// </summary>
        [JsonProperty("fundingValue")]
        public string FundingValue { get; set; }

        /// <summary>
        /// Gets or sets detail about the provider fundings.
        /// </summary>
        [JsonProperty("providerFundings")]
        public IEnumerable<string> ProviderFundings { get; set; }

        /// <inheritdoc/>
        [JsonProperty("variationReasons")]

        [SimpleField(IsFilterable = true)]
        public IEnumerable<string> VariationReasons { get; set; }

        /// <summary>
        /// Gets or sets the Channel Version.
        /// </summary>
        [JsonProperty("channelVersions")]
        public IEnumerable<ChannelVersionModel> ChannelVersions { get; set; }

        /// <summary>
        /// Gets or sets the Statement Channel Version.
        /// </summary>
        [JsonProperty("statementChannelVersion")]
        public int? StatementChannelVersion { get; set; }
    }
}