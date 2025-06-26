using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

public record VacancyDashboardResponse(PageInfo Info, IEnumerable<VacancyDetail> Items);
