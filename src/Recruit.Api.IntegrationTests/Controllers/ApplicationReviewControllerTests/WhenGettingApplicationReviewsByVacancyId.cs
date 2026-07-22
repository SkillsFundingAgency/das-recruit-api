using System.Net;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;
using SFA.DAS.Recruit.Contracts.ApiRequests;
using DomainStatus = SFA.DAS.Recruit.Api.Domain.Enums.ApplicationReviewStatus;
using ContractStatus = SFA.DAS.Recruit.Contracts.ApiResponses.ApplicationReviewStatus;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.ApplicationReviewControllerTests;

public class WhenGettingApplicationReviewsByVacancyId : BaseFixture
{
    [Test]
    public async Task Then_The_ApplicationReviews_Are_Returned()
    {
        // arrange
        var vacancy = Fixture.Create<VacancyEntity>();
        var applicationReviews = Fixture.CreateMany<ApplicationReviewEntity>(3)
            .Select(x =>
            {
                x.VacancyReference = vacancy.VacancyReference!.Value;
                return x;
            }).ToList();

        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(applicationReviews);

        // act
        var response = await Client.GetAsync(new GetVacanciesByidByVacancyIdApplicationreviewsApiRequest(vacancy.Id, null).GetUrl);
        var result = await response.Content.ReadAsAsync<List<GetApplicationReviewResponse>>();

        // assert
        response.EnsureSuccessStatusCode();
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
    }

    [Test]
    public async Task Then_The_ApplicationReviews_Are_Filtered_By_Status()
    {
        // arrange
        var vacancy = Fixture.Create<VacancyEntity>();
        var applicationReviews = Fixture.CreateMany<ApplicationReviewEntity>(3)
            .Select(x =>
            {
                x.VacancyReference = vacancy.VacancyReference!.Value;
                x.Status = DomainStatus.New;
                return x;
            }).ToList();
        applicationReviews[0].Status = DomainStatus.Successful;

        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(applicationReviews);

        // act
        var response = await Client.GetAsync(new GetVacanciesByidByVacancyIdApplicationreviewsApiRequest(vacancy.Id, [ContractStatus.New]).GetUrl);
        var result = await response.Content.ReadAsAsync<List<GetApplicationReviewResponse>>();

        // assert
        response.EnsureSuccessStatusCode();
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
    }

    [Test]
    public async Task Then_NotFound_Is_Returned_When_No_Matching_Vacancy()
    {
        // arrange
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet(Fixture.CreateMany<VacancyEntity>(3).ToList());
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(Fixture.CreateMany<ApplicationReviewEntity>(3).ToList());

        // act
        var response = await Client.GetAsync(new GetVacanciesByidByVacancyIdApplicationreviewsApiRequest(Guid.NewGuid(), null).GetUrl);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}