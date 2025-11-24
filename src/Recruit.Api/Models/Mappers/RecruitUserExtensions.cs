using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Requests.User;
using SFA.DAS.Recruit.Api.Models.Responses.User;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class RecruitUserExtensions
{
    public static UserEntity ToDomain(this PutUserRequest request, Guid id)
    {
        var employerAccounts = request
            .EmployerAccountIds
            .Select(x => new UserEmployerAccountEntity { UserId = id, EmployerAccountId = x })
            .ToList();
        
        return new UserEntity
        {
            Name = request.Name,
            Email = request.Email,
            Id = id,
            UserType = request.UserType.ToDomain(),
            ClosedVacanciesWithdrawnByQaAlertDismissedOn = request.ClosedVacanciesWithdrawnByQaAlertDismissedOn,
            Ukprn = request.Ukprn,
            CreatedDate = request.CreatedDate,
            EmployerAccounts = employerAccounts,
            IdamsUserId = request.IdamsUserId,
            LastSignedInDate = request.LastSignedInDate,
            DfEUserId = request.DfEUserId,
            ClosedVacanciesBlockedProviderAlertDismissedOn = request.ClosedVacanciesBlockedProviderAlertDismissedOn,
            TransferredVacanciesBlockedProviderAlertDismissedOn = request.TransferredVacanciesBlockedProviderAlertDismissedOn,
            TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn = request.TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn,
            NotificationPreferences = request.NotificationPreferences
        };
    }

    private static RecruitUser ToResponseDto(UserEntity user)
    {
        return new RecruitUser {
            Id = user.Id,
            UserType = user.UserType.ToDto(),
            Email = user.Email,
            Name = user.Name,
            NotificationPreferences = user.NotificationPreferences,
            ClosedVacanciesBlockedProviderAlertDismissedOn = user.ClosedVacanciesBlockedProviderAlertDismissedOn,
            ClosedVacanciesWithdrawnByQaAlertDismissedOn = user.ClosedVacanciesWithdrawnByQaAlertDismissedOn,
            CreatedDate = user.CreatedDate,
            DfEUserId = user.DfEUserId,
            EmployerAccountIds = user.EmployerAccounts?.Select(x => x.EmployerAccountId).ToList() ?? [],
            IdamsUserId = user.IdamsUserId,
            LastSignedInDate = user.LastSignedInDate,
            TransferredVacanciesBlockedProviderAlertDismissedOn = user.TransferredVacanciesBlockedProviderAlertDismissedOn,
            TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn = user.TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn,
            Ukprn = user.Ukprn,
            UpdatedDate = user.UpdatedDate,
        };
    }
    
    public static PutUserResponse ToPutResponse(this UserEntity entity)
    {
        return new PutUserResponse(entity.Id);
    }

    public static RecruitUser ToGetResponse(this UserEntity entity)
    {
        return ToResponseDto(entity);
    }
    
    public static RecruitUser ToPatchResponse(this UserEntity entity)
    {
        return ToResponseDto(entity);
    }
}