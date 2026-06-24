
    SELECT
        concat(c.id, parentInfo.id) as uniqueId
        , c.id
        , parentInfo.id as parentId
        , c.fundingPeriodId as fundingPeriodCode
        , c.fundingStreamCode
        , CONCAT(c.fundingStreamCode, '-', c.fundingPeriodId) as fundingStreamPeriod
        , (parentInfo.statusChangedDate ?? parentInfo.externalPublicationDate) as statusChangedDate
        , c.fundingVersion
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
        , parentInfo['group'].name as parentName
        , parentInfo['groupingReason'] as groupingReason
        , parentInfo['group'].groupTypeCode as parentProviderType
        , (select value max(identifier['value'])
                from identifier in parentInfo['group'].identifiers
                where identifier.type = 'LACode') as parentPrimaryIdentifier
        , c.provider.providerType
        , c.provider.providerSubType
        , c.fundingValue.totalValue as totalAmount
        , array(select fl.templateLineId, fl['value']
                from fl in c.fundingValue.fundingLines
                where IS_NULL(fl['value']) = false) fundingLines
        , array(select cal.templateCalculationId, cal['value']
                from cal in c.fundingValue.calculations
                WHERE IS_NULL(cal['value']) = false) calculations
        , c.provider.providerDetails.status as providerStatus
        , c.provider.providerDetails.dateOpened
        , c.provider.providerDetails.openReason
        , c.provider.providerDetails.closeReason
        , c.provider.providerDetails.phaseOfEducation
        , c.provider.providerDetails.parliamentaryConstituencyName
        , c.provider.providerDetails.parliamentaryConstituencyCode
        , c.variationReasons
        , c.provider.providerDetails.localAuthorityName
        , c._ts
        , (select value max(SV['value'])
                from SV in c.channelVersion
                where SV.type = 'Statement') as statementChannelVersion
        , statementType
        , IYOCalcStage1.IsIndicative
        , InYearOpener
    from c
        join parentInfo in c.parentInformation
        join (select value is_defined(c.channelVersion) 
                                ? array_contains(c.channelVersion, {'type' : 'Statement', 'value' : 1}) 
                                    ? 'New' 
                                    : 'Updated'
                                : endswith(c.id, '-1_0') 
                                    ? 'New' 
                                    : 'Updated') as statementType
        join (select (2000 + StringToNumber(substring(c.id, 7, 2))) as StartYear,
                        (2000 + StringToNumber(substring(c.id, 9, 2))) as EndYear,
                        c.provider.providerDetails.status in ('Proposed to open', 'Pending approval') 
                            and array_contains(c.parentInformation, {groupingReason : 'Indicative' }, true) as IsIndicative,
                        DateTimeFromParts(StringToNumber(substring(c.provider.providerDetails.dateOpened, 0, 4)),
                                    StringToNumber(substring(c.provider.providerDetails.dateOpened, 5, 2)),
                                    StringToNumber(substring(c.provider.providerDetails.dateOpened, 8, 2))) as DateOpened,
                        (select value MAX(C733['value']) from C733 in c.fundingValue.calculations where C733.templateCalculationId = 733) as C733_DaysInFullYear,
                        (select value MAX(C567['value']) from C567 in c.fundingValue.calculations where C567.templateCalculationId = 567) as C567_DaysOpenInYear) IYOCalcStage1
        join (select (IYOCalcStage1.DateOpened >= DateTimeFromParts(IYOCalcStage1.StartYear, 04, 01) and IYOCalcStage1.DateOpened <= DateTimeFromParts(IYOCalcStage1.StartYear, 08, 31)) as IsSecondYearInYearOpener,
                     (IYOCalcStage1.DateOpened >= DateTimeFromParts(IYOCalcStage1.StartYear, 08, 31) and IYOCalcStage1.DateOpened <= DateTimeFromParts(IYOCalcStage1.EndYear, 07, 31)) as IsAcademicYearInYearOpener,
                     (IYOCalcStage1.C733_DaysInFullYear != IYOCalcStage1.C567_DaysOpenInYear ) as IsOpenDaysEqualToFullYearDays,
                     (not contains(c.provider.providerDetails.openReason ?? '', 'Fresh Start', true)) as IsNotARebrokerage) as IYOCalcStage2
        join (select value (IYOCalcStage2.IsOpenDaysEqualToFullYearDays or IYOCalcStage2.IsSecondYearInYearOpener or IYOCalcStage2.IsAcademicYearInYearOpener) and IYOCalcStage2.IsNotARebrokerage) as InYearOpener
    WHERE c.fundingStreamCode in ('GAG')
            AND c.fundingPeriodId > 'AC-2324'
            AND c._ts > @HighWaterMark 
    ORDER BY c._ts