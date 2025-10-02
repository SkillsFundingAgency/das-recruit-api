using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Models.Requests;

public record FilteringParams
{
    [FromQuery] public FilteringOptions FilterBy { get; init; } = FilteringOptions.All;
    [FromQuery] public string? SearchTerm { get; init; } = string.Empty;
}