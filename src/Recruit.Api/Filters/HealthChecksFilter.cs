﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.Recruit.Api.Filters;

[ExcludeFromCodeCoverage]
public class HealthChecksFilter : IDocumentFilter
{
    private const string HealthCheckEndpoint = @"/health";

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var pathItem = new OpenApiPathItem();
        var operation = new OpenApiOperation();
        operation.Tags.Add(new OpenApiTag { Name = "Service Status" });

        var healthyResponse = new OpenApiResponse();
        healthyResponse.Content.Add("text/plain", new OpenApiMediaType {
            Schema = new OpenApiSchema {
                Type = "string",
                Enum = [
                    new OpenApiString("Healthy"),
                ]
            }
        });
        operation.Responses.Add("200", healthyResponse);

        var unhealthyResponse = new OpenApiResponse();
        unhealthyResponse.Content.Add("text/plain", new OpenApiMediaType {
            Schema = new OpenApiSchema {
                Type = "string",
                Enum = [
                    new OpenApiString("Unhealthy"),
                ]
            }
        });

        operation.Responses.Add("503", unhealthyResponse);
        pathItem.AddOperation(OperationType.Get, operation);
        swaggerDoc?.Paths.Add(HealthCheckEndpoint, pathItem);
    }
}