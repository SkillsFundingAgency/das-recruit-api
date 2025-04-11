using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Extensions;

[TestFixture]
internal class WhenEntityToApplicationReviewObject
{
    [Test, MoqAutoData]
    public void ToEntity_ReturnsCorrectResponse(PutApplicationReviewRequest request)
    {
        var entity = request.ToEntity(Guid.NewGuid());

        entity.Should().BeEquivalentTo(request);
    }
}