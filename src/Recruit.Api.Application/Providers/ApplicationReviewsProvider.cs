﻿using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Data.Models;
using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Enums;
using Recruit.Api.Domain.Models;

namespace Recruit.Api.Application.Providers;

public interface IApplicationReviewsProvider
{
    Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default);

    Task<PaginatedList<ApplicationReviewEntity>> GetAllByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = "CreatedDate",
        bool isAscending = false,
        CancellationToken token = default);

    Task<PaginatedList<ApplicationReviewEntity>> GetAllByUkprn(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = "CreatedDate",
        bool isAscending = false,
        CancellationToken token = default);

    Task<DashboardModel> GetCountByAccountId(long accountId, ApplicationStatus status, CancellationToken token = default);

    Task<DashboardModel> GetCountByUkprn(int ukprn, ApplicationStatus status, CancellationToken token = default);

    Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default);

    Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default);

    Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByAccountId(long accountId, List<long> vacancyReferences, CancellationToken token = default);
    Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByUkprn(int ukprn, List<long> vacancyReferences, CancellationToken token = default);
}

public class ApplicationReviewsProvider(IApplicationReviewRepository repository) : IApplicationReviewsProvider
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

    public async Task<DashboardModel> GetCountByAccountId(long accountId, ApplicationStatus status, CancellationToken token = default)
    {
        var applicationReviews = await repository.GetAllByAccountId(accountId, status.ToString(), token);

        return GetDashboardModel(applicationReviews);
    }

    public async Task<DashboardModel> GetCountByUkprn(int ukprn, ApplicationStatus status, CancellationToken token = default)
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
        var applicationReviews = await repository.GetAllByAccountId(accountId, vacancyReferences, ApplicationStatus.Submitted.ToString(), token);

        return applicationReviews.GroupBy(fil => fil.VacancyReference)
            .Select(fil => new ApplicationReviewsStats {
                VacancyReference = fil.Key,
                NewApplications = fil.Count(entity => entity.ReviewedDate == null),
                TotalApplication = fil.Count()
            }).ToList();
    }

    public async Task<List<ApplicationReviewsStats>> GetVacancyReferencesCountByUkprn(int ukprn, List<long> vacancyReferences,
        CancellationToken token = default)
    {
        var applicationReviews = await repository.GetAllByUkprn(ukprn, vacancyReferences, ApplicationStatus.Submitted.ToString(), token);

        return applicationReviews.GroupBy(fil => fil.VacancyReference)
            .Select(fil => new ApplicationReviewsStats {
                VacancyReference = fil.Key,
                NewApplications = fil.Count(entity => entity.ReviewedDate == null),
                TotalApplication = fil.Count()
            }).ToList();
    }

    private static DashboardModel GetDashboardModel(List<ApplicationReviewEntity> applicationReviews)
    {
        return new DashboardModel
        {
            NewApplicationsCount = applicationReviews.Count(fil => fil.ReviewedDate == null),
            EmployerReviewedApplicationsCount = applicationReviews.Count(fil => fil is { ReviewedDate: not null }),
        };
    }
}