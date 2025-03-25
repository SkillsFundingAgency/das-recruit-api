using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Recruit.Api.Data;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Database.Tests.DatabaseMock;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Database.Tests.ApplicationReview;

public class WhenUpsertingApplicationReview
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Application_Is_Inserted_If_Not_Exists(
        ApplicationReviewEntity applicationEntity,
        [Frozen]Mock<IRecruitDataContext> context,
        ApplicationReviewRepository repository)
    {
        //Arrange
        context.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(new List<ApplicationReviewEntity>());
            
        //Act
        var actual = await repository.Upsert(applicationEntity);

        //Assert
        context.Verify(x => x.ApplicationReviewEntities.AddAsync(applicationEntity, CancellationToken.None), Times.Once);
        context.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        actual.Item2.Should().BeTrue();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Application_Is_Updated_If_Exists(
        ApplicationReviewEntity applicationEntity,
        [Frozen] Mock<IRecruitDataContext> context,
        ApplicationReviewRepository repository)
    {
        //Arrange
        context.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(new List<ApplicationReviewEntity> { applicationEntity });
            
        //Act
        var actual = await repository.Upsert(applicationEntity);

        //Assert
        context.Verify(x => x.ApplicationReviewEntities.AddAsync(It.IsAny<ApplicationReviewEntity>(), CancellationToken.None), Times.Never);
        context.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        actual.Item2.Should().BeFalse();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Statuses_Are_Updated_If_They_Are_Not_NotStarted(
        ApplicationReviewEntity updateEntity,
        ApplicationReviewEntity applicationEntity,
        [Frozen]Mock<IRecruitDataContext> context,
        ApplicationReviewRepository repository)
    {
        //Arrange
        updateEntity.Status = "2";
        applicationEntity.Status = "0";
        
        context.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(new List<ApplicationReviewEntity>{applicationEntity});
        
        //Act
        var actual = await repository.Upsert(updateEntity);
        
        //Assert
        actual.Item1.Status.Should().Be("0");
        actual.Item2.Should().BeFalse();
    }
}