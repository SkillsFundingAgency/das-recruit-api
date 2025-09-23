using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Extensions;

public static class ApplicationReviewEntityExtensions
{
    public static int New(this List<ApplicationReviewEntity> entities)
    {
        return entities.Count(fil => fil is {Status: ApplicationReviewStatus.New, WithdrawnDate: null});
    }

    public static int Shared(this List<ApplicationReviewEntity> entities)
    {
        return entities.Count(e => e is { Status: ApplicationReviewStatus.Shared, WithdrawnDate: null });
    }

    public static int AllShared(this List<ApplicationReviewEntity> entities)
    {
        var defaultDate = new DateTime(1900, 1, 1, 1, 0, 0, 389, DateTimeKind.Utc);
        return entities.Count(e =>
            e is { Status: ApplicationReviewStatus.Shared, WithdrawnDate: null, DateSharedWithEmployer: not null } &&
            e.DateSharedWithEmployer > defaultDate);
    }

    public static int Successful(this List<ApplicationReviewEntity> entities)
    {
        return entities.Count(e => e is { Status: ApplicationReviewStatus.Successful, WithdrawnDate: null });
    }

    public static int Unsuccessful(this List<ApplicationReviewEntity> entities)
    {
        return entities.Count(e => e is { Status: ApplicationReviewStatus.Unsuccessful, WithdrawnDate: null });
    }

    public static int EmployerReviewed(this List<ApplicationReviewEntity> entities)
    {
        return entities.Count(e =>
            e is { Status: ApplicationReviewStatus.EmployerUnsuccessful or ApplicationReviewStatus.EmployerInterviewing, WithdrawnDate: null });
    }
    public static bool HasNoApplications(this List<ApplicationReviewEntity> entities)
    {
        return entities.All(e => e.WithdrawnDate != null);
    }
    public static int AllCount(this List<ApplicationReviewEntity> entities)
    {
        return entities.Count(e => e.WithdrawnDate == null);
    }
}
