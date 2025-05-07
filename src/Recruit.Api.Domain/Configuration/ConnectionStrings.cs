using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Recruit.Api.Domain.Configuration;

[ExcludeFromCodeCoverage]
public class ConnectionStrings
{
    public required string SqlConnectionString { get; set; }
}