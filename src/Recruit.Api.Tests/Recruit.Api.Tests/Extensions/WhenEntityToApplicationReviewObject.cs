using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Recruit.Api.Extensions;
using SFA.DAS.Recruit.Api.Models.Requests;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Extensions;

[TestFixture]
public class WhenEntityToApplicationReviewObject
{
    [Test, MoqAutoData]
    public void ToEntity_ReturnsCorrectResponse(ApplicationReviewRequest request)
    {
        var entity = request.ToEntity();

        entity.Should().BeEquivalentTo(request, options => 
            options.Excluding(x => x.Status));
        entity.Status.Should().Be((short)request.Status);
    }
}