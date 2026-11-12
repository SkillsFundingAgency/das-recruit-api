IF NOT EXISTS (SELECT 1 FROM sys.sequences 
               WHERE name = 'VacancyReference' 
               AND SCHEMA_NAME(schema_id) = 'dbo')
BEGIN
    EXEC('CREATE SEQUENCE dbo.VacancyReference AS bigint START WITH 2000000000 INCREMENT BY 1');
    PRINT 'Sequence created.';
END
GO

DECLARE @NextVal bigint;

SELECT @NextVal = ISNULL(MAX(VacancyReference), 1999999999) + 1
FROM dbo.Vacancy;

IF @NextVal >= 2000000000
BEGIN
    EXEC('ALTER SEQUENCE dbo.VacancyReference RESTART WITH ' + @NextVal);
    PRINT 'Sequence recalibrated to: ' + CAST(@NextVal AS varchar(20));
END
GO