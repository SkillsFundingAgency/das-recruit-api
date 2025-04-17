CREATE TABLE dbo.EmployerProfileAddress (
    [Id]					int identity(1,1),
    [AccountLegalEntityId]  bigint NOT NULL,
    [AddressLine1]          nvarchar(150) NOT NULL,
    [AddressLine2]          nvarchar(150) NULL,
    [AddressLine3]          nvarchar(150) NULL,
    [AddressLine4]          nvarchar(150) NULL,
    [Postcode]              nvarchar(50) NULL,
    [Latitude]              float NULL,
    [Longitude]             float NULL,
    CONSTRAINT [PK_EmployerProfileAddress] PRIMARY KEY (Id),
    CONSTRAINT [FK_EmployerProfileAddress_EmployerProfile] FOREIGN KEY (AccountLegalEntityId) REFERENCES [EmployerProfile](AccountLegalEntityId),
)