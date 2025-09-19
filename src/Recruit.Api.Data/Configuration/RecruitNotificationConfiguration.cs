using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Configuration;

[ExcludeFromCodeCoverage]
public class RecruitNotificationConfiguration : IEntityTypeConfiguration<RecruitNotificationEntity>
{
    public void Configure(EntityTypeBuilder<RecruitNotificationEntity> builder)
    {
        builder.ToTable("RecruitNotification");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}