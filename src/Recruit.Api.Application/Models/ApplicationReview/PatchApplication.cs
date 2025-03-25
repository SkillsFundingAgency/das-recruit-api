using Microsoft.AspNetCore.JsonPatch;

namespace Recruit.Api.Application.Models.ApplicationReview;

public record PatchApplication
{
    public required Guid Id { get; init; }
    public required JsonPatchDocument<Domain.Entities.ApplicationReview> Patch { get; init; }
}