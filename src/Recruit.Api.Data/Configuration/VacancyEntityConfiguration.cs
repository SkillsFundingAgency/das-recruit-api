using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Data.Configuration;

[ExcludeFromCodeCoverage]
public class VacancyEntityConfiguration : IEntityTypeConfiguration<VacancyEntity>
{
    public void Configure(EntityTypeBuilder<VacancyEntity> builder)
    {
        builder.ToTable("Vacancy");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.VacancyReference).HasConversion<long?>().ValueGeneratedNever();
        builder.Property(x => x.Status).HasConversion(v => v.ToString(), v => Enum.Parse<VacancyStatus>(v)).IsRequired();
        builder.Property(x => x.OwnerType).HasConversion(v => v.ToString(), v => Enum.Parse<OwnerType>(v)).IsRequired(false);
        builder.Property(x => x.SourceOrigin).HasConversion(v => v.ToString(), v => Enum.Parse<SourceOrigin>(v)).IsRequired(false);
        builder.Property(x => x.SourceType).HasConversion(v => v.ToString(), v => Enum.Parse<SourceType>(v)).IsRequired(false);
        builder.Property(x => x.ApplicationMethod).HasConversion(v => v.ToString(), v => Enum.Parse<ApplicationMethod>(v!)).IsRequired(false);
        builder.Property(x => x.EmployerLocationOption).HasConversion(v => v.ToString(), v => Enum.Parse<AvailableWhere>(v!)).IsRequired(false);
        builder.Property(x => x.EmployerNameOption).HasConversion(v => v.ToString(), v => Enum.Parse<EmployerNameOption>(v!)).IsRequired(false);
        builder.Property(x => x.GeoCodeMethod).HasConversion(v => v.ToString(), v => Enum.Parse<GeoCodeMethod>(v!)).IsRequired(false);
        builder.Property(x => x.ApprenticeshipType).HasConversion(v => v.ToString(), v => Enum.Parse<ApprenticeshipTypes>(v)).IsRequired(false);
        builder.Property(x => x.Wage_DurationUnit).HasConversion(v => v.ToString(), v => Enum.Parse<DurationUnit>(v!)).IsRequired(false);
        builder.Property(x => x.Wage_WageType).HasConversion(v => v.ToString(), v => Enum.Parse<WageType>(v!)).IsRequired(false);
        builder.Property(x => x.ClosureReason).HasConversion(v => v.ToString(), v => Enum.Parse<ClosureReason>(v!)).IsRequired(false);
        builder.Property(x => x.Wage_FixedWageYearlyAmount).HasColumnType("decimal").IsRequired(false);
        builder.Property(x => x.Wage_WeeklyHours).HasColumnType("decimal").IsRequired(false);
    }
}