﻿using SFA.DAS.Recruit.Api.Models.Requests.ApplicationReview;
using SFA.DAS.Recruit.Api.Validators;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators;

[TestFixture]
internal class WhenValidatingPutApplicationReviewRequest
{
    [Test, MoqAutoData]
    public void Then_The_Request_Is_Valid(PutApplicationReviewRequest request)
    {
        // arrange
        var sut = new PutApplicationReviewRequestValidator(); // autofixture doesn't create this property
        
        // act
        var result = sut.Validate(request);
        
        // assert
        result.IsValid.Should().BeTrue();
    }
    
    [Test]
    public void Then_The_Request_Is_Invalid()
    {
        // arrange
        var sut = new PutApplicationReviewRequestValidator(); // autofixture doesn't create this property
        var request = new PutApplicationReviewRequest {
            Ukprn = 0,
            AccountId = 0,
            CandidateId = Guid.Empty,
            Status = null!,
            VacancyReference = 0,
            LegacyApplicationId = null,
            VacancyTitle = null!,
            AccountLegalEntityId = 0,
        };
        
        // act
        var result = sut.Validate(request);
        
        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(7);
    }
}