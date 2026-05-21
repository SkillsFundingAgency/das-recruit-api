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
            .Setup(x => x.GetMany(ReportOwnerType.Qa, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        // act
        var result = await sut.GetMany(repository.Object, ReportOwnerType.Qa, token);
        var payload = (result as Ok<List<Report>>)?.Value;

        // assert
        repository.Verify(x => x.GetMany(ReportOwnerType.Qa, token), Times.Once());
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entities, options => options.ExcludingMissingMembers());
        payload.ForEach(x => x.OwnerType.Should().Be(ReportOwnerType.Qa));
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_OwnerType_Query_Param_Is_Passed_To_Repository(
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
            reportEntity.OwnerType = ReportOwnerType.Provider;
            reportEntity.CreatedDate = DateTime.UtcNow;
        }
        repository
            .Setup(x => x.GetMany(ReportOwnerType.Provider, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        // act
        var result = await sut.GetMany(repository.Object, ReportOwnerType.Provider, token);
        var payload = (result as Ok<List<Report>>)?.Value;

        // assert
        repository.Verify(x => x.GetMany(ReportOwnerType.Provider, token), Times.Once());
        payload.Should().NotBeNull();
        payload.ForEach(x => x.OwnerType.Should().Be(ReportOwnerType.Provider));
    }
}