namespace SFA.DAS.Recruit.Api.Validators.Rules;

public class RuleOutcome
{
    public const string NoSpecificTarget = "";

    public RuleOutcome(RuleId ruleId, int score, string narrative, string target = NoSpecificTarget, IEnumerable<RuleOutcome>? details = null, string? data = null)
    {
        Id = Guid.NewGuid();
        RuleId = ruleId;
        Score = Math.Min(score, 100);
        Narrative = narrative;
        Target = target;
        Data = data;

        if (details != null)
        {
            Details = details;
        }
    }

    public Guid Id { get; set; }
    public IEnumerable<RuleOutcome>? Details { get; set; }
    private bool HasDetails => Details?.Any() ?? false;
    public RuleId RuleId { get; set; }
    public int Score { get; set;  }
    public string Narrative { get; set;  }
    public string? Data { get; set; }

    /// <summary>
    /// Field or reference that this outcome relates to
    /// </summary>
    public string Target { get; set;  }

    public override string ToString()
    {
        return $"Rule {RuleId} score {Score} ({(HasDetails ? $"{Details!.Count()} details" : $"'{Narrative}'")})";
    }
}