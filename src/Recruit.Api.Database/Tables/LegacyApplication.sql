﻿CREATE TABLE dbo.[LegacyApplication] (
    [Id]					            uniqueidentifier NOT NULL,
    [CandidateId]                       uniqueidentifier NOT NULL,
    [AddressLine1]                      NVARCHAR(500) NULL,
    [AddressLine2]                      NVARCHAR(500) NULL,
    [AddressLine3]                      NVARCHAR(500) NULL,
    [AddressLine4]                      NVARCHAR(500) NULL,
    [ApplicationDate]                   DATETIME NULL,
    [BirthDate]                         DATETIME NULL,
    [ApplicationReviewDisabilityStatus] NVARCHAR(50) NOT NULL,
    [EducationFromYear]                 INT NULL,
    [EducationInstitution]              NVARCHAR(500) NULL,
    [EducationToYear]                   INT NULL,
    [Email]                             NVARCHAR(255) NOT NULL,
    [FirstName]                         NVARCHAR(150) NOT NULL,
    [HobbiesAndInterests]               NVARCHAR(max) NULL,
    [Improvements]                      NVARCHAR(max) NULL,
    [LastName]                          NVARCHAR(150) NOT NULL,
    [Phone]                             NVARCHAR(50) NULL,
    [Postcode]                          NVARCHAR(50) NULL,
    [Skills]                            NVARCHAR(MAX) NULL,
    [Strengths]                         NVARCHAR(MAX) NULL,
    [Support]                           NVARCHAR(MAX) NULL,
    [Qualifications]                    NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_LegacyApplication] PRIMARY KEY (Id),
)