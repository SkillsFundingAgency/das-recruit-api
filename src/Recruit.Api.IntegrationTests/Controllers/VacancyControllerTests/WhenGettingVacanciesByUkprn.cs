using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

public class WhenGettingVacanciesByUkprn: BaseFixture
{
    [Test]
    public async Task Then_The_Vacancies_Are_Returned()
    {
        // arrange
        int ukprn = Fixture.Create<int>();
        var items = Fixture.CreateMany<VacancyEntity>(100)
            .Select(x =>
            {
                x.Ukprn = ukprn;
                return x;
            }).ToList();
        var applicationReviewItems = Fixture.CreateMany<ApplicationReviewEntity>(100)
            .Select(x =>
            {
                x.Ukprn = ukprn;
                return x;
            }).ToList();

        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet(items);
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(applicationReviewItems);

        // act
        var response = await Client.GetAsync($"{RouteNames.Provider}/{ukprn}/{RouteElements.Vacancies}");
        var pagedResponse = await response.Content.ReadAsAsync<PagedResponse<VacancySummary>>();

        // assert
        response.EnsureSuccessStatusCode();
        pagedResponse.Should().NotBeNull();
        pagedResponse.PageInfo.PageIndex.Should().Be(1);
        pagedResponse.PageInfo.PageSize.Should().Be(25);
        pagedResponse.PageInfo.TotalCount.Should().Be(100);
        pagedResponse.PageInfo.TotalPages.Should().Be(4);
        pagedResponse.Items.Count().Should().Be(25);
    }
    
    [Test]
    public async Task Then_The_Correct_Paged_Vacancies_Are_Returned()
    {
        // arrange
        int ukprn = Fixture.Create<int>();
        var items = Fixture.CreateMany<VacancyEntity>(100)
            .Select(x =>
            {
                x.Ukprn = ukprn;
                return x;
            }).ToList();
        var applicationReviewItems = Fixture.CreateMany<ApplicationReviewEntity>(100)
            .Select(x =>
            {
                x.Ukprn = ukprn;
                return x;
            }).ToList();
        var expectedItems = items.OrderBy(x => x.CreatedDate).Skip(20).Take(10).Select(x => x.ToGetResponse());
        
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet(items);
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(applicationReviewItems);

        // act
        var response = await Client.GetAsync($"{RouteNames.Provider}/{ukprn}/{RouteElements.Vacancies}?page=3&pageSize=10&sortOrder=Asc");
        var pagedResponse = await response.Content.ReadAsAsync<PagedResponse<VacancySummary>>();

        // assert
        response.EnsureSuccessStatusCode();
        pagedResponse.Should().NotBeNull();
        pagedResponse.PageInfo.PageIndex.Should().Be(3);
        pagedResponse.PageInfo.PageSize.Should().Be(10);
        pagedResponse.PageInfo.TotalCount.Should().Be(100);
        pagedResponse.PageInfo.TotalPages.Should().Be(10);
        pagedResponse.Items.Count().Should().Be(expectedItems.Count());
    }
}