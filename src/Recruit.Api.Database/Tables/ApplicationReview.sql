CREATE TABLE dbo.[ApplicationReview] (
    [Id]					        uniqueidentifier	NOT NULL,
    [Ukprn]                         int NOT NULL,
    [AccountId]                     bigint NOT NULL,
    [AccountLegalEntityId]          bigint NOT NULL,
    [Owner]                         tinyint NOT NULL,
    [CandidateFeedback]             nvarchar(max) NULL,
    [EmployerFeedback]              nvarchar(max) NULL,
    [CandidateId]                   uniqueidentifier not NULL,
    [CreatedDate]                   DATETIME NOT NULL,
    [DateSharedWithEmployer]        DATETIME NULL,
    [HasEverBeenEmployerInterviewing] BIT NOT NULL DEFAULT 0,
    [WithdrawnDate]                 DATETIME NULL,
    [ReviewedDate]                  DATETIME NULL,   
    [SubmittedDate]                 DATETIME NOT NULL,   
    [Status]                        NVARCHAR(50) NOT NULL,
    [StatusUpdatedDate]             DATETIME NULL,
    [VacancyReference]              BIGINT NOT NULL,
    [LegacyApplicationId]           uniqueidentifier NULL,
    [ApplicationId]                 uniqueidentifier NULL, --For V2 Applications
    [AdditionalQuestion1]           NVARCHAR(500) NULL,
    [AdditionalQuestion2]           NVARCHAR(500) NULL,
    [VacancyTitle]                  NVARCHAR(500) NOT NULL,
    CONSTRAINT [PK_ApplicationReview] PRIMARY KEY (Id),
    INDEX [IX_ApplicationReview_Ukprn] NONCLUSTERED(Ukprn, Owner),
    INDEX [IX_ApplicationReview_AccountId] NONCLUSTERED(AccountId, Owner),
    INDEX [IX_ApplicationReview_CandidateId] NONCLUSTERED(CandidateId),
    INDEX [IX_ApplicationReview_VacancyRef] NONCLUSTERED(VacancyReference)
)