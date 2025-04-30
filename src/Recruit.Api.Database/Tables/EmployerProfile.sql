CREATE TABLE dbo.EmployerProfile (
    [AccountLegalEntityId]      bigint NOT NULL,
    [AccountId]                 bigint NOT NULL,
    [AboutOrganisation]         nvarchar(MAX) NULL,
    [TradingName]               nvarchar(200) NULL,
    CONSTRAINT [PK_EmployerProfile] PRIMARY KEY (AccountLegalEntityId),
    INDEX [IX_EmployerProfile_AccountId] NONCLUSTERED(AccountId)
)