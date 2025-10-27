using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class VacancyTransferSummaryEntity
{
    public long? AccountId { get; set; }
    public OwnerType? OwnerType { get; set; }
    public string? TransferInfo { get; set; }
    public VacancyStatus Status { get; set; }
    public int? Ukprn { get; set; }
}