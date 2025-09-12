CREATE TABLE dbo.[RecruitNotification]
(
    [Id]	                BIGINT              identity(1,1),
    [UserId]                uniqueidentifier    NOT NULL,
    [EmailTemplateId]       uniqueidentifier 	NOT NULL,
    [SendWhen]              DATETIME            NOT NULL,                   -- Date at which this should be sent
    [StaticData]            nvarchar(1000)      NOT NULL,                   -- Data that is unique for the email
    [DynamicData]           nvarchar(max)       NOT NULL,                   -- Data that will be collated in an email
    CONSTRAINT [PK_RecruitNotifications] PRIMARY KEY (Id),
    INDEX [IX_PK_RecruitNotifications_SendWhen] NONCLUSTERED(SendWhen),
)