using FluentAssertions;
using NUnit.Framework;
using Recruit.Api.Domain.Entities;
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

        response.Should().BeEquivalentTo(entity);
    }

    [Test]
    public void ToResponse_ReturnsNull_WhenEntityIsNull()
    {
        ApplicationReviewEntity? entity = null;

        var result  = entity?.ToResponse();

        result.Should().BeNull();
    }
}
