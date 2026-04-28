using System.Text.RegularExpressions;
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
    
    public static JsonPatchDocument<TEntity> ToDomain<TSource, TEntity>(this JsonPatchDocument<TSource> source) where TEntity : class where TSource : class
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
    
    public static JsonPatchDocument<TEntity> ToDomain<TSource, TEntity>(this JsonPatchDocument<TSource> source, object id, Dictionary<string, Func<object, Operation<TSource>, Operation<TEntity>>> fieldMappings) where TEntity : class where TSource : class
    {
        var pathRegex = new Regex(@"^\/(?<property>[a-zA-Z]*)[\/-]?", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(2));
        var pathReplaceRegex = new Regex(@"([\w]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(2));
        var result = new JsonPatchDocument<TEntity>();
        var operations = source.Operations.Select(x =>
        {
            var match = pathRegex.Match(x.path);
            string queryField = match.Success ? match.Groups["property"].Value : x.path; 

            if (fieldMappings.TryGetValue(queryField, out Func<object, Operation<TSource>, Operation<TEntity>>? mapping))
            {
                var mappedOperation = mapping(id, x);
                mappedOperation.path = pathReplaceRegex.Replace(x.path, mappedOperation.path);
                return mappedOperation;    
            }

            return new Operation<TEntity> {
                from = x.from,
                op = x.op,
                value = x.value,
                path = x.path,
            };
        });
            
        result.Operations.AddRange(operations);
        return result;
    }

    public static void ThrowIfOperationsOn<TEntity>(this JsonPatchDocument<TEntity> document, IEnumerable<string> paths) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(document);

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