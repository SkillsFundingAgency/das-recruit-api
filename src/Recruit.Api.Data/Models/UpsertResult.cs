namespace SFA.DAS.Recruit.Api.Data.Models;

public record UpsertResult<T>(T Entity, bool Created);

public static class UpsertResult
{
    public static UpsertResult<TResult> Create<TResult>(TResult entity, bool created)
    {
        return new UpsertResult<TResult>(entity, created);
    }
}