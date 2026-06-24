using Newtonsoft.Json;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using System;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.API.Interfaces
{
    /// <summary>
    /// An interface representing a funding returned from a search request on the Funding API.
    /// </summary>
    public interface IFundingApiSearchFunding
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
        /// Gets or sets the reason of grouping (payment or information).
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
        /// Gets or sets the group name without certain characters (e.g. 'EastMidlands' rather than 'East Midlands').
        /// </summary>
        [JsonProperty("searchableGroupName")]
        string SearchableGroupName { get; set; }

        /// <summary>
        /// Gets or sets the groups UKPRN (if applicable).
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
        decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets detail about the make up of this funding.
        /// </summary>
        [JsonProperty("fundingValue")]
        string FundingValue { get; set; }

        /// <summary>
        /// Gets or sets detail about the provider fundings.
        /// </summary>
        [JsonProperty("providerFundings")]
        IEnumerable<string> ProviderFundings { get; set; }

        /// <summary>
        /// Gets or sets the Channel Version.
        /// </summary>
        [JsonProperty("channelVersions")]
        public IEnumerable<ChannelVersionModel> ChannelVersions { get; set; }

        /// <summary>
        /// Gets or sets the Statement Channel Version.
        /// </summary>
        [JsonProperty("statementChannelVersion")]
        int? StatementChannelVersion { get; set; }

        /// <summary>
        /// Gets a value indicating whether the funding is first version.
        /// </summary>
        [JsonProperty("isFirstStatementChannelVersion")]
        bool IsFirstStatementChannelVersion { get; }
    }
}
