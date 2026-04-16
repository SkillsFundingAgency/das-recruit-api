using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.VacancyReview;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyReviewControllerTests;

[TestFixture]
internal class WhenPuttingVacancyReview
{
    [Test, RecruitAutoData]
    public async Task Then_Email_Is_Looked_Up_From_SubmittedUserId_And_Created_Returned(
        Guid id,
        Guid submittedByUserId,
        string userEmail,
        UserEntity user,
        PutVacancyReviewRequest request,
        VacancyReviewEntity entity,
        Mock<IUserRepository> userRepository,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        user.Email = userEmail;
        entity.SubmittedByUserEmail = userEmail;
        userRepository
            .Setup(x => x.FindByUserIdAsync(submittedByUserId.ToString(), token))
            .ReturnsAsync(user);

        request.SubmittedByUserEmail = null;
        request.SubmittedByUserId = submittedByUserId.ToString();

        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyReviewEntity>(), token))
            .ReturnsAsync(() => SFA.DAS.Recruit.Api.Data.Models.UpsertResult.Create(entity, true));

        var result = await sut.PutOne(repository.Object, userRepository.Object, Mock.Of<IAutomatedReviewService>(), Mock.Of<IEventsService>(), id, request, token);

        userRepository.Verify(x => x.FindByUserIdAsync(submittedByUserId.ToString(), token), Times.Once);
        repository.Verify(x => x.UpsertOneAsync(It.Is<VacancyReviewEntity>(e => e.SubmittedByUserEmail == userEmail), token), Times.Exactly(2));
        result.Should().BeOfType<Created<SFA.DAS.Recruit.Api.Models.VacancyReview>>();
    }

    [Test, RecruitAutoData]
    public async Task Then_Email_Is_Set_To_Unknown_When_User_Not_Found_And_Created_Returned(
        Guid id,
        string submittedByUserId,
        PutVacancyReviewRequest request,
        VacancyReviewEntity entity,
        Mock<IUserRepository> userRepository,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        // Arrange
        request.SubmittedByUserEmail = null;
        request.SubmittedByUserId = submittedByUserId;

        userRepository
            .Setup(x => x.FindByUserIdAsync(submittedByUserId, token))
            .ReturnsAsync((UserEntity?)null);

        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyReviewEntity>(), token))
            .ReturnsAsync(() => SFA.DAS.Recruit.Api.Data.Models.UpsertResult.Create(entity, true));

        // Act
        var result = await sut.PutOne(repository.Object, userRepository.Object, Mock.Of<IAutomatedReviewService>(), Mock.Of<IEventsService>(), id, request, token);

        // Assert
        userRepository.Verify(x => x.FindByUserIdAsync(submittedByUserId, token), Times.Once);
        repository.Verify(
            x => x.UpsertOneAsync(
                It.Is<VacancyReviewEntity>(e => e.SubmittedByUserEmail == $"unknown-{submittedByUserId}"),
                token),
            Times.Once);
        result.Should().BeOfType<Created<SFA.DAS.Recruit.Api.Models.VacancyReview>>();
    }
    
    
    [Test, RecruitAutoData]
    public async Task Then_Email_Is_Set_To_Unknown_When_No_Id_Or_Email_And_Created_Returned(
        Guid id,
        string submittedByUserId,
        PutVacancyReviewRequest request,
        VacancyReviewEntity entity,
        Mock<IUserRepository> userRepository,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        // Arrange
        request.SubmittedByUserEmail = null;
        request.SubmittedByUserId = null;

        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyReviewEntity>(), token))
            .ReturnsAsync(() => SFA.DAS.Recruit.Api.Data.Models.UpsertResult.Create(entity, true));

        // Act
        var result = await sut.PutOne(repository.Object, userRepository.Object, Mock.Of<IAutomatedReviewService>(), Mock.Of<IEventsService>(), id, request, token);

        // Assert
        repository.Verify(
            x => x.UpsertOneAsync(
                It.Is<VacancyReviewEntity>(e => e.SubmittedByUserEmail == $"unknown-{request.VacancyReference}"),
                token),
            Times.Once);
        result.Should().BeOfType<Created<SFA.DAS.Recruit.Api.Models.VacancyReview>>();
    }

    [Test, RecruitAutoData]
    public async Task Then_UserRepository_Not_Called_When_Email_Provided_And_Ok_Returned(
        Guid id,
        PutVacancyReviewRequest request,
        Mock<IUserRepository> userRepository,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyReviewEntity>(), token))
            .ReturnsAsync(() => SFA.DAS.Recruit.Api.Data.Models.UpsertResult.Create(request.ToDomain(id), false));

        var result = await sut.PutOne(repository.Object, userRepository.Object, Mock.Of<IAutomatedReviewService>(), Mock.Of<IEventsService>(), id, request, token);

        userRepository.Verify(x => x.FindByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        result.Should().BeOfType<Ok<SFA.DAS.Recruit.Api.Models.VacancyReview>>();
    }
    
    [Test, RecruitAutoData]
    public async Task Then_The_Vacancy_Review_Created_Event_Is_Raised_When_A_New_Record_Is_Created(
        Guid id,
        PutVacancyReviewRequest request,
        Mock<IUserRepository> userRepository,
        Mock<IVacancyReviewRepository> repository,
        Mock<IEventsService> eventsService,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        // arrange
        var entity = request.ToDomain(id);
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyReviewEntity>(), token))
            .ReturnsAsync(() => SFA.DAS.Recruit.Api.Data.Models.UpsertResult.Create(entity, true));

        // act
        var result = await sut.PutOne(repository.Object, userRepository.Object, Mock.Of<IAutomatedReviewService>(), eventsService.Object, id, request, token);

        // assert
        result.Should().BeOfType<Created<SFA.DAS.Recruit.Api.Models.VacancyReview>>();
        eventsService.Verify(x => x.PublishVacancyReviewCreatedEventAsync(entity), Times.Once);
    }
    
    [Test, RecruitAutoData]
    public async Task Then_The_Vacancy_Review_Created_Event_Is_Not_Raised_When_A_Record_Is_Updated(
        Guid id,
        PutVacancyReviewRequest request,
        Mock<IUserRepository> userRepository,
        Mock<IVacancyReviewRepository> repository,
        Mock<IEventsService> eventsService,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        // arrange
        var entity = request.ToDomain(id);
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyReviewEntity>(), token))
            .ReturnsAsync(() => SFA.DAS.Recruit.Api.Data.Models.UpsertResult.Create(entity, false));

        // act
        var result = await sut.PutOne(repository.Object, userRepository.Object, Mock.Of<IAutomatedReviewService>(), eventsService.Object, id, request, token);

        // assert
        result.Should().BeOfType<Ok<SFA.DAS.Recruit.Api.Models.VacancyReview>>();
        eventsService.Verify(x => x.PublishVacancyReviewCreatedEventAsync(entity), Times.Never);
    }
    
    [Test, RecruitAutoData]
    public async Task Then_A_Created_Vacancy_Review_Is_Assessed_By_The_Auto_Checks(
        Guid id,
        string userEmail,
        Guid submittedByUserId,
        UserEntity user,
        PutVacancyReviewRequest request,
        Mock<IUserRepository> userRepository,
        Mock<IVacancyReviewRepository> repository,
        Mock<IEventsService> eventsService,
        Mock<IAutomatedReviewService> automatedReviewService,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        // arrange
        user.Email = userEmail;
        userRepository
            .Setup(x => x.FindByUserIdAsync(submittedByUserId.ToString(), token))
            .ReturnsAsync(user);
        
        request.SubmittedByUserEmail = null;
        request.SubmittedByUserId = submittedByUserId.ToString();
        
        var entity = request.ToDomain(id);
        entity.SubmittedByUserEmail = userEmail;
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<VacancyReviewEntity>(), token))
            .ReturnsAsync(() => SFA.DAS.Recruit.Api.Data.Models.UpsertResult.Create(entity, true));
        
        // act
        var result = await sut.PutOne(repository.Object, userRepository.Object, automatedReviewService.Object, eventsService.Object, id, request, token);

        // assert
        result.Should().BeOfType<Created<SFA.DAS.Recruit.Api.Models.VacancyReview>>();
        automatedReviewService.Verify(x => x.ProcessVacancyReviewAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.UpsertOneAsync(It.Is<VacancyReviewEntity>(e => e.SubmittedByUserEmail == userEmail), token), Times.Exactly(2));
    }
}
