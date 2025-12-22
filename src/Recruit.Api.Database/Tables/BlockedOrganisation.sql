CREATE TABLE dbo.BlockedOrganisation (
   [Id]	                      uniqueidentifier NOT NULL,
   [OrganisationId]           nvarchar(20) NOT NULL,
   [OrganisationType]         nvarchar(20) NOT NULL,
   [BlockedStatus]            nvarchar(20) NOT NULL,
   [Reason]                   nvarchar(max) NOT NULL,
   [UpdatedByUserId]          nvarchar(255) NOT NULL,
   [UpdatedByUserEmail]       nvarchar(255) NOT NULL,
   [UpdatedDate]              DATETIME NOT NULL,
   CONSTRAINT [PK_BlockedOrganisation] PRIMARY KEY (Id),
   INDEX [IX_BlockedOrganisation_OrganisationId] NONCLUSTERED(OrganisationId) INCLUDE(Id, UpdatedByUserId, UpdatedDate, BlockedStatus, OrganisationType, Reason)
)