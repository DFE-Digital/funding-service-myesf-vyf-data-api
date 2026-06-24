namespace PDS.ViewYourFunding.Data.Core
{
    /// <summary>
    /// The Funding API Core Constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The cosmos database default funding query.
        /// </summary>
        public const string CosmosDbDefaultFundingQuery
            = @"SELECT c.id, 
        c.fundingPeriod.id as fundingPeriodCode,                    
        c.fundingStream.code as fundingStreamCode, 
        c.statusChangedDate, 
        c.organisationGroup.groupTypeCode as groupingType,
        c.groupingReason, 
        c.fundingVersion, 
        c.templateVersion,  
        c.schemaVersion,  
        c.organisationGroup.name as groupName,  
        c.organisationGroup.searchableName as searchableGroupName,  
        (select value max(ukprn['value']) from ukprn in c.organisationGroup.identifiers where ukprn.type = 'UKPRN') as groupUKPRN,  
        (select value max(ukprn['value']) from ukprn in c.organisationGroup.identifiers where ukprn.type = 'LACode') as groupCode,  
        c.fundingValue.totalValue as totalAmount,  
        c.providerFundings,  
        tostring(c.fundingValue) as fundingValue,  
        c.variationReasons,  
        c._ts,
        (IS_DEFINED(c.channelVersion) ? c.channelVersion : []) as channelVersions,        
        (select value max(SV['value']) from SV in c.channelVersion where SV.type = 'Statement') as statementChannelVersion
    from c  
    where c._ts > @HighWaterMark ORDER BY c._ts";

        /// <summary>
        /// The cosmos database default provider funding query.
        /// </summary>
        public const string CosmosDbDefaultProviderFundingQuery
            = @"SELECT
        concat(c.id, parentInfo.id) as uniqueId,
        c.id,
        parentInfo.id as parentId,
        c.fundingPeriodId as fundingPeriodCode,
        c.fundingStreamCode,
        (parentInfo.statusChangedDate ?? parentInfo.externalPublicationDate) as statusChangedDate,
        c.fundingVersion, c.templateVersion, c.schemaVersion, c.provider.name as organisationName,
        c.provider.searchableName as searchableOrganisationName,
        ukprnIdentifier['value'] as organisationUkprn,
        (select value max(identifier['value']) from identifier in c.provider.otherIdentifiers where identifier.type = 'URN') as providerUrn,
        (select value max(StringToNumber(dfeIdentifier['value'])) from dfeIdentifier in c.provider.otherIdentifiers where dfeIdentifier.type = 'LACode' or dfeIdentifier.type = 'DfeNumber' or dfeIdentifier.type = 'DfeEstablishmentNumber') as organisationDfeNumber,
        c.provider.providerDetails.town as organisationTown,
        c.provider.providerDetails.postcode as organisationPostcode,
        parentInfo['group'].name as parentName, parentInfo['groupingReason'] as groupingReason,
        parentInfo['group'].groupTypeCode as parentProviderType,
        (select value max(identifier['value']) from identifier in parentInfo['group'].identifiers where identifier.type = 'LACode') as parentPrimaryIdentifier,
        c.provider.providerType, c.provider.providerSubType,
        c.fundingValue.totalValue as totalAmount,
        tostring(c.fundingValue) as fundingValue,
        c.provider.providerDetails.status as providerStatus,
        c.provider.providerDetails.openReason,
        c.provider.providerDetails.phaseOfEducation,
        c.provider.providerDetails.parliamentaryConstituencyName,
        c.provider.providerDetails.parliamentaryConstituencyCode,
        c.provider.providerDetails.dateOpened,
        c.provider.providerDetails.closeReason,
        c.provider.providerDetails.dateClosed,
        c.variationReasons,
        c.provider.providerDetails.localAuthorityName,
        c._ts,
        (IS_DEFINED(c.channelVersion) ? c.channelVersion : []) as channelVersions,        
        (select value max(SV['value']) from SV in c.channelVersion where SV.type = 'Statement') as statementChannelVersion
    from c
        join parentInfo in c.parentInformation
        join ukprnIdentifier in c.provider.otherIdentifiers 
    where ukprnIdentifier.type = 'UKPRN' 
        and c._ts > @HighWaterMark 
    ORDER BY c._ts";
    }
}