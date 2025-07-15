namespace SFA.DAS.Recruit.Api.Data;

public abstract class RecruitDataException(string? message, string? detail): Exception(message)
{
    public string? Detail => detail;
}