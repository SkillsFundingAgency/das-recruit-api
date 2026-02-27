using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Configuration;

[ExcludeFromCodeCoverage]
internal class BlockedOrganisationEntityConfiguration : IEntityTypeConfiguration<BlockedOrganisationEntity>
{
    public void Configure(EntityTypeBuilder<BlockedOrganisationEntity> builder)
    {
        builder.ToTable("BlockedOrganisation");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrganisationId).IsRequired().HasMaxLength(20);
        builder.Property(x => x.OrganisationType).IsRequired().HasMaxLength(20);
        builder.Property(x => x.BlockedStatus).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Reason).IsRequired();
        builder.Property(x => x.UpdatedByUserId).IsRequired().HasMaxLength(255);
        builder.Property(x => x.UpdatedByUserEmail).IsRequired().HasMaxLength(255);
        builder.Property(x => x.UpdatedDate).IsRequired().HasColumnType("datetime");

        builder.HasIndex(x => x.OrganisationId)
            .HasDatabaseName("IX_BlockedOrganisation_OrganisationId")
            .IncludeProperties(x => new
            {
                x.Id,
                x.UpdatedByUserId,
                x.UpdatedDate,
                x.BlockedStatus,
                x.OrganisationType,
                x.Reason
            })
            .IsUnique(false);
    }
}
