using System;
using System.Text.Json;

namespace SFA.DAS.Recruit.Api.Domain.Models;
public record ReportCriteria
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int? Ukprn { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}