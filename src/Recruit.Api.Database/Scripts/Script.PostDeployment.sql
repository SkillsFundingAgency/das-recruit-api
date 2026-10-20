EXEC sp_executesql N'
    DECLARE @MaxRef bigint;
    SELECT @MaxRef = ISNULL(MAX(VacancyReference), 0) FROM dbo.[Vacancy];

    DECLARE @Sql nvarchar(500);
    SET @Sql = N''ALTER SEQUENCE dbo.VacancyReferenceSequence RESTART WITH '' 
             + CAST(@MaxRef + 1 AS nvarchar(20)) + N'';'';

    EXEC sp_executesql @Sql;
'
GO