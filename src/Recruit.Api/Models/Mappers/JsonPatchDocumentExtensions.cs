using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class JsonPatchDocumentExtensions
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
}