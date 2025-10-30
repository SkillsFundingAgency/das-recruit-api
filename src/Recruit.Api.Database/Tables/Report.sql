CREATE TABLE dbo.[Report]
(
    [Id]	                uniqueidentifier    NOT NULL,
    [UserId]                nvarchar(200)       NOT NULL,                   -- User Id or DfEUserId of the report owner
    [Name]                  nvarchar(100)       NOT NULL,                   -- Name of the report
    [OwnerType]             nvarchar(20)        NOT NULL,				    -- Type of owner (e.g., 'Provider', 'Qa')
    [Type]                  nvarchar(20)        NOT NULL,				    -- Type of owner (e.g., 'ProviderApplications', 'QaApplications')
    [CreatedDate]           DATETIME            NOT NULL,                   -- Date at which this should be created    
    [CreatedBy]             nvarchar(50)        NULL,                       -- Name of the Report requestor    
    [DownloadCount]         INT                 NOT NULL DEFAULT(0),        -- Number of times the report has been downloaded
    [DynamicCriteria]       nvarchar(max)       NOT NULL,                  
    CONSTRAINT [PK_Report] PRIMARY KEY (Id),
    INDEX [IX_PK_Report_UserId] NONCLUSTERED(UserId),
)