using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.VacancyAnalytics;

namespace SFA.DAS.Recruit.Api.UnitTests.Extensions;
[TestFixture]
internal class WhenMappingVacancyRequestToEntity
{
    [Test, MoqAutoData]
    public void Then_Maps_Request_To_Entity(
        PutVacancyAnalyticsRequest request)
    {
        // Arrange
        long vacancyReference = 9876;

        var before = DateTime.UtcNow;

        // Act
        var entity = request.ToEntity(vacancyReference);

        var after = DateTime.UtcNow;

        // Assert
        entity.VacancyReference.Should().Be(vacancyReference);

        entity.UpdatedDate.Should().BeOnOrAfter(before);
        entity.UpdatedDate.Should().BeOnOrBefore(after);

        entity.Analytics.Should().Be(request.ToJson());
    }
}
