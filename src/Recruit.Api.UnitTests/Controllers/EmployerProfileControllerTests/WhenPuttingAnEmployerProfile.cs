﻿using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.EmployerProfile;
using SFA.DAS.Recruit.Api.Models.Responses.EmployerProfile;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.EmployerProfileControllerTests;

internal class WhenPuttingAnEmployerProfile
{
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Profile_Is_Updated(
        long accountLegalEntityId,
        Mock<IEmployerProfileRepository> repository,
        PutEmployerProfileRequest request,
        [Greedy] EmployerProfileController sut,
        CancellationToken token)
    {
        // arrange
        var entity = _fixture.Create<EmployerProfileEntity>();
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<EmployerProfileEntity>(), token))
            .ReturnsAsync(UpsertResult.Create(entity, false));

        // act
        var result = await sut.PutOne(repository.Object, accountLegalEntityId, request, token);
        var payload = (result as Ok<PutEmployerProfileResponse>)?.Value;
        
        // assert
        repository.Verify(x => x.UpsertOneAsync(ItIs.EquivalentTo(request.ToDomain(accountLegalEntityId)), token), Times.Once);
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Profile_Is_Created(
        long accountLegalEntityId,
        Mock<IEmployerProfileRepository> repository,
        PutEmployerProfileRequest request,
        [Greedy] EmployerProfileController sut,
        CancellationToken token)
    {
        // arrange
        var entity = _fixture.Create<EmployerProfileEntity>();
        repository
            .Setup(x => x.UpsertOneAsync(It.IsAny<EmployerProfileEntity>(), token))
            .ReturnsAsync(UpsertResult.Create(entity, true));

        // act
        var result = await sut.PutOne(repository.Object, accountLegalEntityId, request, token);
        var createdResult = result as Created<PutEmployerProfileResponse>;
        
        // assert
        repository.Verify(x => x.UpsertOneAsync(ItIs.EquivalentTo(request.ToDomain(accountLegalEntityId)), token), Times.Once);
        createdResult.Should().NotBeNull();
        createdResult.Value.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers());
        createdResult.Location.Should().Be($"/api/employer/profiles/{entity.AccountLegalEntityId}");
    }
}