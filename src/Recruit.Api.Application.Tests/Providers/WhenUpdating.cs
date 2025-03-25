using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Recruit.Api.Application.Models.ApplicationReview;
using Recruit.Api.Application.Providers;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Application.Tests.Providers
{
    [TestFixture]
    public class WhenUpdating
    {
        [Test, MoqAutoData]
        public async Task Update_ShouldReturnUpdatedEntity_WhenEntityExists(
            Guid id,
            ApplicationReviewEntity existingEntity,
            ApplicationReviewEntity updatedEntity,
            CancellationToken token,
            List<ApplicationReviewEntity> entities,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            var patchDocument = new PatchApplication {
                Id = id,
                Patch = new Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<ApplicationReview>()
            };
            existingEntity.Id = id;
            existingEntity.Status = "1";

            updatedEntity.Id = id;
            updatedEntity.Status = "2";

            repositoryMock.Setup(r => r.GetById(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingEntity);
            repositoryMock.Setup(r => r.Update(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedEntity);

            // Act
            var result = await provider.Update(patchDocument, token);

            // Assert
            result.Should().BeEquivalentTo(updatedEntity);
        }

        [Test, MoqAutoData]
        public async Task Update_ShouldReturnNull_WhenEntityDoesNotExist(
            Guid id,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            var patchDocument = new PatchApplication {
                Id = id,
                Patch = new Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<ApplicationReview>()
            };
            repositoryMock.Setup(r => r.GetById(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ApplicationReviewEntity)null!);

            // Act
            var result = await provider.Update(patchDocument, token);

            // Assert
            result.Should().BeNull();
        }
    }
}