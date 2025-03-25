using FluentAssertions;
using NUnit.Framework;
using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Extensions;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Extensions;

[TestFixture]
public class WhenApplicationReviewResponseToResponse
{
    [Test, MoqAutoData]
    public void ToResponse_ReturnsCorrectResponse_WhenEntityIsValid(ApplicationReviewEntity entity)
    {
        var response = entity.ToResponse();

        response.Should().BeEquivalentTo(entity, options => 
            options.Excluding(x => x.Status));
        response.Status.Should().Be((ApplicationStatus)entity.Status);
    }

    [Test]
    public void ToResponse_ReturnsNull_WhenEntityIsNull()
    {
        ApplicationReviewEntity? entity = null;

        Action act = () => entity?.ToResponse();

        act.Should().Throw<NullReferenceException>();
    }
}
