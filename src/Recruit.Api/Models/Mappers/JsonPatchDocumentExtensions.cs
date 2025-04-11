using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

internal static class JsonPatchDocumentExtensions
{
    public static JsonPatchDocument<TEntity> ToDomain<TEntity>(this JsonPatchDocument source) where TEntity : class
    {
        var result = new JsonPatchDocument<TEntity>();
        var operations = source.Operations.Select(x => new Operation<TEntity>
        {
            from = x.from,
            op = x.op,
            value = x.value,
            path = x?.path
        });
            
        result.Operations.AddRange(operations);
        return result;
    }

    public static void ThrowIfOperationsOn<TEntity>(this JsonPatchDocument<TEntity> document, IEnumerable<string> paths) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(document, nameof(document));

        foreach (string path in paths)
        {
            var operation = document.Operations.FirstOrDefault(x => x.path.Equals($"/{path}", StringComparison.OrdinalIgnoreCase));
            if (operation is not null)
            {
                throw new JsonPatchException(new JsonPatchError(null, operation, $"Operations on the specified path '{operation.path}' are not permitted."));
            }
        }
    }
}