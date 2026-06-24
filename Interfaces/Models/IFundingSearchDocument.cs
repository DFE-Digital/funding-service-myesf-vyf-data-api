using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.Interfaces.Models
{
    /// <summary>
    /// An interface representing a document from an Funding Search.
    /// </summary>
    public interface IFundingSearchDocument
    {
        /// <summary>
        /// Gets or sets the unique id for the funding.
        /// </summary>
        [JsonProperty("id")]
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of grouping (e.g. LocalAuthority).
        /// </summary>
        [JsonProperty("groupingType")]
        string GroupingType { get; set; }

        /// <summary>
        /// Gets or sets the type of grouping (payment or information).
        /// </summary>
        [JsonProperty("groupingReason")]
        string GroupingReason { get; set; }

        /// <summary>
        /// Gets or sets the funding period code for this funding (e.g. AY-1920).
        /// </summary>
        [JsonProperty("fundingPeriodCode")]
        string FundingPeriodCode { get; set; }

        /// <summary>
        /// Gets or sets the funding stream code (e.g. DSG).
        /// </summary>
        [JsonProperty("fundingStreamCode")]
        string FundingStreamCode { get; set; }

        /// <summary>
        /// Gets or sets the date and time this allocation was published.
        /// </summary>
        [JsonProperty("statusChangedDate")]
        DateTime StatusChangedDate { get; set; }

        /// <summary>
        /// Gets or sets the version of this allocation.
        /// </summary>
        [JsonProperty("fundingVersion")]
        string FundingVersion { get; set; }

        /// <summary>
        /// Gets or sets the version of the schema.
        /// </summary>
        [JsonProperty("schemaVersion")]
        string SchemaVersion { get; set; }

        /// <summary>
        /// Gets or sets the version of the template.
        /// </summary>
        [JsonProperty("templateVersion")]
        string TemplateVersion { get; set; }

        /// <summary>
        /// Gets or sets the group name (e.g. East Midlands).
        /// </summary>
        [JsonProperty("groupName")]
        string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        [JsonProperty("searchableLaName")]
        string SearchableGroupName { get; set; }

        /// <summary>
        /// Gets or sets the provider establishment number.
        /// </summary>
        [JsonProperty("groupUKPRN")]
        string GroupUkprn { get; set; }

        /// <summary>
        /// Gets or sets a code to represent the code.
        /// </summary>
        [JsonProperty("groupCode")]
        string GroupCode { get; set; }

        /// <summary>
        /// Gets or sets the total amount of this funding.
        /// </summary>
        [JsonProperty("totalAmount")]
        double TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets detail about the makeup of this funding.
        /// </summary>
        [JsonProperty("fundingValue")]
        string FundingValue { get; set; }

        /// <summary>
        /// Gets or sets detail about the provider fundings.
        /// </summary>
        [JsonProperty("providerFundings")]
        IEnumerable<string> ProviderFundings { get; set; }

        /// <summary>
        /// Gets or sets the list of variation reasons.
        /// </summary>
        [JsonProperty("variationReasons")]
        IEnumerable<string> VariationReasons { get; set; }

        /// <summary>
        /// Gets or sets the Channel Version.
        /// </summary>
        [JsonProperty("channelVersions")]
        IEnumerable<ChannelVersionModel> ChannelVersions { get; set; }

        /// <summary>
        /// Gets or sets the Statement Channel Version.
        /// </summary>
        [JsonProperty("statementChannelVersion")]
        int? StatementChannelVersion { get; set; }
    }
}
