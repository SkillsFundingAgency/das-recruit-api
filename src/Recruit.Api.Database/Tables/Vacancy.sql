-- Notes:
-- text fields sizes are set to their current validation maximums, unless otherwise noted

CREATE TABLE dbo.[Vacancy] (
    [Id]	                                    uniqueidentifier    NOT NULL default NEWSEQUENTIALID(),
    [VacancyReference]                          bigint              NULL, -- default next value for VacancyReference,
    [AccountId]                                 bigint              NULL, -- converted from EmployerAccountId e.g. MEZSGQ
    [Status]                                    nvarchar(20)        NOT NULL default 'Draft', -- max is currently 9 chars
    [ApprenticeshipType]                        nvarchar(30)        NOT NULL default 'Standard', -- max is currently 10 chars
    [Title]                                     nvarchar(200)       NULL, -- validation max current is 100
    [OwnerType]                                 nvarchar(20)        NOT NULL, -- max is currently 8 chars
    [SourceOrigin]                              nvarchar(20)        NULL, -- max is currently 12 chars
    [SourceType]                                nvarchar(20)        NULL, -- max is currently 9 chars
    [SourceVacancyReference]                    bigint              NULL,
    [ApprovedDate]                              datetime            NULL,
    [CreatedDate]                               datetime            NOT NULL default CURRENT_TIMESTAMP,
    [LastUpdatedDate]                           datetime            NULL,
    [SubmittedDate]                             datetime            NULL,
    [ReviewDate]                                datetime            NULL,
    [ClosedDate]                                datetime            NULL,
    [DeletedDate]                               datetime            NULL,
    [LiveDate]                                  datetime            NULL,
    [StartDate]                                 datetime            NULL,
    [ClosingDate]                               datetime            NULL,
    [SubmittedByUserId]                         nvarchar(50)        NULL,
    [ReviewCount]                               int                 NOT NULL default 0,
    [ApplicationUrl]                            nvarchar(500)       NULL, -- validation currently allows 2000
    [ApplicationMethod]                         nvarchar(50)        NULL, -- max is currently 30 chars
    [ApplicationInstructions]                   nvarchar(500)       NULL,
    [ShortDescription]                          nvarchar(400)       NULL,
    [Description]                               nvarchar(max)       NULL,
    [AnonymousReason]                           nvarchar(200)       NULL,
    [DisabilityConfident]                       bit                 NULL,
    [ContactName]                               nvarchar(100)       NULL,
    [ContactEmail]                              nvarchar(100)       NULL,
    [ContactPhone]                              nvarchar(20)        NULL,
    [EmployerDescription]                       nvarchar(max)       NULL,
    [EmployerLocations]                         nvarchar(max)       NULL,
    [EmployerLocationOption]                    nvarchar(30)        NULL, -- max is currently 17 chars
    [EmployerLocationInformation]               nvarchar(500)       NULL,
    [EmployerName]                              nvarchar(100)       NULL,
    [EmployerNameOption]                        nvarchar(30)        NULL, -- max is currently 14 chars
    [EmployerRejectedReason]                    nvarchar(200)       NULL,
    [LegalEntityName]                           nvarchar(100)       NULL,
    [EmployerWebsiteUrl]                        nvarchar(100)       NULL,
    [GeoCodeMethod]                             nvarchar(30)        NULL, -- max is currently 18 chars
    [AccountLegalEntityId]                      bigint              NULL,
    [NumberOfPositions]                         int                 NULL,
    [OutcomeDescription]                        nvarchar(max)       NULL,
    [ProgrammeId]                               nvarchar(20)        NULL,
    [Skills]                                    nvarchar(max)       NULL, -- json serialised
    [Qualifications]                            nvarchar(max)       NULL, -- json serialised
    [ThingsToConsider]                          nvarchar(max)       NULL,
    [TrainingDescription]                       nvarchar(max)       NULL,
    [AdditionalTrainingDescription]             nvarchar(max)       NULL,
    [Ukprn]                                     int                 NULL,
    [TrainingProvider_Name]                     nvarchar(100)       NULL,
    [TrainingProvider_Address]                  nvarchar(500)       NULL,
    [Wage_Duration]                             int                 NULL,
    [Wage_DurationUnit]                         nvarchar(10)        NULL,
    [Wage_WorkingWeekDescription]               nvarchar(250)       NULL,
    [Wage_WeeklyHours]                          decimal             NULL,
    [Wage_WageType]                             nvarchar(40)        NULL, -- max is currently 33 chars
    [Wage_FixedWageYearlyAmount]                decimal             NULL,
    [Wage_WageAdditionalInformation]            nvarchar(250)       NULL,
    [Wage_CompanyBenefitsInformation]           nvarchar(250)       NULL,
    [ClosureReason]                             nvarchar(30)        NULL, -- max is currently 21 chars
    [TransferInfo]                              nvarchar(500)       NULL, -- json serialise
    [AdditionalQuestion1]                       nvarchar(250)       NULL,
    [AdditionalQuestion2]                       nvarchar(250)       NULL,
    [HasSubmittedAdditionalQuestions]           bit                 NULL,
    [HasChosenProviderContactDetails]           bit                 NULL,
    [HasOptedToAddQualifications]               bit                 NULL,
    [EmployerReviewFieldIndicators]             nvarchar(max)       NULL, -- json serialised
    [ProviderReviewFieldIndicators]             nvarchar(max)       NULL, -- json serialised
    
    CONSTRAINT [PK_Vacancy] PRIMARY KEY (Id),
    INDEX [IX_Vacancy_VacancyReference] NONCLUSTERED(VacancyReference),
    INDEX [IX_Vacancy_Account_Owner_Status] NONCLUSTERED(AccountId, OwnerType, Status),
    INDEX [IX_Vacancy_Ukprn_Owner_Status] NONCLUSTERED(Ukprn, OwnerType, Status),
)