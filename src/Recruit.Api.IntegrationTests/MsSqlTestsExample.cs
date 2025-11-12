using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

public class MsSqlTestsExample: MsSqlBaseFixture
{
    [Test, Retry(3)]
    public async Task Example_Test()
    {
        // arrange
        var items = await TestData.CreateMany<VacancyEntity>(10, x =>
        {
            foreach (var vacancy in x)
            {
                vacancy.ClosedDate = null;
                vacancy.Status = VacancyStatus.Draft;
                vacancy.SubmittedByUserId = null;
                vacancy.SubmittedDate = null;
                vacancy.ReviewRequestedDate = null;
                vacancy.ReviewRequestedByUserId = null;
                vacancy.ApprovedDate = null;
            }
        });
        var expected = items[1];

        // act
        var response = await Measure.ThisAsync(async () => await Client.GetAsync($"{RouteNames.Vacancies}/{expected.Id}"));
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        response.EnsureSuccessStatusCode();
        vacancy.Should().NotBeNull();
        vacancy.Should().BeEquivalentTo(expected.ToGetResponse(), opt => opt.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>());
    }
}