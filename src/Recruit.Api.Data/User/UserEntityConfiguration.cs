using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Data.User;

[ExcludeFromCodeCoverage]
public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder
            .ToTable("User")
            .HasMany(x => x.EmployerAccounts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserType).HasConversion(v => v.ToString(), v => (UserType)Enum.Parse(typeof(UserType), v));
    }
}