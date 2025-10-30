using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Data.Configuration;

[ExcludeFromCodeCoverage]
internal class ReportEntityConfiguration : IEntityTypeConfiguration<ReportEntity>
{
    public void Configure(EntityTypeBuilder<ReportEntity> builder)
    {
        builder.ToTable("Report");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("Id").HasColumnType("uniqueidentifier").IsRequired();
        builder.Property(x => x.UserId).HasColumnName("UserId").HasColumnType("nvarchar(100)").IsRequired();
        builder.Property(x => x.Name).IsRequired().HasColumnName("Name").HasColumnType("nvarchar(100)");
        builder.Property(x => x.Type).HasConversion(v => v.ToString(), v => Enum.Parse<ReportType>(v)).IsRequired();
        builder.Property(x => x.OwnerType).HasConversion(v => v.ToString(), v => Enum.Parse<ReportOwnerType>(v)).IsRequired();
        builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").HasColumnType("DateTime").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").HasColumnType("nvarchar(50)");
        builder.Property(x => x.DynamicCriteria).HasColumnName("DynamicCriteria").HasColumnType("nvarchar(max)").IsRequired();

        builder
            .HasIndex(a => new { a.UserId })
            .HasDatabaseName("IX_PK_Report_UserId");
    }
}
