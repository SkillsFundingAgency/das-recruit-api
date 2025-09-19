using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Configuration;

[ExcludeFromCodeCoverage]
internal class EmployerProfileEntityConfiguration : IEntityTypeConfiguration<EmployerProfileEntity>
{
    public void Configure(EntityTypeBuilder<EmployerProfileEntity> builder)
    {
        builder.ToTable("EmployerProfile");
        builder
            .HasMany(x => x.Addresses)
            .WithOne(x => x.EmployerProfile)
            .HasForeignKey(x => x.AccountLegalEntityId)
            .HasPrincipalKey(x => x.AccountLegalEntityId);
    }
}