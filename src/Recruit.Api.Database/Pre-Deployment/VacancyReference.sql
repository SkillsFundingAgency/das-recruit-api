DECLARE @NextReference BIGINT;
SELECT @NextReference = ISNULL(MAX(VacancyReference), 1999999999) + 1
FROM dbo.Vacancy;
DECLARE @SQL NVARCHAR(200);
SET @SQL = N'ALTER SEQUENCE dbo.VacancyReference RESTART WITH '
         + CAST(@NextReference AS NVARCHAR(30));
EXEC sys.sp_executesql @SQL;
