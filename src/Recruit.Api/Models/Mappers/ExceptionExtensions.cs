using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Data;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class ExceptionExtensions
{
    public static ProblemDetails ToProblemDetails(this RecruitDataException ex)
    {
        return new ProblemDetails {
            Title = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Detail = ex.Detail,
        };
    }
}