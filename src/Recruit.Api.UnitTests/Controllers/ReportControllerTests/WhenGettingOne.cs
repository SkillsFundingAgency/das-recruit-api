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
internal class WhenGettingOne
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Vacancy_Entities_Are_Returned(
        Guid reportId,
        ReportEntity reportEntity,
        Mock<IReportRepository> repository,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        reportEntity.DynamicCriteria = JsonConvert.SerializeObject(new ReportCriteria {
            FromDate = DateTime.UtcNow.AddDays(-1),
            ToDate = DateTime.UtcNow.AddDays(1),
            Ukprn = 123456
        });
        reportEntity.Id = reportId;
        reportEntity.CreatedDate = DateTime.UtcNow;
        reportEntity.OwnerType = ReportOwnerType.Provider;
        repository
            .Setup(x => x.GetOneAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reportEntity);

        // act
        var result = await sut.GetOne(repository.Object, reportId, token);
        var payload = (result as Ok<Report>)?.Value;

        // assert
        repository.Verify(x => x.GetOneAsync(reportId, token), Times.Once());
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(reportEntity, options => options.ExcludingMissingMembers());
    }
}