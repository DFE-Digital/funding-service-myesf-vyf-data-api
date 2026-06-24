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
WHERE CONCAT(c.fundingStreamCode, '-', c.fundingPeriodId) > 'GAG-AC-2425'
        AND c._ts > @HighWaterMark 
ORDER BY c._ts