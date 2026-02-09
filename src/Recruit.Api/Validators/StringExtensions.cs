using System.Text.RegularExpressions;
using FluentValidation;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.Validators;

public static class StringExtensions
{
    public static string FormatForParsing(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;

        var rgx = new Regex("[^a-zA-Z0-9]");
        var sanitized = rgx.Replace(value, " ");

        sanitized = RemoveContiguousWhitespace(sanitized);

        return $" {sanitized} ";
    }

    public static string ToDelimitedString<T>(this IEnumerable<T> items, string delimiter)
    {
        return items == null ? string.Empty : string.Join(delimiter, items);
    }
        
    private static string RemoveContiguousWhitespace(string value)
    {
        return Regex.Replace(value, @"\s+", " ");
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

public static class FluentExtensions
{
    public static IRuleBuilderOptions<T, string> ValidPostCode<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new PostCodeValidator<T, string>());
    }
    
    public static IRuleBuilderOptions<T, string> ValidHtmlCharacters<T>(this IRuleBuilder<T, string> ruleBuilder, IHtmlSanitizerService sanitizer)
    {
        return ruleBuilder.SetValidator(new HtmlValidator<T, string>(sanitizer));
    }
}