using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.EmployerProfile;

[ExcludeFromCodeCoverage]
public class EmployerProfileAddressEntityConfiguration : IEntityTypeConfiguration<EmployerProfileAddressEntity>
{
    public void Configure(EntityTypeBuilder<EmployerProfileAddressEntity> builder)
    {
        builder.ToTable("EmployerProfileAddress");
    }
}