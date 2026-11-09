IF NOT EXISTS (
    SELECT 1 FROM sys.sequences 
    WHERE name = 'VacancyReference' 
      AND schema_name(schema_id) = 'dbo'
)
BEGIN
CREATE SEQUENCE dbo.VacancyReference
    AS bigint
    START WITH 2000000000
    INCREMENT BY 1;
END


DECLARE @MaxRef   bigint = (
    SELECT ISNULL(MAX([VacancyReference
]),
1999999999) 
    FROM dbo.Vacancy
);
DECLARE @CurrentSeq bigint = (
    SELECT current_value 
    FROM sys.sequences 
    WHERE name = 'VacancyReference' 
      AND schema_name(schema_id) = 'dbo'
);

IF @CurrentSeq <= @MaxRef
BEGIN
ALTER SEQUENCE dbo.VacancyReference RESTART WITH @MaxRef + 1;
END