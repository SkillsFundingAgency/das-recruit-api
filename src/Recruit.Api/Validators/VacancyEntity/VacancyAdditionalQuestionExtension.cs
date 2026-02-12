using FluentValidation;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyAdditionalQuestionExtension
{
    public static void ValidateAdditionalQuestionCheck(this IRuleBuilderInitial<Vacancy, string> rule,
        IProhibitedContentRepository profanityListProvider, VacancyRuleSet vacancyRuleSet)
    {   
        rule
            .MaximumLength(250)
            .WithMessage(x =>
            {
                var questionCount = x.ApprenticeshipType.GetValueOrDefault() == ApprenticeshipTypes.Foundation ? 3 : 4;
                return $"Question {questionCount - 1} must not exceed 250 characters.";
            })
            .WithErrorCode("321")
            .WithState(_ => vacancyRuleSet)
            .ProfanityCheck(profanityListProvider)
            .WithMessage("Questions must not contain a restricted word")
            .WithErrorCode("322")
            .WithState(_ => vacancyRuleSet)
            .Must(s => !string.IsNullOrEmpty(s) && s.Contains('?'))
            .WithMessage(x =>
            {
                var questionCount = x.ApprenticeshipType.GetValueOrDefault() == ApprenticeshipTypes.Foundation ? 3 : 4;
                return $"Question {questionCount - 1} must include a question mark (‘?’).";
            })
            .WithErrorCode("340")
            .WithState(_ => vacancyRuleSet)
            .RunCondition(vacancyRuleSet);
    }
}