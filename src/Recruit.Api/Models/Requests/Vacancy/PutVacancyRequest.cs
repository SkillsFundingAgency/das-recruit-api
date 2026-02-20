using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

public class PutVacancyRequest : VacancyRequest
{
    public long? VacancyReference { get; init; }
    
    public DateTime? CreatedDate { get; init; }
}