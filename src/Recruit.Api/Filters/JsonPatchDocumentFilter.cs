using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.Recruit.Api.Filters;

[ExcludeFromCodeCoverage]
public class JsonPatchDocumentFilter: IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var patchOperation = swaggerDoc.Components.Schemas.AsEnumerable()
            .FirstOrDefault(s => s.Key.ToLower() == "operation");

        if (patchOperation.Key != null)
            patchOperation.Value.Properties.Remove("operationType");
    }
}