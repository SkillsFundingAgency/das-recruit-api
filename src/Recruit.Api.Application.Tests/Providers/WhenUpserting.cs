using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Recruit.Api.Application.Providers;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Data.Models;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Application.Tests.Providers
{
    [TestFixture]
    public class WhenUpserting
    {
        [Test, MoqAutoData]
        public async Task Upsert_Returns_Repository_Value(
            ApplicationReviewEntity existingEntity,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            var expectedTuple = UpsertResult.Create(existingEntity, true);

            repositoryMock
                .Setup(r => r.Upsert(existingEntity, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTuple);

            // Act
            var result = await provider.Upsert(existingEntity);

            // Assert
            result.Should().BeEquivalentTo(expectedTuple);
        }
    }
}