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

    Task<DashboardModel> GetCountByAccountId(long accountId, CancellationToken token = default);

    Task<DashboardModel> GetCountByUkprn(int ukprn, CancellationToken token = default);

    Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default);

    Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default);

    Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByAccountId(long accountId, List<long> vacancyReferences, CancellationToken token = default);
    Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByUkprn(int ukprn, List<long> vacancyReferences, CancellationToken token = default);
    Task<ApplicationReviewEntity?> GetByApplicationId(Guid applicationId, CancellationToken token = default);
}

internal class ApplicationReviewsProvider(IApplicationReviewRepository repository) : IApplicationReviewsProvider
{
    public async Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default)
    {
        return await repository.GetById(id, token);
    }
    
    public async Task<ApplicationReviewEntity?> GetByApplicationId(Guid applicationId, CancellationToken token = default)
    {
        return await repository.GetByApplicationId(applicationId, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllByAccountId(long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default)
    {
        return await repository.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllByUkprn(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default)
    {
        return await repository.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<DashboardModel> GetCountByAccountId(long accountId, CancellationToken token = default)
    {
        var applicationReviews = await repository.GetAllByAccountId(accountId, token);

        return GetDashboardModel(applicationReviews);
    }

    public async Task<DashboardModel> GetCountByUkprn(int ukprn, CancellationToken token = default)
    {
        var applicationReviews = await repository.GetAllByUkprn(ukprn, token);

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

        return GetApplicationReviewsStats(vacancyReferences, applicationReviews);
    }

    public async Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByUkprn(int ukprn, List<long> vacancyReferences,
        CancellationToken token = default)
    {
        var applicationReviews = await repository.GetAllByUkprn(ukprn, vacancyReferences, token);

        return GetApplicationReviewsStats(vacancyReferences, applicationReviews);
    }

    private static DashboardModel GetDashboardModel(List<ApplicationReviewEntity> applicationReviews)
    {
        return new DashboardModel
        {
            NewApplicationsCount = applicationReviews.Count(fil =>
                fil is {Status: nameof(ApplicationReviewStatus.New), WithdrawnDate: null}),
            EmployerReviewedApplicationsCount = applicationReviews.Count(entity =>
                entity.Status is nameof(ApplicationReviewStatus.EmployerUnsuccessful) or nameof(ApplicationReviewStatus.EmployerInterviewing)),
        };
    }

    private static List<ApplicationReviewsStats> GetApplicationReviewsStats(List<long> vacancyReferences, List<ApplicationReviewEntity> applicationReviews)
    {
        return vacancyReferences
            .GroupJoin(
                applicationReviews,
                vacancyRef => vacancyRef,
                review => review.VacancyReference,
                (vacancyRef, reviews) =>
                {
                    var applicationReviewEntities = reviews.ToList();

                    return new ApplicationReviewsStats {
                        VacancyReference = vacancyRef,
                        NewApplications = applicationReviewEntities.Count(e =>
                            e is {Status: nameof(ApplicationReviewStatus.New), WithdrawnDate: null}),
                        SharedApplications = applicationReviewEntities.Count(e =>
                            e is {Status: nameof(ApplicationReviewStatus.Shared), WithdrawnDate: null}),
                        SuccessfulApplications = applicationReviewEntities.Count(e =>
                            e is {Status: nameof(ApplicationReviewStatus.Successful), WithdrawnDate: null}),
                        UnsuccessfulApplications = applicationReviewEntities.Count(e =>
                            e is {Status: nameof(ApplicationReviewStatus.Unsuccessful), WithdrawnDate: null}),
                        EmployerReviewedApplications = applicationReviewEntities.Count(e =>
                            (e.Status is nameof(ApplicationReviewStatus.EmployerUnsuccessful) or
                                nameof(ApplicationReviewStatus.EmployerInterviewing)) &&
                            e.WithdrawnDate == null),
                        Applications = applicationReviewEntities.Count(e => e.WithdrawnDate == null),
                        HasNoApplications = applicationReviewEntities.All(e => e.WithdrawnDate != null)
                    };
                })
            .ToList();
    }
}