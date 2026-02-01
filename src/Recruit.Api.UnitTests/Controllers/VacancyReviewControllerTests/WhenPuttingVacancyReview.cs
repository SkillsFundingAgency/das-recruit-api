using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.VacancyReview;

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

        var result = await sut.PutOne(repository.Object, userRepository.Object, id, request, token);

        userRepository.Verify(x => x.FindByUserIdAsync(submittedByUserId.ToString(), token), Times.Once);
        repository.Verify(x => x.UpsertOneAsync(It.Is<VacancyReviewEntity>(e => e.SubmittedByUserEmail == userEmail), token), Times.Once);
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

        var result = await sut.PutOne(repository.Object, userRepository.Object, id, request, token);

        userRepository.Verify(x => x.FindByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        result.Should().BeOfType<Ok<SFA.DAS.Recruit.Api.Models.VacancyReview>>();
    }
}
