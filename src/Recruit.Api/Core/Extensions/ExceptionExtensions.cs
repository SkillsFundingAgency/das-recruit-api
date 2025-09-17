using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core.Exceptions;
using NotSupportedException = SFA.DAS.Recruit.Api.Core.Exceptions.NotSupportedException;

namespace SFA.DAS.Recruit.Api.Core.Extensions;

public static class ExceptionExtensions
{
    public static IResult ToResponse(this NotSupportedException ex)
    {
        return TypedResults.Problem(new ProblemDetails {
            Status = StatusCodes.Status501NotImplemented,
            Title = "The request could not be completed",
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