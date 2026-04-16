using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SFA.DAS.Recruit.Api.Validators.Rules;

public static class RuleExtensions
{
    /// <summary>Options used to determine the refer/approve thresholds which are used when calculating the ruleset outcome.
    /// This allows for stringency or leniency to be applied.
    /// Basically, lower scores are better.
    /// <para>A low <paramref name="referralThreshold"></paramref> value means there's more chance of a refer outcome as 
    /// any score that are above this value will be a referral.</para>
    /// <para>A high <paramref name="referralThreshold"></paramref> value means there's more chance of an approve outcome.
    /// Any scores between these thresholds will result in an indeterminate outcome.</para>
    /// </summary>
    /// <param name="referralThreshold">If the score is ABOVE this value then the outcome will be to refer</param>
    /// <param name="approvalThreshold">If the score is BELOW this value then the outcome will be to approve</param>
    public record struct RuleSetOptions(int ReferralThreshold, int ApprovalThreshold)
    {
        public static readonly RuleSetOptions ZeroTolerance = new (0, 1);
    }
    
    private static RuleSetDecision CalculateOutcome(this List<RuleOutcome> ruleOutcomes, RuleSetOptions options)
    {
        var totalScore = ruleOutcomes.Sum(o => o.Score);

        if (totalScore > options.ReferralThreshold)
        {
            return RuleSetDecision.Refer;
        }
        
        if (totalScore < options.ApprovalThreshold)
        {
            return RuleSetDecision.Approve;
        }

        return RuleSetDecision.Indeterminate;
    }
    
    public static async Task<RuleSetOutcome> EvaluateAsync<T>(this IEnumerable<IRule<T>> rules, T subject, CancellationToken cancellationToken = default)
    {
        var elements = rules.ToArray();
        if (elements.Length == 0)
        {
            throw new InvalidOperationException("RuleSet has no rules defined");
        }
        
        var outcome = new RuleSetOutcome();
        foreach (var rule in elements)
        {
            var ruleOutcome = await rule.EvaluateAsync(subject, cancellationToken);
            outcome.RuleOutcomes.Add(ruleOutcome);
        }

        outcome.Decision = outcome.RuleOutcomes.CalculateOutcome(RuleSetOptions.ZeroTolerance);
        return outcome;
    }
    
    public static string GetQualifiedFieldId<T>(this Expression<Func<T>> property)
    {
        return GetFieldIdForProperty(property.Body);
    }
        
    private static string GetFieldIdForProperty(Expression propertyBody)
    {
        var memberExpression = propertyBody as MemberExpression;
        var fieldId = string.Empty;
        while (memberExpression is { Member.MemberType: MemberTypes.Property })
        {
            fieldId = memberExpression.Member.Name + (fieldId != string.Empty ? "." : string.Empty) + fieldId;
            memberExpression = memberExpression.Expression as MemberExpression;
        }

        return fieldId;
    }
    
    public static string FormatForParsing(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;

        var rgx = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled, TimeSpan.FromSeconds(3000));
        var sanitized = rgx.Replace(value, " ");

        sanitized = RemoveContiguousWhitespace(sanitized);

        return $" {sanitized} ";
    }
        
    private static string RemoveContiguousWhitespace(string value)
    {
        return Regex.Replace(value, @"\s+", " ", RegexOptions.Compiled, TimeSpan.FromSeconds(3000));
    }

    public static int CountOccurrences(this string body, string term)
    {
        var count = 0;
        var offset = 0;

        if (string.IsNullOrWhiteSpace(body) || string.IsNullOrWhiteSpace(term)) return count;

        var paddedTerm = $" {term} ";
        var checkBody = $" {body} ";

        while ((offset = checkBody.IndexOf(paddedTerm, offset, StringComparison.InvariantCultureIgnoreCase)) != -1)
        {
            offset += term.Length;
            count++;
        }

        return count;
    }
}