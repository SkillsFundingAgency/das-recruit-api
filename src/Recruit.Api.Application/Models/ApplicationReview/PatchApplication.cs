using Microsoft.AspNetCore.JsonPatch;
using Recruit.Api.Domain.Entities;

namespace Recruit.Api.Application.Models.ApplicationReview;

public record PatchApplication
{
    public required Guid Id { get; init; }
    public required JsonPatchDocument<PatchApplicationReview> Patch { get; init; }
}