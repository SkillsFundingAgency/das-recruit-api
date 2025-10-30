using System.Threading;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface IReportRepository : IReadRepository<ReportEntity, Guid>, IWriteRepository<ReportEntity, Guid>
{
    Task<List<ApplicationReviewReport>> Generate(Guid reportId, CancellationToken token);
    Task<List<ReportEntity>> GetManyByUkprn(int ukprn, CancellationToken token);
    Task<List<ReportEntity>> GetMany(CancellationToken token);
    Task IncrementReportDownloadCountAsync(Guid reportId, CancellationToken token);
}
public class ReportRepository(IRecruitDataContext recruitDataContext) : IReportRepository
{
    private const int DeleteReportAfterTimeSpanDays = 7;

    public async Task<List<ApplicationReviewReport>> Generate(Guid reportId, CancellationToken token)
    {
        var cutOffDateTime = DateTime.UtcNow.AddDays(DeleteReportAfterTimeSpanDays * -1);

        var reportEntity = await recruitDataContext.ReportEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportId 
                                      && r.CreatedDate > cutOffDateTime, token);

        if (reportEntity == null || string.IsNullOrEmpty(reportEntity.DynamicCriteria) || reportEntity.Criteria == null)
            return [];

        var criteria = reportEntity.Criteria;
        
        var appQuery = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(app => app.SubmittedDate >= criteria.FromDate &&
                          app.SubmittedDate <= criteria.ToDate);

        var vacancyQuery = recruitDataContext.VacancyEntities
            .AsNoTracking()
            .Where(v => v.Status == VacancyStatus.Live || v.Status == VacancyStatus.Closed);

        if (reportEntity.OwnerType == ReportOwnerType.Provider)
        {
            appQuery = appQuery.Where(app => app.Ukprn == criteria.Ukprn);
            vacancyQuery = vacancyQuery.Where(v => v.Ukprn == criteria.Ukprn);
        }

        return await appQuery
            .Join(
                vacancyQuery,
                app => app.VacancyReference,
                v => v.VacancyReference,
                (app, v) => new ApplicationReviewReport {
                    VacancyReference = app.VacancyReference,
                    CandidateId = app.CandidateId,
                    ApplicationId = app.Id,
                    VacancyTitle = v.Title,
                    EmployerName = v.EmployerName,
                    ApplicationStatus = app.Status,
                    ApprenticeshipType = v.ApprenticeshipType ?? ApprenticeshipTypes.Standard,
                    ApplicationSubmittedDate = app.SubmittedDate,
                    AvailableWhere = v.EmployerLocationOption,
                    ProgrammeId = Convert.ToInt32(v.ProgrammeId),
                    TrainingProviderName = v.TrainingProvider_Name,
                    VacancyClosingDate = v.ClosedDate ?? v.ClosingDate,
                    NumberOfDaysApplicationAtThisStatus = EF.Functions.DateDiffDay(
                        app.StatusUpdatedDate ?? app.SubmittedDate,
                        DateTime.UtcNow)
                })
            .ToListAsync(token);
    }

    public async Task<List<ReportEntity>> GetManyByUkprn(int ukprn, CancellationToken token)
    {
        var cutOffDateTime = DateTime.UtcNow.AddDays(DeleteReportAfterTimeSpanDays * -1);

        var providerReports = await recruitDataContext.ReportEntities
            .AsNoTracking()
            .Where(r => r.OwnerType == ReportOwnerType.Provider &&
                        r.CreatedDate > cutOffDateTime)
            .ToListAsync(token);

        var filteredReports = providerReports
            .Where(r => r.Criteria != null && r.Criteria.Ukprn == ukprn)
            .ToList();

        return filteredReports;
    }

    public async Task<List<ReportEntity>> GetMany(CancellationToken token)
    {
        var cutOffDateTime = DateTime.UtcNow.AddDays(DeleteReportAfterTimeSpanDays * -1);

        var reportEntity = await recruitDataContext.ReportEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.OwnerType == ReportOwnerType.Qa
                                      && r.CreatedDate > cutOffDateTime,
                token);

        if (reportEntity == null || string.IsNullOrEmpty(reportEntity.DynamicCriteria) || reportEntity.Criteria == null)
            return [];

        return await recruitDataContext.ReportEntities
            .AsNoTracking()
            .Where(r => r.OwnerType == ReportOwnerType.Provider
                        && r.Criteria != null)
            .ToListAsync(token);
    }

    public async Task IncrementReportDownloadCountAsync(Guid reportId, CancellationToken cancellationToken)
    {
        var existingEntity = await GetOneAsync(reportId, cancellationToken);
        if (existingEntity is not null)
        {
            existingEntity.DownloadCount += 1;
            await recruitDataContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<ReportEntity?> GetOneAsync(Guid key, CancellationToken cancellationToken)
    {
        return await recruitDataContext
            .ReportEntities
            .FirstOrDefaultAsync(x => x.Id == key, cancellationToken);
    }

    public async Task<UpsertResult<ReportEntity>> UpsertOneAsync(ReportEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = entity.Id == Guid.Empty ? null : await GetOneAsync(entity.Id, cancellationToken);
        if (existingEntity is null)
        {
            await recruitDataContext.ReportEntities.AddAsync(entity, cancellationToken);
            await recruitDataContext.SaveChangesAsync(cancellationToken);
            return UpsertResult.Create(entity, true);
        }

        recruitDataContext.SetValues(existingEntity, entity);
        await recruitDataContext.SaveChangesAsync(cancellationToken);
        return UpsertResult.Create(entity, false);
    }

    public Task<bool> DeleteOneAsync(Guid key, CancellationToken cancellationToken)
    {
        return Task.FromResult(false);
    }
}