using FluentValidation;
using SFA.DAS.InputValidation.Fluent.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyApplicationExtension
{
    public static void VacancyApplicationMethodValidation(this IRuleBuilderInitial<Vacancy, Vacancy> rule, IProhibitedContentRepository profanityListProvider, IExternalWebsiteHealthCheckService externalWebsiteHealthCheckService)
    {
        rule.ChildRules(c =>
        {
            c.RuleFor(x => x.ApplicationMethod)
                .NotEmpty()
                .WithMessage("Select a website for applications")
                .WithErrorCode("85")
                .WithState(_ => VacancyRuleSet.ApplicationMethod)
                .IsInEnum()
                .WithMessage("Select a website for applications")
                .WithErrorCode("85")
                .WithState(_ => VacancyRuleSet.ApplicationMethod)
                .RunCondition(VacancyRuleSet.ApplicationMethod);

            c.When(x => x.ApplicationMethod == ApplicationMethod.ThroughFindAnApprenticeship, () =>
            {
                c.RuleFor(x => x.ApplicationUrl)
                    .Empty()
                    .WithMessage($"Application website link must be empty when apply through Find an apprenticeship service option is specified")
                    .WithErrorCode("86")
                    .WithState(_ => VacancyRuleSet.ApplicationMethod)
                    .RunCondition(VacancyRuleSet.ApplicationMethod);

                c.RuleFor(x => x.ApplicationInstructions)
                    .Empty()
                    .WithMessage($"Application process must be empty when apply through Find an apprenticeship service option is specified")
                    .WithErrorCode("89")
                    .WithState(_ => VacancyRuleSet.ApplicationMethod)
                    .RunCondition(VacancyRuleSet.ApplicationMethod);
            });

            c.When(x => x.ApplicationMethod == ApplicationMethod.ThroughExternalApplicationSite, () =>
            {
                c.RuleFor(x=>x.ApplicationUrl!).VacancyApplicationUrlValidation(externalWebsiteHealthCheckService);
                c.RuleFor(x=>x.ApplicationInstructions!).VacancyApplicationInstructionsValidation(profanityListProvider);
            });
        });
        
    }
    private static void VacancyApplicationUrlValidation(this IRuleBuilderInitial<Vacancy, string> rule, IExternalWebsiteHealthCheckService externalWebsiteHealthCheckService)
    {
        rule
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter the application website link")
            .WithErrorCode("85")
            .WithState(_ => VacancyRuleSet.ApplicationMethod)
            .MaximumLength(2000)
            .WithMessage("Application website link must not exceed {MaxLength} characters")
            .WithErrorCode("84")
            .WithState(_ => VacancyRuleSet.ApplicationMethod)
            .Must(FluentExtensions.BeValidWebUrl)
            .WithMessage("Application website link must be a valid link")
            .WithErrorCode("86")
            .WithState(_ => VacancyRuleSet.ApplicationMethod)
            .MustBeValidWebsiteAsync(externalWebsiteHealthCheckService)
            .WithMessage("Enter a valid website address")
            .WithErrorCode("86.1")
            .WithState(_ => VacancyRuleSet.ApplicationMethod)
            .RunCondition(VacancyRuleSet.ApplicationMethod);
    }

    private static void VacancyApplicationInstructionsValidation(this IRuleBuilderInitial<Vacancy, string> rule, IProhibitedContentRepository profanityListProvider)
    {
        rule
            .MaximumLength(500)
            .WithMessage("How to apply must not exceed {MaxLength} characters")
            .WithErrorCode("88")
            .WithState(_ => VacancyRuleSet.ApplicationMethod)
            .ValidFreeTextCharacters()
            .WithMessage("How to apply contains some invalid characters")
            .WithErrorCode("89")
            .WithState(_ => VacancyRuleSet.ApplicationMethod)!
            .ProfanityCheck(profanityListProvider)
            .WithMessage("How to apply must not contain a banned word or phrase.")
            .WithErrorCode("612")
            .WithState(_ => VacancyRuleSet.ApplicationMethod)
            .RunCondition(VacancyRuleSet.ApplicationMethod);
    }
}