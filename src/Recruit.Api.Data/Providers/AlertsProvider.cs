using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Providers;
public interface IAlertsProvider
{
    Task<EmployerTransferredVacanciesAlertModel> GetEmployerTransferredVacanciesAlertByAccountId(long accountId, string userId, TransferReason reason, CancellationToken token = default);
    Task<BlockedProviderAlertModel> GetBlockedProviderAlertCountByAccountId(long accountId, string userId, CancellationToken token = default);
    Task<WithdrawnVacanciesAlertModel> GetWithDrawnByQaAlertByAccountId(long accountId, string userId, CancellationToken token = default);
    Task<ProviderTransferredVacanciesAlertModel> GetProviderTransferredVacanciesAlertByUkprn(int ukprn, string userId, CancellationToken token = default);
    Task<WithdrawnVacanciesAlertModel> GetWithDrawnByQaAlertByUkprnId(int ukprn, string userId, CancellationToken token = default);
}

public class AlertsProvider(IVacancyRepository vacancyRepository,
    IUserRepository userRepository) : IAlertsProvider
{
    public async Task<EmployerTransferredVacanciesAlertModel> GetEmployerTransferredVacanciesAlertByAccountId(long accountId,
        string userId,
        TransferReason reason,
        CancellationToken token = default)
    {
        var userEntity = await userRepository.FindByUserIdAsync(userId, token);
        if (userEntity is null)
        {
            return new EmployerTransferredVacanciesAlertModel();
        }

        var lastDismissedDate = reason switch {
            TransferReason.EmployerRevokedPermission => userEntity
                .TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn.GetValueOrDefault(DateTime.MinValue),
            TransferReason.BlockedByQa => userEntity.TransferredVacanciesBlockedProviderAlertDismissedOn
                .GetValueOrDefault(DateTime.MinValue),
            _ => DateTime.MinValue
        };

        var vacancies = await vacancyRepository.GetAllTransferInfoByAccountId(accountId, token);

        var transferredProviders = vacancies
            .Select(v => ApiUtils.DeserializeOrNull<TransferInfo>(v.TransferInfo))
            .Where(info => info != null
                           && info.Reason == reason
                           && info.TransferredDate > lastDismissedDate)
            .Select(info => info?.ProviderName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        return new EmployerTransferredVacanciesAlertModel {
            TransferredVacanciesProviderNames = transferredProviders,
            TransferredVacanciesCount = transferredProviders.Count
        };
    }

    public async Task<BlockedProviderAlertModel> GetBlockedProviderAlertCountByAccountId(long accountId,
        string userId,
        CancellationToken token = default)
    {
        var userEntity = await userRepository.FindByUserIdAsync(userId, token);
        if (userEntity is null)
        {
            return new BlockedProviderAlertModel();
        }

        var lastDismissedDate = userEntity
            .ClosedVacanciesBlockedProviderAlertDismissedOn.GetValueOrDefault(DateTime.MinValue);

        var vacancies = await vacancyRepository.GetAllClosedEmployerVacanciesByClosureReason(accountId,ClosureReason.BlockedByQa,lastDismissedDate, token);

        var blockedTransfers = vacancies
            .Select(v => new {
                VacancyTitle = $"{v.Title} (VAC{v.VacancyReference})",
                TransferInfo = ApiUtils.DeserializeOrNull<TransferInfo>(v.TransferInfo)
            })
            .ToList();

        var blockedProviderNames = blockedTransfers
            .Select(x => x.TransferInfo!.ProviderName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        return new BlockedProviderAlertModel {
            ClosedVacancies = blockedTransfers.Select(v => v.VacancyTitle).ToList(),
            BlockedProviderNames = blockedProviderNames
        };
    }

    public async Task<WithdrawnVacanciesAlertModel> GetWithDrawnByQaAlertByAccountId(long accountId,
        string userId,
        CancellationToken token = default)
    {
        var userEntity = await userRepository.FindByUserIdAsync(userId, token);
        if (userEntity is null)
        {
            return new WithdrawnVacanciesAlertModel();
        }

        var lastDismissedDate = userEntity.ClosedVacanciesWithdrawnByQaAlertDismissedOn
            .GetValueOrDefault(DateTime.MinValue);

        var vacancies = await vacancyRepository.GetAllClosedEmployerVacanciesByClosureReason(accountId,ClosureReason.WithdrawnByQa,lastDismissedDate, token, VacancyStatus.Closed);

        var blockedTransfers = vacancies
            .Select(v => $"{v.Title} (VAC{v.VacancyReference})")
            .ToList();

        return new WithdrawnVacanciesAlertModel {
            ClosedVacancies = blockedTransfers.ToList(),
        };
    }


    public async Task<ProviderTransferredVacanciesAlertModel> GetProviderTransferredVacanciesAlertByUkprn(int ukprn,
        string userId,
        CancellationToken token = default)
    {
        var userEntity = await userRepository.FindByUserIdAsync(userId, token);
        if (userEntity is null)
        {
            return new ProviderTransferredVacanciesAlertModel();
        }

        var lastDismissedDate = userEntity.TransferredVacanciesEmployerRevokedPermissionAlertDismissedOn
            .GetValueOrDefault(DateTime.MinValue);

        var vacancies = await vacancyRepository.GetAllTransferInfoByUkprn(ukprn, token);

        var legalEntityNames = vacancies
            .Select(v => ApiUtils.DeserializeOrNull<TransferInfo>(v.TransferInfo))
            .Where(info => info is not null && info.TransferredDate > lastDismissedDate)
            .Select(info => info?.LegalEntityName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        return new ProviderTransferredVacanciesAlertModel {
            LegalEntityNames = legalEntityNames
        };
    }

    public async Task<WithdrawnVacanciesAlertModel> GetWithDrawnByQaAlertByUkprnId(int ukprn,
        string userId,
        CancellationToken token = default)
    {
        var userEntity = await userRepository.FindByUserIdAsync(userId, token);
        if (userEntity is null)
        {
            return new WithdrawnVacanciesAlertModel();
        }

        var lastDismissedDate = userEntity.ClosedVacanciesWithdrawnByQaAlertDismissedOn
            .GetValueOrDefault(DateTime.MinValue);

        var vacancies = await vacancyRepository.GetAllClosedProviderVacanciesByClosureReason(ukprn, ClosureReason.WithdrawnByQa, lastDismissedDate, token);

        var closedVacancyTitles = vacancies
            .Select(v => $"{v.Title} (VAC{v.VacancyReference})")
            .OrderBy(title => title)
            .ToList();

        return new WithdrawnVacanciesAlertModel {
            ClosedVacancies = closedVacancyTitles
        };
    }
}
