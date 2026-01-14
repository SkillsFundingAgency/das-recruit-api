using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Configuration;
public class VacancyAnalyticsEntityConfiguration : IEntityTypeConfiguration<VacancyAnalyticsEntity>
{
    public void Configure(EntityTypeBuilder<VacancyAnalyticsEntity> builder)
    {
        builder.ToTable("VacancyAnalytics");
        builder.HasKey(x => x.VacancyReference);
        builder.Property(x => x.VacancyReference).HasColumnName("VacancyReference").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.UpdatedDate).HasColumnName("UpdatedDate").HasColumnType("datetime").IsRequired();
        builder.Property(x => x.Analytics).HasColumnName("Analytics").HasColumnType("nvarchar(max)");
    }
}