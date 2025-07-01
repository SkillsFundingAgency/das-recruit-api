using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
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

    Task<EmployerApplicationReviewStatsModel> GetCountByAccountId(long accountId, CancellationToken token = default);

    Task<SharedApplicationReviewsStatsModel> GetSharedApplicationsCountByAccountId(long accountId,
        CancellationToken token = default);

    Task<ProviderApplicationReviewStatsModel> GetCountByUkprn(int ukprn, CancellationToken token = default);

    Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default);

    Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default);

    Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByAccountId(long accountId, List<long> vacancyReferences, ApplicationReviewStatus? applicationSharedFilteringStatus = null, CancellationToken token = default);
    Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByUkprn(int ukprn, List<long> vacancyReferences, CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetAllByVacancyReference(long vacancyReference, CancellationToken token = default);
    Task<ApplicationReviewEntity?> GetByApplicationId(Guid applicationId, CancellationToken token = default);
    Task<PaginatedList<VacancyDetail>> GetAllByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        List<ApplicationReviewStatus>? status = null,
        CancellationToken token = default);

    Task<PaginatedList<VacancyDetail>> GetAllSharedByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default);

    Task<PaginatedList<VacancyDetail>> GetAllByUkprn(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        List<ApplicationReviewStatus>? status = null,
        CancellationToken token = default);
}

internal class ApplicationReviewsProvider(
    IApplicationReviewRepository applicationReviewRepository) : IApplicationReviewsProvider
{
    private static readonly DateTime DefaultDate = new(1900, 1, 1, 1, 0, 0, 389, DateTimeKind.Utc);

    public async Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default)
    {
        return await applicationReviewRepository.GetById(id, token);
    }

    public async Task<ApplicationReviewEntity?> GetByApplicationId(Guid applicationId, CancellationToken token = default)
    {
        return await applicationReviewRepository.GetByApplicationId(applicationId, token);
    }

    public async Task<PaginatedList<VacancyDetail>> GetAllByAccountId(long accountId,
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

    public async Task<PaginatedList<VacancyDetail>> GetAllSharedByAccountId(long accountId,
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

    public async Task<PaginatedList<VacancyDetail>> GetAllByUkprn(int ukprn,
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

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllByAccountId(long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default)
    {
        return await applicationReviewRepository.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllByUkprn(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default)
    {
        return await applicationReviewRepository.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<EmployerApplicationReviewStatsModel> GetCountByAccountId(long accountId, CancellationToken token = default)
    {
        return await applicationReviewRepository.GetAllByAccountId(accountId, token);
    }

    public async Task<SharedApplicationReviewsStatsModel> GetSharedApplicationsCountByAccountId(long accountId, CancellationToken token = default)
    {
        return await applicationReviewRepository.GetAllSharedByAccountId(accountId, token);
    }

    public async Task<ProviderApplicationReviewStatsModel> GetCountByUkprn(int ukprn, CancellationToken token = default)
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

    public async Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByAccountId(long accountId,
        List<long> vacancyReferences,
        ApplicationReviewStatus? applicationSharedFilteringStatus,
        CancellationToken token = default)
    {
        List<ApplicationReviewEntity> applicationReviews;
        if (applicationSharedFilteringStatus is ApplicationReviewStatus.Shared or ApplicationReviewStatus.AllShared)
        {
            if (applicationSharedFilteringStatus == ApplicationReviewStatus.AllShared)
            {
                applicationReviews = await applicationReviewRepository.GetAllSharedByAccountId(accountId, vacancyReferences, token);
            }
            else
            {
                applicationReviews = await applicationReviewRepository.GetNewSharedByAccountId(accountId, vacancyReferences, token);
            }
        }
        else
        {
            applicationReviews = await applicationReviewRepository.GetByAccountIdAndVacancyReferencesAsync(accountId, vacancyReferences, token);
        }


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
            .Select(g => new VacancyDetail {
                VacancyReference = g.Key,
                NewApplications = g.Count(ar => ar is { Status: nameof(ApplicationReviewStatus.New), WithdrawnDate: null }),
                Applications = g.Count(ar => ar.WithdrawnDate == null),
                Shared = g.Count(ar => ar is { Status: nameof(ApplicationReviewStatus.Shared), WithdrawnDate: null }),
                AllSharedApplications = g.Count(ar => ar.DateSharedWithEmployer > DefaultDate && ar.WithdrawnDate == null)
            })
            .ToList();
    }
}