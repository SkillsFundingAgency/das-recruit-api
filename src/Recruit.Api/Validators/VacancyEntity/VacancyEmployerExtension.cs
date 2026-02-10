using FluentValidation;
using SFA.DAS.InputValidation.Fluent.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyEmployerExtension
{
    public static void ValidateOrganisationCheck(this IRuleBuilderInitial<Vacancy, Vacancy> rule, IProhibitedContentRepository profanityListProvider)
    {
        rule.ChildRules(c =>
        {
            c.RuleFor(x => x.EmployerName)
                .NotEmpty()
                .WithMessage((vacancy, value) => $"Select the employer name you want on your {(vacancy.OwnerType == OwnerType.Employer ? "advert" : "vacancy")}")
                .WithErrorCode("4")
                .WithState(_ => VacancyRuleSet.EmployerName)
                .RunCondition(VacancyRuleSet.EmployerName);
            
            c.RuleFor(x => x.LegalEntityName)
                .NotEmpty()
                .WithMessage("You must select one organisation")
                .WithErrorCode("400")
                .WithState(_ => VacancyRuleSet.LegalEntityName)
                .RunCondition(VacancyRuleSet.LegalEntityName);
            
            c.When(v => v.EmployerNameOption == EmployerNameOption.TradingName, () =>
                c.RuleFor(x => x.EmployerName)
                    .NotEmpty()
                    .WithMessage("Enter the trading name")
                    .WithErrorCode("401")
                    .WithState(_ => VacancyRuleSet.TradingName)
                    .MaximumLength(100)
                    .WithMessage("The trading name must not exceed {MaxLength} characters")
                    .WithErrorCode("402")
                    .WithState(_ => VacancyRuleSet.TradingName)
                    .ValidFreeTextCharacters()
                    .WithMessage("The trading name contains some invalid characters")
                    .WithErrorCode("403")
                    .WithState(_ => VacancyRuleSet.TradingName)!
                    .ProfanityCheck(profanityListProvider)
                    .WithMessage("Trading name must not contain a banned word or phrase.")
                    .WithErrorCode("602")
                    .WithState(_ => VacancyRuleSet.TradingName)
                    .RunCondition(VacancyRuleSet.TradingName));

            c.When(v => v.EmployerNameOption == EmployerNameOption.Anonymous, () =>
                c.RuleFor(x => x.EmployerName)
                    .NotEmpty()
                    .WithMessage("Enter a brief description of what the employer does")
                    .WithErrorCode("405")
                    .WithState(_ => VacancyRuleSet.EmployerNameOption)
                    .MaximumLength(100)
                    .WithMessage("The description must not be more than {MaxLength} characters")
                    .WithErrorCode("406")
                    .WithState(_ => VacancyRuleSet.EmployerNameOption)
                    .ValidFreeTextCharacters()
                    .WithMessage("The description contains some invalid characters")
                    .WithErrorCode("407")
                    .WithState(_ => VacancyRuleSet.EmployerNameOption)!
                    .ProfanityCheck(profanityListProvider)
                    .WithMessage("Description must not contain a banned word or phrase.")
                    .WithErrorCode("603")
                    .WithState(_ => VacancyRuleSet.EmployerNameOption)
                    .RunCondition(VacancyRuleSet.EmployerNameOption));

            c.When(v => v.EmployerNameOption == EmployerNameOption.Anonymous, () =>
                c.RuleFor(x => x.AnonymousReason)
                    .NotEmpty()
                    .WithMessage((vacancy, value) => $"Enter why you want your {(vacancy.OwnerType == OwnerType.Employer ? "advert" : "vacancy")} to be anonymous")
                    .WithErrorCode("408")
                    .WithState(_ => VacancyRuleSet.EmployerNameOption)
                    .MaximumLength(200)
                    .WithMessage("The reason must not be more than {MaxLength} characters")
                    .WithErrorCode("409")
                    .WithState(_ => VacancyRuleSet.EmployerNameOption)
                    .ValidFreeTextCharacters()
                    .WithMessage("The reason contains some invalid characters")
                    .WithErrorCode("410")
                    .WithState(_ => VacancyRuleSet.EmployerNameOption)!
                    .ProfanityCheck(profanityListProvider)
                    .WithMessage("Reason must not contain a banned word or phrase.")
                    .WithErrorCode("604")
                    .WithState(_ => VacancyRuleSet.EmployerNameOption)
                    .RunCondition(VacancyRuleSet.EmployerNameOption));

            c.RuleFor(x => x.EmployerNameOption)
                .NotEmpty()
                    .WithMessage((vacancy, value) => $"Select the employer name you want on your {(vacancy.OwnerType == OwnerType.Employer ? "advert" : "vacancy")}")
                    .WithErrorCode("404")
                .WithState(_ => VacancyRuleSet.EmployerNameOption)
                .RunCondition(VacancyRuleSet.EmployerNameOption);

            c.When(v => v.EmployerLocationOption is null, () =>
            {
                c.RuleFor(x => x.EmployerLocations)
                    .Cascade(CascadeMode.Stop)
                    .NotNull()
                    .WithMessage("You must provide an employer location")
                    .WithErrorCode("98")
                    .WithState(_ => VacancyRuleSet.EmployerAddress)
                    .Must(x => x is { Count: 1 })
                    .WithMessage("You must provide an employer location")
                    .WithErrorCode("98")
                    .WithState(_ => VacancyRuleSet.EmployerAddress)
                    .RunCondition(VacancyRuleSet.EmployerAddress);
                
                c.RuleForEach(x => x.EmployerLocations)
                    .SetValidator(new AddressValidator())
                    .WithState(_ => VacancyRuleSet.EmployerNameOption)
                    .RunCondition(VacancyRuleSet.EmployerAddress);
            });

            c.When(v => v.EmployerLocationOption == AvailableWhere.OneLocation, () =>
            {
                c.RuleFor(x => x.EmployerLocations)
                    .Must(x => x is { Count: 1 })
                    .WithMessage("Select a location")
                    .WithState(_ => VacancyRuleSet.EmployerAddress)
                    .RunCondition(VacancyRuleSet.EmployerAddress);
                
                c.RuleForEach(x => x.EmployerLocations)
                    .SetValidator(new AddressValidator())
                    .WithState(_ => VacancyRuleSet.EmployerAddress)
                    .RunCondition(VacancyRuleSet.EmployerAddress);
            });
            
            c.When(v => v.EmployerLocationOption == AvailableWhere.MultipleLocations, () =>
            {
                c.RuleFor(x => x.EmployerLocations)
                    .NotNull()
                    .Must(x => x.Count is >=2 and <=10)
                    .WithMessage("Select between 2 and 10 locations")
                    .WithState(_ => VacancyRuleSet.EmployerAddress)
                    .RunCondition(VacancyRuleSet.EmployerAddress);
                
                c.RuleForEach(x => x.EmployerLocations)
                    .SetValidator(new AddressValidator())
                    .RunCondition(VacancyRuleSet.EmployerAddress);
            });

            c.When(v => v.EmployerLocationOption == AvailableWhere.AcrossEngland, () =>
            {
                c.RuleFor(x => x.EmployerLocationInformation)
                    .NotNull()
                    .WithMessage("Add more information about where the apprentice will work")
                    .WithState(_ => VacancyRuleSet.EmployerAddress)
                    .MaximumLength(500)
                    .WithMessage("Information about where the apprentice will work must be 500 characters or less")
                    .WithState(_ => VacancyRuleSet.EmployerAddress)
                    .ProfanityCheck(profanityListProvider)
                    .WithMessage($"Additional information must not contain a banned word or phrase")
                    .WithState(_ => VacancyRuleSet.EmployerAddress)
                    .RunCondition(VacancyRuleSet.EmployerAddress);
            });
        });
    }
}