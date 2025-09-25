using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Configuration;

[ExcludeFromCodeCoverage]
public class VacancyEntityConfiguration : IEntityTypeConfiguration<VacancyEntity>
{
    public void Configure(EntityTypeBuilder<VacancyEntity> builder)
    {
        builder.ToTable("Vacancy");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.VacancyReference).HasConversion<long>().ValueGeneratedNever();
        builder.Property(x => x.Status).HasConversion(v => v.ToString(), v => Enum.Parse<VacancyStatus>(v));
        builder.Property(x => x.OwnerType).HasConversion(v => v.ToString(), v => Enum.Parse<OwnerType>(v));
        builder.Property(x => x.SourceOrigin).HasConversion(v => v.ToString(), v => Enum.Parse<SourceOrigin>(v));
        builder.Property(x => x.SourceType).HasConversion(v => v.ToString(), v => Enum.Parse<SourceType>(v));
        //builder.Property(x => x.ApplicationMethod).HasConversion(v => v.ToString(), v => Enum.Parse<ApplicationMethod>(v!));
        builder.Property<ApplicationMethod>(x => x.ApplicationMethod).HasConversion<EnumConverter>();
        builder.Property(x => x.EmployerLocationOption).HasConversion(v => v.ToString(), v => Enum.Parse<AvailableWhere>(v!));
        builder.Property(x => x.EmployerNameOption).HasConversion(v => v.ToString(), v => Enum.Parse<EmployerNameOption>(v!));
        builder.Property(x => x.GeoCodeMethod).HasConversion(v => v.ToString(), v => Enum.Parse<GeoCodeMethod>(v!));
        builder.Property(x => x.ApprenticeshipType).HasConversion(v => v.ToString(), v => Enum.Parse<ApprenticeshipTypes>(v));
        builder.Property(x => x.Wage_DurationUnit).HasConversion(v => v.ToString(), v => Enum.Parse<DurationUnit>(v!));
        builder.Property(x => x.Wage_WageType).HasConversion(v => v.ToString(), v => Enum.Parse<WageType>(v!));
        builder.Property(x => x.ClosureReason).HasConversion(v => v.ToString(), v => Enum.Parse<ClosureReason>(v!));
        builder.Property(x => x.Wage_FixedWageYearlyAmount).HasColumnType("decimal");
        builder.Property(x => x.Wage_WeeklyHours).HasColumnType("decimal");
    }

    /// <summary>
    /// Using a ValueConverter allows us to process null values
    /// </summary>
    public class EnumConverter() : ValueConverter<ApplicationMethod?, string?>(
        static x => JsonSerializer.Serialize(x, Options),
        static x => JsonSerializer.Deserialize<ApplicationMethod>(x ?? "{}", Options)!)
    {
        private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        public override bool ConvertsNulls => true;
    }
}