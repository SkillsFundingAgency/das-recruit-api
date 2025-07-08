using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Recruit.Api.Models.Requests;

public class PagingParams
{
    [FromQuery] public ushort? Page { get; init; } = null;
    [FromQuery] public ushort? PageSize { get; init; } = null;
}