using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.ProhibitedContent;

[ExcludeFromCodeCoverage]
public class ProhibitedContentEntityConfiguration : IEntityTypeConfiguration<ProhibitedContentEntity>
{
    public void Configure(EntityTypeBuilder<ProhibitedContentEntity> builder)
    {
        builder.ToTable("ProhibitedContent");
        builder.HasKey(x => new { x.ContentType, x.Content });
        builder.Property(x => x.ContentType).HasColumnName("ContentType").HasColumnType("tinyint").HasConversion<int>().IsRequired();
        builder.Property(x => x.Content).HasColumnName("Content").HasColumnType("nvarchar(128)").IsRequired();
    }
}