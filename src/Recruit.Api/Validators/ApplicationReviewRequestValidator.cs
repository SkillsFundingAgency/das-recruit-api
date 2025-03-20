using FluentValidation;
using SFA.DAS.Recruit.Api.Models.Requests;

namespace SFA.DAS.Recruit.Api.Validators;

public class ApplicationReviewRequestValidator : AbstractValidator<ApplicationReviewRequest>
{
    public ApplicationReviewRequestValidator()
    {
        RuleFor(req => req.Id).NotNull().NotEmpty();
        RuleFor(req => req.Id).NotNull().NotEmpty();
        RuleFor(req => req.Ukprn).NotNull().NotEmpty();
        RuleFor(req => req.AccountId).NotNull().NotEmpty();
        RuleFor(req => req.Owner).NotNull().NotEmpty();
        RuleFor(req => req.CandidateId).NotNull().NotEmpty();
        RuleFor(req => req.CreatedDate).NotNull().NotEmpty();
        RuleFor(req => req.SubmittedDate).NotNull().NotEmpty();
        RuleFor(req => req.Status).NotNull().NotEmpty();
        RuleFor(req => req.StatusUpdatedByUserId).NotNull().NotEmpty();
        RuleFor(req => req.VacancyReference).NotNull().NotEmpty();
        RuleFor(req => req.LegacyApplicationId).NotNull().NotEmpty();
    }
}