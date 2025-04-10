using Microsoft.AspNetCore.JsonPatch.Exceptions;

namespace SFA.DAS.Recruit.Api.Extensions;

public static class JsonPatchExceptionExtensions
{
    public static IDictionary<string, string[]> ToProblemsDictionary(this JsonPatchException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return new Dictionary<string, string[]>
        {
            {exception.FailedOperation.path, [exception.Message]}
        };
    }
}