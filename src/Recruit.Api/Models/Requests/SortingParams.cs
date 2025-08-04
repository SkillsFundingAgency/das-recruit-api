using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Models.Requests;

public class SortingParams<T>
{
    [FromQuery] public SortOrder? SortOrder { get; init; }
    [FromQuery] public T? SortColumn { get; init; }
}