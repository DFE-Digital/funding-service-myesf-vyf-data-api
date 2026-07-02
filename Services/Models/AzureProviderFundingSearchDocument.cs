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
    [AzureSearchIndex("fundingapi-providerfunding")]
    public class AzureProviderFundingSearchDocument : IProviderFundingSearchDocument
    {
        /// <summary>
        /// Gets or sets the unique id for the funding.
        /// </summary>
        [JsonProperty("uniqueid")]

        [SimpleField(IsFilterable = true, IsSortable = true, IsKey = true)]
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets the unique id for the funding.
        /// </summary>
        [JsonProperty("id")]

        [SimpleField(IsFilterable = true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the funding this came from (if there are multiple, there will be multiple instances of this object).
        /// </summary>
        [JsonProperty("parentId")]

        [SimpleField(IsFilterable = true)]
        public string ParentId { get; set; }

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

        [SimpleField(IsFilterable = true, IsSortable = true)]
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
        /// Gets or sets the provider name.
        /// </summary>
        [JsonProperty("organisationName")]
        public string OrganisationName { get; set; }

        /// <summary>
        /// Gets or sets the searchable provider name (strips some characters).
        /// </summary>
        [JsonProperty("searchableOrganisationName")]

        [SearchableField(IndexAnalyzerName = LexicalAnalyzerName.Values.StandardAsciiFoldingLucene, SearchAnalyzerName = LexicalAnalyzerName.Values.StandardAsciiFoldingLucene)]
        public string SearchableOrganisationName { get; set; }

        /// <summary>
        /// Gets or sets the provider establishment number.
        /// </summary>
        [JsonProperty("organisationUkprn")]

        [SearchableField, SimpleField(IsFilterable = true)]
        public string OrganisationUkprn { get; set; }

        /// <summary>
        /// Gets or sets the provider establishment number.
        /// </summary>
        [JsonProperty("organisationDfeNumber")]

        [SearchableField]
        public string OrganisationDfeNumber { get; set; }

        /// <summary>
        /// Gets or sets the provider town.
        /// </summary>
        [JsonProperty("organisationTown")]
        public string OrganisationTown { get; set; }

        /// <summary>
        /// Gets or sets the provider postcode.
        /// </summary>
        [JsonProperty("organisationPostcode")]
        public string OrganisationPostcode { get; set; }

        /// <summary>
        /// Gets or sets name of the parent organisation group.
        /// </summary>
        [JsonProperty("parentName")]

        [SimpleField(IsFilterable = true)]
        public string ParentName { get; set; }

        /// <summary>
        /// Gets or sets the parent organisation group's primary identifier (e.g. UKPRN).
        /// </summary>
        [JsonProperty("parentPrimaryIdentifier")]

        [SimpleField(IsFilterable = true)]
        public string ParentPrimaryIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the parent organisation groups' provider type (e.g. LocalAuthority).
        /// </summary>
        [JsonProperty("parentProviderType")]

        [SimpleField(IsFilterable = true)]
        public string ParentProviderType { get; set; }

        /// <summary>
        /// Gets or sets the type of provider.
        /// </summary>
        [JsonProperty("providerType")]
        public string ProviderType { get; set; }

        /// <summary>
        /// Gets or sets the reason of opening.
        /// </summary>
        [JsonProperty("openReason")]
        public string OpenReason { get; set; }

        /// <summary>
        /// Gets or sets the status of provider (e.g. 'Open').
        /// </summary>
        [JsonProperty("providerStatus")]

        [SimpleField(IsFilterable = true)]
        public string ProviderStatus { get; set; }

        /// <summary>
        /// Gets or sets the closure reason (e.g. 'Closure' or 'Not Applicable').
        /// </summary>
        [JsonProperty("closeReason")]
        public string CloseReason { get; set; }

        /// <summary>
        /// Gets or sets the sub-type of provider.
        /// </summary>
        [JsonProperty("providerSubType")]
        public string ProviderSubType { get; set; }

        /// <summary>
        /// Gets or sets the total amount of this funding.
        /// </summary>
        [JsonProperty("totalAmount")]
        public double TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets detail about the makeup of this provider funding.
        /// </summary>
        [JsonProperty("fundingValue")]
        public string FundingValue { get; set; }

        /// <summary>
        /// Gets or sets detail about the provider fundings.
        /// </summary>
        [JsonProperty("providerFundings")]
        public IEnumerable<string> ProviderFundings { get; set; }

        /// <summary>
        /// Gets or sets the grouping reason (e.g. Payment or Information).
        /// </summary>
        [JsonProperty("groupingReason")]

        [SimpleField(IsFilterable = true)]
        public string GroupingReason { get; set; }

        /// <summary>
        /// Gets or sets the phase of education.
        /// </summary>
        [JsonProperty("phaseOfEducation")]
        public string PhaseOfEducation { get; set; }

        /// <summary>
        /// Gets or sets the date opened.
        /// </summary>
        [JsonProperty("dateOpened")]
        public DateTime? DateOpened { get; set; }

        /// <summary>
        /// Gets or sets the date closed.
        /// </summary>
        [JsonProperty("dateClosed")]
        public DateTime? DateClosed { get; set; }

        /// <inheritdoc/>
        [JsonProperty("variationReasons")]

        [SimpleField(IsFilterable = true)]
        public IEnumerable<string> VariationReasons { get; set; }

        /// <summary>
        /// Gets or sets the provider urn.
        /// </summary>
        [JsonProperty("providerUrn")]
        public string ProviderUrn { get; set; }

        /// <summary>
        /// Gets or sets the local authority name.
        /// </summary>
        [JsonProperty("localAuthorityName")]
        public string LocalAuthorityName { get; set; }

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
        public IEnumerable<ChannelVersionModel> ChannelVersions { get; set; }

        /// <summary>
        /// Gets or sets the Statement Channel Version.
        /// </summary>
        [JsonProperty("statementChannelVersion")]
        public int? StatementChannelVersion { get; set; }
    }
}