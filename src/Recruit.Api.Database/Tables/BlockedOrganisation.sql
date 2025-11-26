CREATE TABLE dbo.BlockedOrganisation (
   [Id]	                      uniqueidentifier NOT NULL,
   [OrganisationId]           varchar(20) NOT NULL,
   [OrganisationType]         varchar(20) NOT NULL,
   [BlockedStatus]            varchar(20) NOT NULL,
   [Reason]                   varchar(max) NOT NULL,
   [UpdatedByUserId]          uniqueidentifier NOT NULL,
   [UpdatedDate]              DATETIME NOT NULL,
   CONSTRAINT [PK_BlockedOrganisation] PRIMARY KEY (Id),
   CONSTRAINT [FK_BlockedOrganisation_User] FOREIGN KEY (UpdatedByUserId) REFERENCES [User](Id),
   INDEX [IX_BlockedOrganisation_OrganisationId] NONCLUSTERED(OrganisationId) INCLUDE(Id, UpdatedByUserId, UpdatedDate, BlockedStatus, OrganisationType, Reason)
)