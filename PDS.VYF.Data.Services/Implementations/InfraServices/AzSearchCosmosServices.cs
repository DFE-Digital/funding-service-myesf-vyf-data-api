namespace PDS.VYF.Data.Services.Implementations.InfraServices
{
    using Microsoft.Extensions.Options;
    using PDS.ViewYourFunding.Data.Core;
    using PDS.VYF.Data.Services.Abstracts.InfraServices;
    using PDS.VYF.Data.Services.Enums;
    using PDS.VYF.Data.Services.Extensions;
    using PDS.VYF.Data.Services.Models.AzSearchModels;

    /// <summary>
    /// The service class of Cosmos Queries for Azure Search.
    /// </summary>
    public class AzSearchCosmosServices : IAzSearchCosmosServices
    {
        private readonly string loggedInParentQuery = @"
SELECT c.id
    , c.fundingPeriod.id as fundingPeriodCode
    , c.fundingStream.code as fundingStreamCode
    , fundingStreamPeriod
    , c.statusChangedDate
    , c.organisationGroup.groupTypeCode as groupingType
    , c.groupingReason
    , CONCAT(c.fundingStream.code, '-', c.organisationGroup.groupTypeCode, '-', c.groupingReason) as fundingStreamGroupingTypeReason
    , c.fundingVersion
    , StringToNumber(Replace(c.fundingVersion, '_', '.')) fundingVersionInt
    , c.templateVersion
    , c.schemaVersion
    , c.organisationGroup.name as groupName
    , c.organisationGroup.searchableName as searchableGroupName
    , (select value max(ukprn['value']) from ukprn in c.organisationGroup.identifiers where ukprn.type = 'UKPRN') as groupUKPRN
    , (select value max(ukprn['value']) from ukprn in c.organisationGroup.identifiers where ukprn.type = 'LACode') as groupCode
    , c.fundingValue.totalValue as totalAmount
    , c.providerFundings
    , array(select value substring(replace(pf, fundingStreamPeriod, ''), 1 ,8) from pf in c.providerFundings) ChildUKPRNs
    , array(select fl.templateLineId, fl['value'] from fl in c.fundingValue.fundingLines WHERE IS_NULL(fl['value']) = false) fundinglines
    , array(select cal.templateCalculationId, cal['value'] from cal in c.fundingValue.calculations WHERE IS_NULL(cal['value']) = false) calculations
    , c.variationReasons
    , c._ts
    , (IS_DEFINED(c.channelVersion) ? c.channelVersion : []) as channelVersions
    , (select value max(SV['value']) from SV in c.channelVersion where SV.type = 'Statement') as statementChannelVersion
    , IsParent
FROM c
    join (select value ARRAY_LENGTH(c.providerFundings) >= 1 and ToString(c.providerFundings) NOT LIKE CONCAT('%-', c.partitionKey, '-%')) as IsParent
    join (SELECT VALUE CONCAT(c.fundingStream.code, '-', c.fundingPeriod.id)) fundingStreamPeriod
WHERE c.fundingStream.code = 'GAG'
  AND (c.id like '%Payment-AcademyTrust%' or c.id like '%Indicative-AcademyTrust%')
  AND c.fundingPeriod.id >= 'AC-2324'
  AND c._ts > @HighWaterMark
ORDER BY c._ts
";

        private readonly string loggedInChildQuery = @"
    SELECT c.id
        , CONCAT(c.fundingStreamCode, '-', c.fundingPeriodId, '-', c.partitionKey) as idWithoutVersion
        , array(select PI.id as parentId
                    , PI['group'].name as parentName
                    , PI['groupingReason'] as groupingReason
                    , PI['group'].groupTypeCode as parentProviderType
                    , CONCAT(PI['group'].groupTypeCode, '-', PI['groupingReason']) groupingTypeReason
                    , PI.statusChangedDate ?? PI.externalPublicationDate as statusChangedDate
                    , (select value MAX(identifier['value'])
                        from identifier in PI['group'].identifiers
                        where identifier.type = 'LACode') as parentPrimaryIdentifier
                    , (select value MAX(identifier['value'])
                        from identifier in PI['group'].identifiers
                        where identifier.type = 'UKPRN') as parentUKPRN
                FROM PI in c.parentInformation) as parentInfo
        , c.fundingPeriodId as fundingPeriodCode
        , c.fundingStreamCode
        , CONCAT(c.fundingStreamCode, '-', c.fundingPeriodId) as fundingStreamPeriod
        , (2000 + StringToNumber(substring(c.fundingPeriodId, 3, 2))) as yearFrom
        , (2000 + StringToNumber(substring(c.fundingPeriodId, 5, 2))) as yearTo
        , statusChangedDate
        , LEFT(statusChangedDate, 10) as statusChangedDateOnly
        , c.fundingVersion
        , StringToNumber(Replace(c.fundingVersion, '_', '.')) fundingVersionInt
        , c.templateVersion
        , c.schemaVersion
        , c.provider.name as organisationName
        , c.provider.searchableName as searchableOrganisationName
        , (select value max(identifier['value']) 
                from identifier in c.provider.otherIdentifiers
                where identifier.type = 'UKPRN') as organisationUkprn
        , (select value max(identifier['value']) 
                from identifier in c.provider.otherIdentifiers
                where identifier.type = 'URN') as providerUrn
        , (select value max(StringToNumber(dfeIdentifier['value']))
                from dfeIdentifier in c.provider.otherIdentifiers
                where dfeIdentifier.type in ('LACode', 'DfeNumber', 'DfeEstablishmentNumber')) as organisationDfeNumber
        , c.provider.providerDetails.town as organisationTown
        , c.provider.providerDetails.postcode as organisationPostcode
        , c.provider.providerType
        , c.provider.providerSubType
        , c.fundingValue.totalValue as totalAmount
        , array(select 
                       fl.templateLineId,
                       fl['value'],
                       fl['type'],
                       ARRAY(select DP.distributionPeriodId, DP['value']  FROM DP in fl.distributionPeriods) as distributionPeriods
                from fl in c.fundingValue.fundingLines
                where IS_NULL(fl['value']) = false) fundingLines
        , array(select fl.templateLineId, fl['value']
                from fl in c.fundingValue.fundingLines
                where IS_NULL(fl['value']) = false and fl.templateLineId in (1, 718, 297, 298, 299, 300)) fundingLinesForSummary
        , array(select cal.templateCalculationId, cal['value']
                from cal in c.fundingValue.calculations
                WHERE IS_NULL(cal['value']) = false) calculations
        , array(select cal.templateCalculationId, cal['value']
                from cal in c.fundingValue.calculations
                WHERE IS_NULL(cal['value']) = false and cal.templateCalculationId in (17, 567, 710, 711, 733)) calculationsForSummary
        , c.provider.providerDetails.status as providerStatus
        , c.provider.providerDetails.dateOpened
        , c.provider.providerDetails.openReason
        , c.provider.providerDetails.closeReason
        , c.provider.providerDetails.dateClosed
        , c.provider.providerDetails.phaseOfEducation
        , c.provider.providerDetails.localAuthorityName
        , c.provider.providerDetails.parliamentaryConstituencyName
        , c.provider.providerDetails.parliamentaryConstituencyCode
        , c.variationReasons
        , c._ts
        , (select value max(SV['value'])
                from SV in c.channelVersion
                where SV.type = 'Statement') as statementChannelVersion
        , statementType
        , IsIndicative
    from c
        JOIN (SELECT value MIN(PI.statusChangedDate ?? PI.externalPublicationDate) FROM PI in c.parentInformation) as statusChangedDate
        join (select value is_defined(c.channelVersion) 
                                ? array_contains(c.channelVersion, {'type' : 'Statement', 'value' : 1}) 
                                    ? 'New' 
                                    : 'Updated'
                                : endswith(c.id, '-1_0')
                                    ? 'New'
                                    : 'Updated') as statementType
        join (select value c.provider.providerDetails.status in ('Proposed to open', 'Pending approval')
                            and array_contains(c.parentInformation, {groupingReason : 'Indicative' }, true)) as IsIndicative
    WHERE  c.fundingStreamCode in ('GAG')
            AND c.fundingPeriodId >= 'AC-2324'
            and (statementType = 'New'
                    or endswith(c.id, '-1_0')
                    or EXISTS(select 1 FROM vr in c.variationReasons WHERE ARRAY_CONTAINS(<<FilterVariationReasons>>, vr)))
            AND c._ts > @HighWaterMark 
    ORDER BY c._ts";

        private readonly ApplicationConfiguration appConfiguration;

        private string? filterVariationReasons;

        /// <summary>
        /// Gets the filter variation reasons.
        /// </summary>
        /// <value>
        /// The filter variation reasons.
        /// </value>
        public string FilterVariationReasons
        {
            get
            {
                if (this.filterVariationReasons == null)
                {
                    this.filterVariationReasons = "[" + this.appConfiguration.RestrictedVariationReasons.AddQuoteInEachValue(",", true) + "]";
                }

                return this.filterVariationReasons;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzSearchCosmosServices"/> class.
        /// </summary>
        /// <param name="appConfigurationOptions">The application configuration options.</param>
        public AzSearchCosmosServices(IOptions<ApplicationConfiguration> appConfigurationOptions)
        {
            this.appConfiguration = appConfigurationOptions.Value;
        }

        /// <summary>
        /// Gets the cosmos query.
        /// </summary>
        /// <param name="azSearchIndexTypeEnum">The az search index type enum.</param>
        /// <returns>The cosmos query for the given azure index type.</returns>
        public string GetCosmosQuery(AzSearchIndexTypeEnum azSearchIndexTypeEnum)
        {
            return azSearchIndexTypeEnum switch
            {
                AzSearchIndexTypeEnum.LoggedIn_Parent => this.loggedInParentQuery,
                AzSearchIndexTypeEnum.LoggedIn_Child => this.loggedInChildQuery.Replace("<<FilterVariationReasons>>", this.FilterVariationReasons),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Gets the name of the container.
        /// </summary>
        /// <param name="azSearchIndexTypeEnum">The az search index type enum.</param>
        /// <returns>The Cosmos Cotainer name for the given azure Search index type.</returns>
        public string GetContainerName(AzSearchIndexTypeEnum azSearchIndexTypeEnum)
        {
            return azSearchIndexTypeEnum switch
            {
                AzSearchIndexTypeEnum.LoggedIn_Parent => this.appConfiguration.Repositories.CosmosDb.FundingCollection,
                AzSearchIndexTypeEnum.LoggedIn_Child => this.appConfiguration.Repositories.CosmosDb.ProviderFundingCollection,
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Gets the type of the index field.
        /// </summary>
        /// <param name="azSearchIndexTypeEnum">The az search index type enum.</param>
        /// <returns>The Type of the given azure Search Index type.</returns>
        public Type GetIndexFieldType(AzSearchIndexTypeEnum azSearchIndexTypeEnum)
        {
            return azSearchIndexTypeEnum switch
            {
                AzSearchIndexTypeEnum.LoggedIn_Parent => typeof(LoggedInParentAzSearchModel),
                AzSearchIndexTypeEnum.LoggedIn_Child => typeof(LoggedInChildAzSearchModel),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Gets the index types for thumb print.
        /// </summary>
        /// <param name="azSearchIndexTypeEnum">The az search index type enum.</param>
        /// <returns>All types for ThumbPrint Calculation.</returns>
        public Type[] GetIndexTypesForThumbPrint(AzSearchIndexTypeEnum azSearchIndexTypeEnum)
        {
            return azSearchIndexTypeEnum switch
            {
                AzSearchIndexTypeEnum.LoggedIn_Parent => new Type[]
                {
                    typeof(LoggedInParentAzSearchModel),
                    typeof(LoggedInTemplateLine),
                    typeof(LoggedInCalculation),
                    typeof(LoggedInDistributionPeriod)
                },
                AzSearchIndexTypeEnum.LoggedIn_Child => new Type[]
                {
                    typeof(LoggedInChildAzSearchModel),
                    typeof(LoggedInTemplateLine),
                    typeof(LoggedInCalculation),
                    typeof(LoggedInDistributionPeriod),
                    typeof(LoggedInParentInfoModel)
                },
                _ => throw new NotImplementedException(),
            };
        }
    }
}
