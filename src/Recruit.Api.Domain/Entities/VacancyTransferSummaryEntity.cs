using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class VacancyTransferSummaryEntity
{
    public long? AccountId { get; set; }
    public OwnerType? OwnerType { get; set; }
    public string? TransferInfo { get; set; }
    public VacancyStatus Status { get; set; }
    public int? Ukprn { get; set; }
    public DateTime? DeletedDate { get; set; }
}

public class VacancyClosureSummaryEntity
{
    public long? VacancyReference { get; set; }
    public string? Title { get; set; }
    public long? AccountId { get; set; }
    public OwnerType? OwnerType { get; set; }
    public ClosureReason? ClosureReason { get; set; }
    public DateTime? ClosedDate { get; set; }
    public int? Ukprn { get; set; }
    public string? TransferInfo { get; set; }
    public VacancyStatus Status { get; set; }
    public DateTime? DeletedDate { get; set; }
}