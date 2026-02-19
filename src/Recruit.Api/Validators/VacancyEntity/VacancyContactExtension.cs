using FluentValidation;
using SFA.DAS.InputValidation.Fluent.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyContactExtension
{
    public static void VacancyContactDetailValidation(this IRuleBuilderInitial<PutVacancyRequest, ContactDetail?> rule,
        IProhibitedContentRepository profanityListProvider)
    {
        rule.ChildRules(r =>
        {
            r.RuleFor(x => x.Name)
                .MaximumLength(100)
                .WithMessage("Contact name must not exceed {MaxLength} characters")
                .WithErrorCode("90")
                .WithState(_ => VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails)
                .ValidFreeTextCharacters()
                .WithMessage("Contact name contains some invalid characters")
                .WithErrorCode("91")
                .WithState(_ => VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails)!
                .ProfanityCheck(profanityListProvider)
                .WithMessage("Contact name must not contain a banned word or phrase")
                .WithErrorCode("615")
                .WithState(_ => VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails);

            r.RuleFor(x => x.Email)
                .MaximumLength(100)
                .WithMessage("Email address must not exceed {MaxLength} characters")
                .WithErrorCode("92")
                .WithState(_ => VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails)
                .ValidFreeTextCharacters()
                .WithMessage("Email address contains some invalid characters")
                .WithErrorCode("93")
                .WithState(_ => VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails)
                .Matches(ValidationConstants.EmailAddressRegex)
                .WithMessage("Email address must be in a valid format")
                .WithErrorCode("94")
                .WithState(_ => VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails)
                .When(v => !string.IsNullOrEmpty(v.Email))
                .ProfanityCheck(profanityListProvider)
                .WithMessage("Email address must not contain a banned word or phrase")
                .WithErrorCode("616")
                .WithState(_ => VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails);

            r.RuleFor(x => x.Phone)
                .MaximumLength(16)
                .WithMessage("Contact number must not exceed {MaxLength} digits")
                .WithErrorCode("95")
                .WithState(_ => VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails)
                .MinimumLength(8)
                .WithMessage("Contact number must be more than {MinLength} digits")
                .WithErrorCode("96")
                .WithState(_ => VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails)
                .Matches(ValidationConstants.PhoneNumberRegex)
                .WithMessage("Contact number contains some invalid characters")
                .WithErrorCode("97")
                .WithState(_ => VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails)
                .When(v => !string.IsNullOrEmpty(v.Phone))
                .WithState(_ => VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails);
        }).RunCondition(VacancyRuleSet.EmployerContactDetails | VacancyRuleSet.ProviderContactDetails);
    }
}