using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Models;
public record TransferInfo
{
    public long Ukprn { get; set; }
    public string ProviderName { get; set; }
    public string LegalEntityName { get; set; }
    public DateTime TransferredDate { get; set; }
    public TransferReason Reason { get; set; }
}
