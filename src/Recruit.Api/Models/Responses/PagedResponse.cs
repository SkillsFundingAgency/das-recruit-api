namespace SFA.DAS.Recruit.Api.Models.Responses;

public record PagedResponse<T>(PageInfo PageInfo, IEnumerable<T> Items);