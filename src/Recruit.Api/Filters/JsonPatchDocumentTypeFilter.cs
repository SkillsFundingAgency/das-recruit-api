using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.Recruit.Api.Filters;

[ExcludeFromCodeCoverage]
public class JsonPatchDocumentTypeFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!string.Equals(context.ApiDescription.HttpMethod, "patch", StringComparison.OrdinalIgnoreCase))
            return;

        var bodyParam = context.ApiDescription.ParameterDescriptions
            .FirstOrDefault(p => p.Source.Id == "Body");

        if (bodyParam?.Type is not { IsGenericType: true } paramType)
            return;

        var def = paramType.GetGenericTypeDefinition();
        if (!typeof(JsonPatchDocument<>).IsAssignableFrom(def))
            return;

        var innerType = paramType.GetGenericArguments().FirstOrDefault();
        if (innerType != null)
            operation.Extensions["x-patch-document-type"] = new OpenApiString(innerType.Name);
    }
}
