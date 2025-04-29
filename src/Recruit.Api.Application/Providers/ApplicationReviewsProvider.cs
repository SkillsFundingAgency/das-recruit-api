using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Application.Providers;

public interface IApplicationReviewsProvider
{
    Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default);

    Task<PaginatedList<ApplicationReviewEntity>> GetAllByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default);

    Task<PaginatedList<ApplicationReviewEntity>> GetAllByUkprn(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default);

    Task<DashboardModel> GetCountByAccountId(long accountId, ApplicationReviewStatus status, CancellationToken token = default);

    Task<DashboardModel> GetCountByUkprn(int ukprn, ApplicationReviewStatus status, CancellationToken token = default);

    Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default);

    Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default);

    Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByAccountId(long accountId, List<long> vacancyReferences, CancellationToken token = default);
    Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByUkprn(int ukprn, List<long> vacancyReferences, CancellationToken token = default);
}

internal class ApplicationReviewsProvider(IApplicationReviewRepository repository) : IApplicationReviewsProvider
{
    public async Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default)
    {
        return await repository.GetById(id, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllByAccountId(long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        CancellationToken token = default)
    {
        return await repository.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllByUkprn(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = "CreatedDate",
        bool isAscending = false, CancellationToken token = default)
    {
        return await repository.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<DashboardModel> GetCountByAccountId(long accountId, ApplicationReviewStatus status, CancellationToken token = default)
    {
        var applicationReviews = await repository.GetAllByAccountId(accountId, status.ToString(), token);

        return GetDashboardModel(applicationReviews);
    }

    public async Task<DashboardModel> GetCountByUkprn(int ukprn, ApplicationReviewStatus status, CancellationToken token = default)
    {
        var applicationReviews = await repository.GetAllByUkprn(ukprn, status.ToString(), token);

        return GetDashboardModel(applicationReviews);
    }

    public async Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        return await repository.Update(entity, token);
    }

    public async Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        return await repository.Upsert(entity, token);
    }

    public async Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByAccountId(long accountId, List<long> vacancyReferences,
        CancellationToken token = default)
    {
        var applicationReviews = await repository.GetAllByAccountId(accountId, vacancyReferences, token);

        return GetApplicationReviewsStats(applicationReviews);
    }

    public async Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByUkprn(int ukprn, List<long> vacancyReferences,
        CancellationToken token = default)
    {
        var applicationReviews = await repository.GetAllByUkprn(ukprn, vacancyReferences, token);

        return GetApplicationReviewsStats(applicationReviews);
    }

    private static DashboardModel GetDashboardModel(List<ApplicationReviewEntity> applicationReviews)
    {
        return new DashboardModel
        {
            NewApplicationsCount = applicationReviews.Count(fil => fil.Status == ApplicationReviewStatus.New.ToString() && fil.WithdrawnDate is null),
            EmployerReviewedApplicationsCount = applicationReviews.Count(fil => fil is { ReviewedDate: not null, WithdrawnDate: null}),
        };
    }

    private static List<ApplicationReviewsStats> GetApplicationReviewsStats(List<ApplicationReviewEntity> applicationReviews)
    {
        return applicationReviews
            .Where(fil => fil.WithdrawnDate is null)
            .GroupBy(fil => fil.VacancyReference)
            .Select(fil => new ApplicationReviewsStats
            {
                VacancyReference = fil.Key,
                NewApplications = fil.Count(entity => entity.Status == ApplicationReviewStatus.New.ToString()),
                SharedApplications = fil.Count(entity => entity.Status == ApplicationReviewStatus.Shared.ToString()),
                SuccessfulApplications = fil.Count(entity => entity.Status == ApplicationReviewStatus.Successful.ToString()),
                UnsuccessfulApplications = fil.Count(entity => entity.Status == ApplicationReviewStatus.Unsuccessful.ToString()),
                EmployerReviewedApplications = fil.Count(entity => entity.DateSharedWithEmployer != null || entity.Status == ApplicationReviewStatus.InReview.ToString()),
                Applications = fil.Count()
            })
            .ToList();
    }
}