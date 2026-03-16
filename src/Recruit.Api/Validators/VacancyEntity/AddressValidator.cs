using FluentValidation;
using SFA.DAS.InputValidation.Fluent.Extensions;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public class AddressValidator : AbstractValidator<Address> 
{
    public AddressValidator()
    {
        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .WithMessage("Enter the address where the apprentice will work")
            .WithErrorCode("5")
            .WithState(_=>VacancyRuleSet.EmployerAddress)
            .ValidFreeTextCharacters()
            .WithMessage("Address line 1 contains some invalid characters")
            .WithErrorCode("6")
            .WithState(_=>VacancyRuleSet.EmployerAddress)
            .MaximumLength(100)
            .WithMessage("Address line 1 must not be more than {MaxLength} characters")
            .WithErrorCode("7")
            .WithState(_=>VacancyRuleSet.EmployerAddress);

        RuleFor(x => x.AddressLine2)
            .ValidFreeTextCharacters()
            .WithMessage("Address line 2 contains some invalid characters")
            .WithErrorCode("6")
            .WithState(_=>VacancyRuleSet.EmployerAddress)
            .MaximumLength(100)
            .WithMessage("Address line 2 must not be more than {MaxLength} characters")
            .WithErrorCode("7")
            .WithState(_=>VacancyRuleSet.EmployerAddress);
            
        RuleFor(x => x.AddressLine3)
            .ValidFreeTextCharacters()
            .WithMessage("Address line 3 contains some characters")
            .WithErrorCode("6")
            .WithState(_=>VacancyRuleSet.EmployerAddress)
            .MaximumLength(100)
            .WithMessage("Address line 3 must not be more than {MaxLength} characters")
            .WithErrorCode("7")
            .WithState(_=>VacancyRuleSet.EmployerAddress);
            
        RuleFor(x => x.AddressLine4)
            .ValidFreeTextCharacters()
            .WithMessage("Address line 4 contains some invalid characters")
            .WithErrorCode("6")
            .WithState(_=>VacancyRuleSet.EmployerAddress)
            .MaximumLength(100)
            .WithMessage("Address line 4 must not be more than {MaxLength} characters")
            .WithErrorCode("7")
            .WithState(_=>VacancyRuleSet.EmployerAddress);

        RuleFor(x => x.Postcode)
            .NotEmpty()
            .WithMessage("Enter the postcode")
            .WithErrorCode("8")
            .WithState(_=>VacancyRuleSet.EmployerAddress)
            .ValidPostCode()
            .When(x => !string.IsNullOrEmpty(x.Postcode), ApplyConditionTo.CurrentValidator)
            .WithMessage("'{PropertyName}' is not in a valid format")
            .WithErrorCode("9")
            .WithState(_=>VacancyRuleSet.EmployerAddress);
    }
    
}