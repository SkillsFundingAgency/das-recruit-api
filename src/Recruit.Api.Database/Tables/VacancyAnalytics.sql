CREATE TABLE dbo.[VacancyAnalytics] (
    [VacancyReference]                                              bigint	      NOT NULL,
    [UpdatedDate]                                                   datetime      NOT NULL,
    [Analytics]                                                     nvarchar(MAX) NULL
    CONSTRAINT [PK_VacancyAnalytics] PRIMARY KEY (VacancyReference)
)