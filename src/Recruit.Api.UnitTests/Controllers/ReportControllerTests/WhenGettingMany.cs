using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReportControllerTests;
[TestFixture]
internal class WhenGettingMany
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Vacancy_Entities_Are_Returned(
        List<ReportEntity> entities,
        Mock<IReportRepository> repository,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        foreach (var reportEntity in entities)
        {
            reportEntity.DynamicCriteria = JsonConvert.SerializeObject(new ReportCriteria {
                FromDate = DateTime.UtcNow.AddDays(-1),
                ToDate = DateTime.UtcNow.AddDays(1),
            });
            reportEntity.OwnerType = ReportOwnerType.Qa;
            reportEntity.CreatedDate = DateTime.UtcNow;
        }
        repository
            .Setup(x => x.GetMany(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        // act
        var result = await sut.GetMany(repository.Object, token);
        var payload = (result as Ok<List<Report>>)?.Value;

        // assert
        repository.Verify(x => x.GetMany(token), Times.Once());
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entities, options => options.ExcludingMissingMembers());
        payload.ForEach(x => x.OwnerType.Should().Be(ReportOwnerType.Qa));
    }
}