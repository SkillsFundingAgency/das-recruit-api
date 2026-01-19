using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

public class WhenGettingVacancyCountByUserId
{
    [Test, MoqAutoData]
    public async Task Then_If_The_User_Is_Not_Found_Zero_Is_Returned(
        Guid userId,
        Mock<IUserRepository> userRepository,
        Mock<IVacancyRepository> vacancyRepository,
        [Greedy] VacancyController sut)
    {
        // arrange
        userRepository
            .Setup(x => x.FindByUserIdAsync(userId.ToString(), CancellationToken.None))
            .ReturnsAsync((UserEntity?)null);

        // act
        var result = (await sut.CountByUserId(userId, userRepository.Object, vacancyRepository.Object, CancellationToken.None)) as Ok<DataResponse<int>>;

        // assert
        result.Should().NotBeNull();
        result.Value!.Data.Should().Be(0);
    }
    
    [Test, RecruitAutoData]
    public async Task Then_If_The_User_Is_Found_The_Count_Is_Returned(
        UserEntity user,
        int count,
        Mock<IUserRepository> userRepository,
        Mock<IVacancyRepository> vacancyRepository,
        [Greedy] VacancyController sut)
    {
        // arrange
        userRepository
            .Setup(x => x.FindByUserIdAsync(user.Id.ToString(), CancellationToken.None))
            .ReturnsAsync(user);
        
        vacancyRepository
            .Setup(x => x.CountVacanciesByUserIdAsync(user.Id, CancellationToken.None))
            .ReturnsAsync(count);

        // act
        var result = (await sut.CountByUserId(user.Id, userRepository.Object, vacancyRepository.Object, CancellationToken.None)) as Ok<DataResponse<int>>;

        // assert
        result.Should().NotBeNull();
        result.Value!.Data.Should().Be(count);
    }
}