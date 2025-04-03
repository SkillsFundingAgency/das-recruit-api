CREATE TABLE dbo.[ProhibitedContent] (
    [ContentType]                   TINYINT NOT NULL,
    [Content]                       NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_ProhibitedContent] PRIMARY KEY (ContentType,Content)
)