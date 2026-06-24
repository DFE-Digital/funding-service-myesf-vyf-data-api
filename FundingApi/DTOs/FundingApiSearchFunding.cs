using Newtonsoft.Json;
using PDS.ViewYourFunding.Data.API.Interfaces;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using PDS.ViewYourFunding.Data.Services.Helpers;
using System;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.API.DTOs
{
    /// <summary>
    /// A class representing a single allocation returned from a search request on the Funding API.
    /// </summary>
    public class FundingApiSearchFunding : IFundingApiSearchFunding
    {
        /// <summary>
        /// Gets or sets the unique id for the funding.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of grouping  (e.g. LocalAuthority).
        /// </summary>
        [JsonProperty("groupingType")]
        public string GroupingType { get; set; }

        /// <summary>
        /// Gets or sets the reason of grouping (payment or information).
        /// </summary>
        [JsonProperty("groupingReason")]
        public string GroupingReason { get; set; }

        /// <summary>
        /// Gets or sets the funding period code for this funding (e.g. AY-1920).
        /// </summary>
        [JsonProperty("fundingPeriodCode")]
        public string FundingPeriodCode { get; set; }

        /// <summary>
        /// Gets or sets the funding stream code (e.g. DSG).
        /// </summary>
        [JsonProperty("fundingStreamCode")]
        public string FundingStreamCode { get; set; }

        /// <summary>
        /// Gets or sets the date and time this allocation was published.
        /// </summary>
        [JsonProperty("statusChangedDate")]
        public DateTime StatusChangedDate { get; set; }

        /// <summary>
        /// Gets or sets the version of this allocation.
        /// </summary>
        [JsonProperty("fundingVersion")]
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
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        [JsonProperty("searchableGroupName")]
        public string SearchableGroupName { get; set; }

        /// <summary>
        /// Gets or sets the provider establishment number.
        /// </summary>
        [JsonProperty("groupUKPRN")]
        public string GroupUkprn { get; set; }

        /// <summary>
        /// Gets or sets a code to represent the code.
        /// </summary>
        [JsonProperty("groupCode")]
        public string GroupCode { get; set; }

        /// <summary>
        /// Gets or sets the total amount of this funding.
        /// </summary>
        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

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

        /// <summary>
        /// Gets a value indicating whether the funding is first version.
        /// </summary>
        [JsonProperty("isFirstStatementChannelVersion")]
        public bool IsFirstStatementChannelVersion => StatementChannelVersionHelper.IsFirstVersionOfFunding(this.Id, this.StatementChannelVersion);
    }
}