using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Models;

public sealed record VacancyDashboardModel
{
    public int ClosedVacanciesCount { get; init; }
    public int DraftVacanciesCount { get; init; }
    public int ReviewVacanciesCount { get; init; }
    public int ReferredVacanciesCount { get; init; }
    public int LiveVacanciesCount { get; init; }
    public int SubmittedVacanciesCount { get; init; }
    public int ClosingSoonVacanciesCount { get; init; }
    public int ClosingSoonWithNoApplications { get; init; }


    public static implicit operator VacancyDashboardModel(List<VacancyDashboardCountModel> source)
    {
        var counts = source
            .GroupBy(c => c.Status)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Count));

        return new VacancyDashboardModel {
            ClosedVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Closed),
            DraftVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Draft),
            ReviewVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Review),
            ReferredVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Referred)
                                     + counts.GetValueOrDefault(VacancyStatus.Rejected),
            LiveVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Live),
            SubmittedVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Submitted),
        };
    }
}