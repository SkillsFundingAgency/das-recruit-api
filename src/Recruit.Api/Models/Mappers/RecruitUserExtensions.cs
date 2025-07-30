using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Requests.User;
using SFA.DAS.Recruit.Api.Models.Responses.User;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class RecruitUserExtensions
{
    public static UserEntity ToDomain(this PutUserRequest request, Guid id)
    {
        return new UserEntity {
            Name = request.Name,
            Email = request.Email,
            Id = id,
            UserType = Enum.Parse<UserType>(request.UserType),
            ClosedVacanciesWithdrawnByQaAlertDismissedOn = request.ClosedVacanciesWithdrawnByQaAlertDismissedOn,
            Ukprn = request.Ukprn,
            CreatedDate = request.CreatedDate,
            EmployerAccountIds = System.Text.Json.JsonSerializer.Serialize(request.EmployerAccountIds),
            IdamsUserId = request.IdamsUserId,
            LastSignedInDate = request.LastSignedInDate,
            DfEUserId = request.DfEUserId,
            ClosedVacanciesBlockedProviderAlertDismissedOn = request.ClosedVacanciesBlockedProviderAlertDismissedOn,
            TransferredVacanciesBlockedProviderAlertDismissedOn = request.TransferredVacanciesBlockedProviderAlertDismissedOn,
            TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn = request.TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn
        };
    }
    
    public static PutUserResponse ToPutResponse(this UserEntity entity)
    {
        return new PutUserResponse {
            Id = entity.Id,
        };
    }
}