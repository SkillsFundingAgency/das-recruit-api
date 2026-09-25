using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Recruit.Api.Domain.Configuration;

[ExcludeFromCodeCoverage]
public class BlobStorageConfiguration
{
    public required string ConnectionString { get; set; }
}
