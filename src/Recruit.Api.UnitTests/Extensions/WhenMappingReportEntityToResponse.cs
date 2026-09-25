using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.UnitTests.Extensions;

internal class WhenMappingReportEntityToResponse
{
    [Test, RecursiveMoqAutoData]
    public void ToResponse_MapsStatus_WhenStatusIsSet(ReportEntity entity)
    {
        entity.Status = ReportStatus.Failed;

        var result = entity.ToResponse();

        result.Status.Should().Be(ReportStatus.Failed);
    }

    [Test, RecursiveMoqAutoData]
    public void ToResponse_DefaultsStatusToGenerated_WhenStatusIsNull(ReportEntity entity)
    {
        entity.Status = null;

        var result = entity.ToResponse();

        result.Status.Should().Be(ReportStatus.Generated);
    }
}
