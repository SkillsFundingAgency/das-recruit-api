namespace SFA.DAS.Recruit.Api.Models.Requests.User;

public sealed record GetUserRequest
{
    public required string Email { get; set; }
    public required UserType UserType { get; set; }
}