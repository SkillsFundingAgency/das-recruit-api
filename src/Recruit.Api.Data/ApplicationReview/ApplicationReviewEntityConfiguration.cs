﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruit.Api.Domain.Entities;

namespace Recruit.Api.Data.ApplicationReview;

public class ApplicationReviewEntityConfiguration : IEntityTypeConfiguration<ApplicationReviewEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationReviewEntity> builder)
    {
        builder.ToTable("ApplicationReview");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("Id").HasColumnType("uniqueidentifier").IsRequired();
        builder.Property(x => x.Ukprn).HasColumnName("Ukprn").HasColumnType("int").IsRequired();
        builder.Property(x => x.AccountId).HasColumnName("AccountId").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.AccountLegalEntityId).HasColumnName("AccountLegalEntityId").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.Owner).HasColumnName("Owner").HasColumnType("tinyint").IsRequired().HasConversion<int>();
        builder.Property(x => x.CandidateFeedback).HasColumnName("CandidateFeedback").HasColumnType("nvarchar(max)");
        builder.Property(x => x.EmployerFeedback).HasColumnName("EmployerFeedback").HasColumnType("nvarchar(max)");
        builder.Property(x => x.CandidateId).HasColumnName("CandidateId").HasColumnType("uniqueidentifier").IsRequired();
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").HasColumnType("DateTime").IsRequired();
        builder.Property(x => x.DateSharedWithEmployer).HasColumnName("DateSharedWithEmployer").HasColumnType("DateTime");
        builder.Property(x => x.HasEverBeenEmployerInterviewing).HasColumnName("HasEverBeenEmployerInterviewing").HasColumnType("bit").IsRequired().HasDefaultValue(0);
        builder.Property(x => x.WithdrawnDate).HasColumnName("WithdrawnDate").HasColumnType("DateTime");
        builder.Property(x => x.ReviewedDate).HasColumnName("ReviewedDate").HasColumnType("DateTime");
        builder.Property(x => x.SubmittedDate).HasColumnName("SubmittedDate").HasColumnType("DateTime").IsRequired();
        builder.Property(x => x.Status).HasColumnName("Status").HasColumnType("nvarchar(50)").IsRequired();
        builder.Property(x => x.StatusUpdatedDate).HasColumnName("StatusUpdatedDate").HasColumnType("DateTime");
        builder.Property(x => x.VacancyReference).HasColumnName("VacancyReference").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.LegacyApplicationId).HasColumnName("LegacyApplicationId").HasColumnType("uniqueidentifier");
        builder.Property(x => x.ApplicationId).HasColumnName("ApplicationId").HasColumnType("uniqueidentifier");
        builder.Property(x => x.AdditionalQuestion1).HasColumnName("AdditionalQuestion1").HasColumnType("nvarchar(500)");
        builder.Property(x => x.AdditionalQuestion2).HasColumnName("AdditionalQuestion2").HasColumnType("nvarchar(500)");
        builder.Property(x => x.VacancyTitle).HasColumnName("VacancyTitle").HasColumnType("nvarchar(500)").IsRequired();
    }
}