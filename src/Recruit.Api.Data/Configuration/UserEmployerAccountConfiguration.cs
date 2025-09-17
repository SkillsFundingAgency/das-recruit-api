using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Configuration;

[ExcludeFromCodeCoverage]
public class UserEmployerAccountConfiguration : IEntityTypeConfiguration<UserEmployerAccountEntity>
{
    public void Configure(EntityTypeBuilder<UserEmployerAccountEntity> builder)
    {
        builder
            .ToTable("UserEmployerAccount")
            .HasOne(x => x.User)
            .WithMany(x => x.EmployerAccounts)
            .HasForeignKey(x => x.UserId);
        
        builder.HasKey(x => new { x.UserId, x.EmployerAccountId });
    }
}