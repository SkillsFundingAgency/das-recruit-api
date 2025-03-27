using SFA.DAS.Recruit.Api.Extensions;
using SFA.DAS.Recruit.Api.Models.Requests;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Extensions;

[TestFixture]
public class WhenEntityToApplicationReviewObject
{
    [Test, MoqAutoData]
    public void ToEntity_ReturnsCorrectResponse(PutApplicationReviewRequest request)
    {
        var entity = request.ToEntity(Guid.NewGuid());

        entity.Should().BeEquivalentTo(request);
    }
}