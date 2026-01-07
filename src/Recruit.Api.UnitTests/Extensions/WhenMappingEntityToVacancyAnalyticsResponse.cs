using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.UnitTests.Extensions;
[TestFixture]
internal class WhenMappingEntityToVacancyAnalyticsResponse
{
    [Test, MoqAutoData]
    public void ToResponse_Should_Map_All_Fields(List<VacancyAnalytics> vacancyAnalytics)
    {
        // Arrange
        var entity = new VacancyAnalyticsEntity {
            VacancyReference = 1234,
            UpdatedDate = new DateTime(2024, 1, 1),
            Analytics = System.Text.Json.JsonSerializer.Serialize(vacancyAnalytics)
        };

        // Force parsing so AnalyticsData is populated
        var _ = entity.AnalyticsData;

        // Act
        var response = entity.ToResponse();

        // Assert
        response.VacancyReference.Should().Be(1234);
        response.UpdatedDate.Should().Be(new DateTime(2024, 1, 1));

        response.Analytics.Should().HaveCount(3);
    }

    [Test]
    public void ToResponse_Should_Handle_Empty_Analytics()
    {
        // Arrange
        var entity = new VacancyAnalyticsEntity {
            VacancyReference = 456,
            UpdatedDate = DateTime.UtcNow,
            Analytics = "[]"
        };

        // Act
        var response = entity.ToResponse();

        // Assert
        response.Analytics.Should().BeEmpty();
    }

    [Test]
    public void ToResponse_Should_Handle_Invalid_Json_By_Returning_Empty_List()
    {
        // Arrange
        var entity = new VacancyAnalyticsEntity {
            VacancyReference = 789,
            UpdatedDate = DateTime.UtcNow,
            Analytics = "{invalid json}"
        };

        // Act
        var response = entity.ToResponse();

        // Assert
        response.Analytics.Should().NotBeNull();
        response.Analytics.Should().BeEmpty();
    }
}
