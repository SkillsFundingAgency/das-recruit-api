using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

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
        builder.Property(x => x.NotificationPreferences).HasConversion<NotificationPreferencesConverter>();
    }
}

/// <summary>
/// Using a ValueConverter allows us to process null values
/// </summary>
public class NotificationPreferencesConverter() : ValueConverter<NotificationPreferences, string?>(
    x => JsonSerializer.Serialize(x, Options),
    x => x == null ? new NotificationPreferences() : JsonSerializer.Deserialize<NotificationPreferences>(x, Options)!)
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    public override bool ConvertsNulls => true;
}