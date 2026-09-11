using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ApplicationReviewControllerTests;

public class WhenGettingVacancyApplicationsCountRequiringFeedback
{
    [Test, MoqAutoData]
    public async Task Then_The_Results_Are_Mapped_Correctly(
        List<long> vacancyIds,
        Dictionary<long, int> results,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController sut,
        CancellationToken token)
    {
        // arrange
        provider
            .Setup(x => x.GetVacancyApplicationsCountRequiringFeedback(vacancyIds, token))
            .ReturnsAsync(results);

        // act
        var actual = await sut.GetVacancyApplicationsCountRequiringFeedback(vacancyIds, token);
        var response = actual as Ok<DataResponse<List<VacancyApplicationsCountRequiringFeedback>>>;

        // assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        response.Value.Should().NotBeNull();
        response.Value.Data.Should().BeEquivalentTo(results, o => o
            .WithMapping<VacancyApplicationsCountRequiringFeedback>(x => x.Key, x => x.VacancyReference)
            .WithMapping<VacancyApplicationsCountRequiringFeedback>(x => x.Value, x => x.ApplicationsRequiringFeedbackCount)
        );
    }
}