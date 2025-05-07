CREATE TABLE dbo.[UserNotificationPreferences] (
    [UserId]                UNIQUEIDENTIFIER NOT NULL,
    [Types]                 NVARCHAR(1024) NULL,
    [Scope]                 NVARCHAR(128) NULL,
    [Frequency]             NVARCHAR(128) NULL,
    CONSTRAINT [PK_UserNotificationPreferences] PRIMARY KEY (UserId)
)