using System.Globalization;
using System.Linq;
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
        var appReviews = await applicationReviewRepository.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, status, token);
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
            await applicationReviewRepository.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending,
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

    public async Task<DashboardModel> GetCountByAccountId(long accountId, CancellationToken token = default)
    {
        var applicationReviews = await applicationReviewRepository.GetAllByAccountId(accountId, token);
        
        // TODO: This will be moved to as separate endpoint in future and combine the results in the Recruit Outer Api
        var allSharedApplicationReviews = await applicationReviewRepository.GetAllSharedByAccountId(accountId, token); 

        var dashboardModel = new DashboardModel {
            NewApplicationsCount = applicationReviews.FirstOrDefault(c=>c.Status == ApplicationReviewStatus.New)?.Count ?? 0,
            SharedApplicationsCount = allSharedApplicationReviews,
            SuccessfulApplicationsCount = applicationReviews.FirstOrDefault(c=>c.Status == ApplicationReviewStatus.Successful)?.Count ?? 0,
            UnsuccessfulApplicationsCount = applicationReviews.FirstOrDefault(c=>c.Status == ApplicationReviewStatus.Unsuccessful)?.Count ?? 0,
            EmployerReviewedApplicationsCount = (applicationReviews.FirstOrDefault(c=>c.Status == ApplicationReviewStatus.EmployerInterviewing)?.Count ?? 0) 
                + (applicationReviews.FirstOrDefault(c=>c.Status == ApplicationReviewStatus.EmployerUnsuccessful)?.Count ?? 0)
        };

        return dashboardModel;
    }

    public async Task<DashboardModel> GetCountByUkprn(int ukprn, CancellationToken token = default)
    {
        var applicationReviews = await applicationReviewRepository.GetAllByUkprn(ukprn, token);

        return new DashboardModel {
            NewApplicationsCount = applicationReviews.FirstOrDefault(c=>c.Status == ApplicationReviewStatus.New)?.Count ?? 0,
            SharedApplicationsCount = 0,
            SuccessfulApplicationsCount = applicationReviews.FirstOrDefault(c=>c.Status == ApplicationReviewStatus.Successful)?.Count ?? 0,
            UnsuccessfulApplicationsCount = applicationReviews.FirstOrDefault(c=>c.Status == ApplicationReviewStatus.Unsuccessful)?.Count ?? 0,
            EmployerReviewedApplicationsCount = (applicationReviews.FirstOrDefault(c=>c.Status == ApplicationReviewStatus.EmployerInterviewing)?.Count ?? 0) 
                                                + (applicationReviews.FirstOrDefault(c=>c.Status == ApplicationReviewStatus.EmployerUnsuccessful)?.Count ?? 0)
        };
    }

    public async Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        return await applicationReviewRepository.Update(entity, token);
    }

    public async Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        return await applicationReviewRepository.Upsert(entity, token);
    }

    public async Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByAccountId(long accountId, List<long> vacancyReferences,ApplicationReviewStatus? applicationSharedFilteringStatus,
        CancellationToken token = default)
    {
        List<ApplicationReviewEntity> applicationReviews;
        if (applicationSharedFilteringStatus is ApplicationReviewStatus.Shared or ApplicationReviewStatus.AllShared)
        {
            if (applicationSharedFilteringStatus == ApplicationReviewStatus.AllShared)
            {
                applicationReviews  = await applicationReviewRepository.GetAllSharedByAccountId(accountId,vacancyReferences, token);    
            }
            else
            {
                applicationReviews  = await applicationReviewRepository.GetNewSharedByAccountId(accountId, vacancyReferences,token);    
            }
        }
        else
        {
            applicationReviews  = await applicationReviewRepository.GetAllByAccountId(accountId, vacancyReferences, token);
        }
        

        return GetApplicationReviewsStats(vacancyReferences, applicationReviews);
    }

    public async Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByUkprn(int ukprn, List<long> vacancyReferences,
        CancellationToken token = default)
    {
        var applicationReviews = await applicationReviewRepository.GetAllByUkprn(ukprn, vacancyReferences, token);

        return GetApplicationReviewsStats(vacancyReferences, applicationReviews);
    }

    public async Task<List<ApplicationReviewEntity>> GetAllByVacancyReference(long vacancyReference, CancellationToken token = default)
    {
        return await applicationReviewRepository.GetAllByVacancyReference(vacancyReference, token);
    }

    private static DashboardModel GetDashboardModel(List<ApplicationReviewEntity> applicationReviews, bool skipAllShared = false)
    {
        // Use a default date to filter out uninitialized DateSharedWithEmployer values
        var defaultDate = new DateTime(1900, 1, 1, 1, 0, 0, 389, DateTimeKind.Utc);

        return new DashboardModel
        {
            NewApplicationsCount = applicationReviews.Count(fil =>
                fil is {Status: nameof(ApplicationReviewStatus.New), WithdrawnDate: null}),
            EmployerReviewedApplicationsCount = applicationReviews.Count(entity =>
                entity.Status is nameof(ApplicationReviewStatus.EmployerUnsuccessful) or nameof(ApplicationReviewStatus.EmployerInterviewing)),
            SharedApplicationsCount = applicationReviews.Count(e =>
                e is { Status: nameof(ApplicationReviewStatus.Shared), WithdrawnDate: null }),
            AllSharedApplicationsCount = skipAllShared ? 0 : applicationReviews.Count(e =>
                e is { Status: nameof(ApplicationReviewStatus.Shared), WithdrawnDate: null, DateSharedWithEmployer: not null } &&
                e.DateSharedWithEmployer > defaultDate),
            SuccessfulApplicationsCount = applicationReviews.Count(e =>
                e is { Status: nameof(ApplicationReviewStatus.Successful), WithdrawnDate: null }),
            UnsuccessfulApplicationsCount = applicationReviews.Count(e =>
                e is { Status: nameof(ApplicationReviewStatus.Unsuccessful), WithdrawnDate: null }),
            HasNoApplications = applicationReviews.All(e => e.WithdrawnDate != null)
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

                    return new ApplicationReviewsStats
                    {
                        VacancyReference = vacancyRef,
                        NewApplications = applicationReviewEntities.Count(e =>
                            e is { Status: nameof(ApplicationReviewStatus.New), WithdrawnDate: null }),
                        SharedApplications = applicationReviewEntities.Count(e =>
                            e is { Status: nameof(ApplicationReviewStatus.Shared), WithdrawnDate: null }),
                        AllSharedApplications = applicationReviewEntities.Count(e =>
                            e is { Status: nameof(ApplicationReviewStatus.Shared), WithdrawnDate: null, DateSharedWithEmployer: not null } &&
                            e.DateSharedWithEmployer > DateTime.MinValue),
                        SuccessfulApplications = applicationReviewEntities.Count(e =>
                            e is { Status: nameof(ApplicationReviewStatus.Successful), WithdrawnDate: null }),
                        UnsuccessfulApplications = applicationReviewEntities.Count(e =>
                            e is { Status: nameof(ApplicationReviewStatus.Unsuccessful), WithdrawnDate: null }),
                        EmployerReviewedApplications = applicationReviewEntities.Count(e =>
                            e.Status is nameof(ApplicationReviewStatus.EmployerUnsuccessful) or
                                nameof(ApplicationReviewStatus.EmployerInterviewing) &&
                            e.WithdrawnDate == null),
                        Applications = applicationReviewEntities.Count(e => e.WithdrawnDate == null),
                        HasNoApplications = applicationReviewEntities.All(e => e.WithdrawnDate != null)
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
                NewApplications = g.Count(ar => ar is {Status: nameof(ApplicationReviewStatus.New), WithdrawnDate: null}),
                Applications = g.Count(ar => ar.WithdrawnDate == null),
                Shared = g.Count(ar => ar is {Status: nameof(ApplicationReviewStatus.Shared), WithdrawnDate: null}),
                AllSharedApplications = g.Count(ar => ar.DateSharedWithEmployer > defaultDate && ar.WithdrawnDate == null)
            })
            .ToList();
    }
}