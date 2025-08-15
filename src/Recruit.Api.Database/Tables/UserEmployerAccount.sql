CREATE TABLE dbo.[UserEmployerAccount] (
    [UserId]	                                                    uniqueidentifier	NOT NULL,
    [EmployerAccountId]                                             nvarchar(30)        NOT NULL,
    CONSTRAINT [PK_UserEmployerAccount] PRIMARY KEY (UserId, EmployerAccountId),
    CONSTRAINT [FK_UserEmployerAccount_User] FOREIGN KEY (UserId) REFERENCES [User](Id),
    INDEX [IX_EmployerAccount] NONCLUSTERED(EmployerAccountId),
)