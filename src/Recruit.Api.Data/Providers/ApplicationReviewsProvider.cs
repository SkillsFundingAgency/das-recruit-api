using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Providers;

public interface IApplicationReviewsProvider
{
    Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default);

    Task<PaginatedList<ApplicationReviewEntity>> GetPagedAccountIdAsync(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default);

    Task<PaginatedList<ApplicationReviewEntity>> GetPagedUkprnAsync(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default);

    Task<DashboardModel> GetCountByAccountId(long accountId, CancellationToken token = default);

    Task<DashboardModel> GetCountByUkprn(int ukprn, CancellationToken token = default);

    Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default);

    Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default);

    Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByAccountId(long accountId,
        List<long> vacancyReferences,
        ApplicationReviewStatus? applicationSharedFilteringStatus = null,
        CancellationToken token = default);

    Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByUkprn(int ukprn,
        List<long> vacancyReferences,
        CancellationToken token = default);

    Task<List<ApplicationReviewEntity>> GetAllByVacancyReference(long vacancyReference, CancellationToken token = default);

    Task<ApplicationReviewEntity?> GetByApplicationId(Guid applicationId, CancellationToken token = default);

    Task<PaginatedList<VacancyDetail>> GetPagedByAccountAndStatusAsync(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        List<ApplicationReviewStatus>? status = null,
        CancellationToken token = default);

    Task<PaginatedList<VacancyDetail>> GetPagedAllSharedByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default);

    Task<PaginatedList<VacancyDetail>> GetPagedByUkprnAndStatusAsync(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        List<ApplicationReviewStatus>? status = null,
        CancellationToken token = default);

    Task<List<ApplicationReviewEntity>> GetAllByIdAsync(List<Guid> ids, CancellationToken token = default);
    Task<ApplicationReviewEntity?> GetByVacancyReferenceAndCandidateId(long vacancyReference, Guid candidateId, CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetAllByVacancyReferenceAndStatus(long vacancyReference, ApplicationReviewStatus status, bool isTempStatus,
        CancellationToken token = default);
}

internal class ApplicationReviewsProvider(
    IApplicationReviewRepository applicationReviewRepository) : IApplicationReviewsProvider
{
    public async Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default)
    {
        return await applicationReviewRepository.GetById(id, token);
    }

    public async Task<ApplicationReviewEntity?> GetByApplicationId(Guid applicationId, CancellationToken token = default)
    {
        return await applicationReviewRepository.GetByApplicationId(applicationId, token);
    }

    public async Task<PaginatedList<VacancyDetail>> GetPagedByAccountAndStatusAsync(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        List<ApplicationReviewStatus>? status = null,
        CancellationToken token = default)
    {
        var appReviews = await applicationReviewRepository.GetPagedByAccountAndStatusAsync(accountId, pageNumber, pageSize, sortColumn, isAscending, status, token);
        var vacancyDetails = GetVacancyDetails(appReviews.Items);
        return new PaginatedList<VacancyDetail>(vacancyDetails, appReviews.TotalCount, appReviews.PageIndex, appReviews.PageSize);
    }

    public async Task<PaginatedList<VacancyDetail>> GetPagedAllSharedByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default)
    {
        var appReviews = await applicationReviewRepository.GetAllSharedByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);
        var vacancyDetails = GetVacancyDetails(appReviews.Items);
        return new PaginatedList<VacancyDetail>(vacancyDetails, appReviews.TotalCount, appReviews.PageIndex, appReviews.PageSize);
    }

    public async Task<PaginatedList<VacancyDetail>> GetPagedByUkprnAndStatusAsync(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        List<ApplicationReviewStatus>? status = null,
        CancellationToken token = default)
    {
        var appReviews =
            await applicationReviewRepository.GetPagedByUkprnAndStatusAsync(ukprn, pageNumber, pageSize, sortColumn, isAscending,
                status, token);
        var vacancyDetails = GetVacancyDetails(appReviews.Items);
        return new PaginatedList<VacancyDetail>(vacancyDetails, appReviews.TotalCount, appReviews.PageIndex,
            appReviews.PageSize);
    }

    public async Task<List<ApplicationReviewEntity>> GetAllByIdAsync(List<Guid> ids, CancellationToken token = default)
    {
        return await applicationReviewRepository.GetAllByIdAsync(ids, token);
    }

    public async Task<ApplicationReviewEntity?> GetByVacancyReferenceAndCandidateId(long vacancyReference, Guid candidateId, CancellationToken token = default)
    {
        return await applicationReviewRepository.GetByVacancyReferenceAndCandidateId(vacancyReference, candidateId,
            token);
    }

    public async Task<List<ApplicationReviewEntity>> GetAllByVacancyReferenceAndStatus(long vacancyReference, ApplicationReviewStatus status, bool isTempStatus,
        CancellationToken token = default)
    {
        return await applicationReviewRepository.GetAllByVacancyReferenceAndStatus(vacancyReference, status,
            isTempStatus, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetPagedAccountIdAsync(long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default)
    {
        return await applicationReviewRepository.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetPagedUkprnAsync(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default)
    {
        return await applicationReviewRepository.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<DashboardModel> GetCountByAccountId(long accountId, CancellationToken token = default)
    {
        var dashboardCount = await applicationReviewRepository.GetAllByAccountId(accountId, token);
        int sharedApplicationReviewsCount = await applicationReviewRepository.GetSharedCountByAccountId(accountId, token);
        int allSharedApplicationReviewsCount = await applicationReviewRepository.GetAllSharedCountByAccountId(accountId, token);

        var dashboardModel = (DashboardModel)dashboardCount;
        dashboardModel.SharedApplicationsCount = sharedApplicationReviewsCount;
        dashboardModel.AllSharedApplicationsCount = allSharedApplicationReviewsCount;

        return dashboardModel;
    }

    public async Task<DashboardModel> GetCountByUkprn(int ukprn, CancellationToken token = default)
    {
        return await applicationReviewRepository.GetAllByUkprn(ukprn, token);
    }

    public async Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        return await applicationReviewRepository.Update(entity, token);
    }

    public async Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        return await applicationReviewRepository.Upsert(entity, token);
    }

    public async Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByAccountId(long accountId, List<long> vacancyReferences, ApplicationReviewStatus? applicationSharedFilteringStatus,
        CancellationToken token = default)
    {
        var applicationReviews = applicationSharedFilteringStatus is ApplicationReviewStatus.Shared or ApplicationReviewStatus.AllShared
            ? applicationSharedFilteringStatus == ApplicationReviewStatus.AllShared
                ? await applicationReviewRepository.GetAllSharedByAccountId(accountId, vacancyReferences, token)
                : await applicationReviewRepository.GetNewSharedByAccountId(accountId, vacancyReferences, token)
            : await applicationReviewRepository.GetByAccountIdAndVacancyReferencesAsync(accountId, vacancyReferences, token);

        return GetApplicationReviewsStats(vacancyReferences, applicationReviews);
    }

    public async Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByUkprn(int ukprn, List<long> vacancyReferences,
        CancellationToken token = default)
    {
        var applicationReviews = await applicationReviewRepository.GetByUkprnAndVacancyReferencesAsync(ukprn, vacancyReferences, token);

        return GetApplicationReviewsStats(vacancyReferences, applicationReviews);
    }

    public async Task<List<ApplicationReviewEntity>> GetAllByVacancyReference(long vacancyReference, CancellationToken token = default)
    {
        return await applicationReviewRepository.GetAllByVacancyReference(vacancyReference, token);
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
                        NewApplications = applicationReviewEntities.New(),
                        SharedApplications = applicationReviewEntities.Shared(),
                        AllSharedApplications = applicationReviewEntities.AllShared(),
                        SuccessfulApplications = applicationReviewEntities.Successful(),
                        UnsuccessfulApplications = applicationReviewEntities.Unsuccessful(),
                        EmployerReviewedApplications = applicationReviewEntities.EmployerReviewed(),
                        Applications = applicationReviewEntities.AllCount(),
                        HasNoApplications = applicationReviewEntities.HasNoApplications()
                    };
                })
            .ToList();
    }

    private static List<VacancyDetail> GetVacancyDetails(List<ApplicationReviewEntity> applicationReviews)
    {
        if (applicationReviews.Count == 0) return [];

        return applicationReviews
            .GroupBy(ar => ar.VacancyReference)
            .Select(g =>
            {
                int newApplications = g.Count(ar => ar is { Status: ApplicationReviewStatus.New, WithdrawnDate: null });
                int shared = g.Count(ar => ar is { Status: ApplicationReviewStatus.Shared, WithdrawnDate: null });
                int allSharedApplications = g.Count(ar => ar is {DateSharedWithEmployer: not null, WithdrawnDate: null});
                int applications = g.Count(ar => ar.WithdrawnDate == null);

                return new VacancyDetail {
                    VacancyReference = g.Key,
                    NewApplications = newApplications,
                    Applications = applications,
                    Shared = shared,
                    AllSharedApplications = allSharedApplications
                };
            })
            .ToList();
    }
}