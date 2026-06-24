using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.Data.Interfaces.Models
{
    /// <summary>
    /// An interface representing a document from a Provider Funding Search.
    /// </summary>
    public interface IProviderFundingSearchDocument
    {
        /// <summary>
        /// Gets or sets the unique id for the funding.
        /// </summary>
        [JsonProperty("id")]
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the funding this came from (if there are multiple, there will be multiple instances of this object).
        /// </summary>
        [JsonProperty("parentId")]
        string ParentId { get; set; }

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
        /// Gets or sets the provider name.
        /// </summary>
        [JsonProperty("organisationName")]
        string OrganisationName { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        [JsonProperty("searchableOrganisationName")]
        string SearchableOrganisationName { get; set; }

        /// <summary>
        /// Gets or sets the provider establishment number.
        /// </summary>
        [JsonProperty("organisationUkprn")]
        string OrganisationUkprn { get; set; }

        /// <summary>
        /// Gets or sets the provider establishment number.
        /// </summary>
        [JsonProperty("organisationDfeNumber")]
        string OrganisationDfeNumber { get; set; }

        /// <summary>
        /// Gets or sets the provider town.
        /// </summary>
        [JsonProperty("organisationTown")]
        string OrganisationTown { get; set; }

        /// <summary>
        /// Gets or sets the provider postcode.
        /// </summary>
        [JsonProperty("organisationPostcode")]
        string OrganisationPostcode { get; set; }

        /// <summary>
        /// Gets or sets name of the parent organisation group.
        /// </summary>
        [JsonProperty("parentName")]
        string ParentName { get; set; }

        /// <summary>
        /// Gets or sets the parent organisation group's primary identifier (e.g. UKPRN).
        /// </summary>
        [JsonProperty("parentPrimaryIdentifier")]
        string ParentPrimaryIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the parent organisation groups' provider type (e.g. LocalAuthority).
        /// </summary>
        [JsonProperty("parentProviderType")]
        string ParentProviderType { get; set; }

        /// <summary>
        /// Gets or sets the type of provider.
        /// </summary>
        [JsonProperty("providerType")]
        string ProviderType { get; set; }

        /// <summary>
        /// Gets or sets the reason of opening.
        /// </summary>
        [JsonProperty("openReason")]
        string OpenReason { get; set; }

        /// <summary>
        /// Gets or sets the sub-type of provider.
        /// </summary>
        [JsonProperty("providerSubType")]
        string ProviderSubType { get; set; }

        /// <summary>
        /// Gets or sets the status of provider (e.g. 'Open').
        /// </summary>
        [JsonProperty("providerStatus")]
        string ProviderStatus { get; set; }

        /// <summary>
        /// Gets or sets the closure reason (e.g. 'Closure' or 'Not Applicable').
        /// </summary>
        [JsonProperty("closeReason")]
        string CloseReason { get; set; }

        /// <summary>
        /// Gets or sets the total amount of this funding.
        /// </summary>
        [JsonProperty("totalAmount")]
        double TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets detail about the makeup of this provider funding.
        /// </summary>
        [JsonProperty("fundingValue")]
        string FundingValue { get; set; }

        /// <summary>
        /// Gets or sets the grouping reason (e.g. Payment or Information).
        /// </summary>
        [JsonProperty("groupingReason")]
        string GroupingReason { get; set; }

        /// <summary>
        /// Gets or sets the phase of education.
        /// </summary>
        [JsonProperty("phaseOfEducation")]
        string PhaseOfEducation { get; set; }

        /// <summary>
        /// Gets or sets the date opened.
        /// </summary>
        [JsonProperty("dateOpened")]
        DateTime? DateOpened { get; set; }

        /// <summary>
        /// Gets or sets the date closed.
        /// </summary>
        [JsonProperty("dateClosed")]
        DateTime? DateClosed { get; set; }

        /// <summary>
        /// Gets or sets the list of variation reasons.
        /// </summary>
        [JsonProperty("variationReasons")]
        IEnumerable<string> VariationReasons { get; set; }

        /// <summary>
        /// Gets or sets the provider urn.
        /// </summary>
        [JsonProperty("providerUrn")]
        string ProviderUrn { get; set; }

        /// <summary>
        /// Gets or sets the local authority name.
        /// </summary>
        [JsonProperty("localAuthorityName")]
        string LocalAuthorityName { get; set; }

        /// <summary>
        /// Gets or sets the parliamentary constituency name.
        /// </summary>
        [JsonProperty("parliamentaryConstituencyName")]
        public string ParliamentaryConstituencyName { get; set; }

        /// <summary>
        /// Gets or sets the parliamentary constituency code.
        /// </summary>
        [JsonProperty("parliamentaryConstituencyCode")]
        public string ParliamentaryConstituencyCode { get; set; }

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