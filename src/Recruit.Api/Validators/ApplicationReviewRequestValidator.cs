﻿using FluentValidation;
using SFA.DAS.Recruit.Api.Models.Requests;

namespace SFA.DAS.Recruit.Api.Validators;

public class PutApplicationReviewRequestValidator : AbstractValidator<PutApplicationReviewRequest>
{
    public PutApplicationReviewRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(req => req.Ukprn).NotNull().NotEmpty();
        RuleFor(req => req.AccountId).NotNull().NotEmpty();
        RuleFor(req => req.CandidateId).NotNull().NotEmpty();
        RuleFor(req => req.CreatedDate).NotNull().NotEmpty();
        RuleFor(req => req.Status).NotNull().NotEmpty();
        RuleFor(req => req.VacancyReference).NotNull().NotEmpty();
        RuleFor(req => req.VacancyTitle).NotNull().NotEmpty();
        RuleFor(req => req.AccountLegalEntityId).NotNull().NotEmpty();
        RuleFor(req => req.HasEverBeenEmployerInterviewing).NotNull();
    }
}