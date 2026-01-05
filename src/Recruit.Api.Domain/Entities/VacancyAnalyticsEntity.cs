using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Domain.Entities;
public class VacancyAnalyticsEntity
{
    public required long VacancyReference { get; set; }
    public DateTime UpdatedDate { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    [MaxLength(4000)]
    public string Analytics { get; set; } = string.Empty;

    private bool _isParsed = false;
    private List<VacancyAnalytics>? _cached;

    private void ParseAnalyticsIfNeeded()
    {
        if (_isParsed)
            return;

        _isParsed = true;

        try
        {
            _cached = JsonSerializer.Deserialize<List<VacancyAnalytics>>(Analytics)
                      ?? [];
        }
        catch
        {
            _cached = [];
        }
    }

    [NotMapped]
    public List<VacancyAnalytics> AnalyticsData
    {
        get
        {
            ParseAnalyticsIfNeeded();
            return _cached!;
        }
    }
}