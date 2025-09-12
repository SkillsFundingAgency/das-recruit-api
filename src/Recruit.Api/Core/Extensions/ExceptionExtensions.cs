using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core.Exceptions;

namespace SFA.DAS.Recruit.Api.Core.Extensions;

public static class ExceptionExtensions
{
    public static IResult ToResponse(this MissingEmailStrategyException ex)
    {
        return TypedResults.Problem(new ProblemDetails {
            Status = StatusCodes.Status501NotImplemented,
            Title = "Missing email handler",
            Detail = ex.Message
        });
    }
    public static IResult ToResponse(this DataIntegrityException ex)
    {
        return TypedResults.Problem(new ProblemDetails {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Data integrity error",
            Detail =
                "Could not process request, there was a data integrity failure. See the internal api logs for more details."
        });
    }
}