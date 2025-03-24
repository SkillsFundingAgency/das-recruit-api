using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Recruit.Api.Data;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Database.Tests.DatabaseMock;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Database.Tests.ApplicationReview
{
    [TestFixture]
    public class WhenUpdatingApplicationReview
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_The_Application_Is_Updated(
            ApplicationReviewEntity entity,
            [Frozen] Mock<IRecruitDataContext> context,
            ApplicationReviewRepository repository)
        {
            context.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(new List<ApplicationReviewEntity> { entity });
            var actual = await repository.Update(entity);

            actual.Should().BeEquivalentTo(entity);
            context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}