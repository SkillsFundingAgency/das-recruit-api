using FluentValidation;
using SFA.DAS.InputValidation.Fluent.Extensions;
using SFA.DAS.Recruit.Api.Data.ReferenceData;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public class VacancyQualificationValidator : AbstractValidator<Qualification>
{
    public VacancyQualificationValidator(IProhibitedContentRepository profanityListProvider)
    {

        RuleFor(x => x.QualificationType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Select a qualification")
            .WithErrorCode("53")
            .WithState(_ => VacancyRuleSet.Qualifications)
            .Must(Reference.CandidateQualifications.Contains)
            .WithMessage("Invalid qualification type")
            .WithErrorCode("57")
            .WithState(_ => VacancyRuleSet.Qualifications)
            .WithState(_ => VacancyRuleSet.Qualifications);

        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage("Enter the subject")
            .WithErrorCode("54")
            .WithState(_ => VacancyRuleSet.Qualifications)
            .MaximumLength(50)
            .WithMessage("The qualification must not exceed {MaxLength} characters")
            .WithErrorCode("7")
            .WithState(_ => VacancyRuleSet.Qualifications)
            .ValidFreeTextCharacters()
            .WithMessage("Subject contains some invalid characters")
            .WithErrorCode("6")
            .WithState(_ => VacancyRuleSet.Qualifications)!
            .ProfanityCheck(profanityListProvider)
            .WithMessage("Subject must not contain a banned word or phrase.")
            .WithErrorCode("618")
            .WithState(_ => VacancyRuleSet.Qualifications)
            .WithState(_ => VacancyRuleSet.Qualifications);

        RuleFor(x => x.Grade)
            .NotEmpty()
            .WithMessage("Enter the grade")
            .WithErrorCode("55")
            .WithState(_ => VacancyRuleSet.Qualifications)
            .MaximumLength(30)
            .WithMessage("The grade should be no longer than {MaxLength} characters")
            .WithErrorCode("7")
            .WithState(_ => VacancyRuleSet.Qualifications)
            .ValidFreeTextCharacters()
            .WithMessage("Grade contains some invalid characters")
            .WithErrorCode("6")
            .WithState(_ => VacancyRuleSet.Qualifications)!
            .ProfanityCheck(profanityListProvider)
            .WithMessage("Grade must not contain a banned word or phrase.")
            .WithErrorCode("619")
            .WithState(_ => VacancyRuleSet.Qualifications);

        When(x => x.QualificationType != null && x.QualificationType.Contains("BTEC"), () =>
        {
            RuleFor(x => x.Level)
                .NotEmpty()
                .WithMessage("Select the BTEC level")
                .WithErrorCode("1115")
                .WithState(_ => VacancyRuleSet.Qualifications);
        });
        When(x => x.QualificationType != null && x.QualificationType.Contains("Other"), () =>
        {
            RuleFor(x => x.OtherQualificationName)
                .NotEmpty()
                .WithMessage("Enter the name of the qualification")
                .WithErrorCode("2115")
                .WithState(_ => VacancyRuleSet.Qualifications);
        });

        RuleFor(x => x.Weighting)
            .NotEmpty()
            .WithMessage("Select if this qualification is essential or desirable")
            .WithErrorCode("56")
            .WithState(_ => VacancyRuleSet.Qualifications);
    }
}