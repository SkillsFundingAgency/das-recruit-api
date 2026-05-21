using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Validators;

public class ProfanityCheckValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
    public override string Name => "ProfanityCheckValidator";

    private readonly IProhibitedContentRepository _profanityListProvider;

    public ProfanityCheckValidator(IProhibitedContentRepository profanityListProvider)
    {
        _profanityListProvider = profanityListProvider;
    }
        
    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return base.GetDefaultMessageTemplate("{PropertyName} must not contain a banned word or phrase.");
    }


    public override bool IsValid(ValidationContext<T> context, TProperty propertyValue)
    {
        var profanityList = _profanityListProvider.GetByContentTypeAsync(ProhibitedContentType.Profanity, CancellationToken.None);

        var freeText = propertyValue as string;

        var formatForParsing = freeText.FormatForParsing();

        foreach (var profanity in profanityList.Result)
        {
            if (freeText != null)
            {
                var occurrences = formatForParsing.CountOccurrences(profanity.Content);

                if (occurrences > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }
}