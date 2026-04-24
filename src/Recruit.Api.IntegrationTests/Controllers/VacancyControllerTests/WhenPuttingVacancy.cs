using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apim.Shared.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.UnitTests;
using SFA.DAS.Recruit.Contracts.ApiRequests;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

public class WhenPuttingVacancy: BaseFixture
{
    [Test]
    public async Task Then_Without_Required_Fields_Bad_Request_Is_Returned()
    {
        // act
        var response = await Client.PutAsJsonAsync(new PutVacanciesByVacancyIdApiRequest { VacancyId = Guid.NewGuid() }.PutUrl, new {});
        var errors = await response.Content.ReadAsAsync<ValidationProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Should().NotBeNull();
        errors.Errors.Should().HaveCount(2);
        errors.Errors.Should().ContainKeys(
            nameof(PutVacancyRequest.OwnerType),
            nameof(PutVacancyRequest.Status)
        );
    }
    
    
    [Test]
    public async Task Then_With_Validation_And_Incorrect_Content_Bad_Request_Is_Returned()
    {
        // act
        var request = new PutVacanciesByVacancyIdApiRequest {
            VacancyId = Guid.NewGuid(),
            ValidateOnly = true,
            RuleSet = Contracts.ApiResponses.VacancyRuleSet.ShortDescription | Contracts.ApiResponses.VacancyRuleSet.Title,
            Data = new Contracts.ApiResponses.PutVacancyRequest {
                OwnerType = Contracts.ApiResponses.OwnerType.Employer,
                Status = Contracts.ApiResponses.VacancyStatus.Draft,
                Title = "Short title",
                ShortDescription = "Short description"
            }
        };
        var response = await Client.PutAsJsonAsync(request.PutUrl, request.Data);
        var errors = await response.Content.ReadAsAsync<ValidationProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Should().NotBeNull();
        errors.Errors.Should().HaveCount(2);
        errors.Errors.Should().ContainKeys(
            nameof(VacancyRequest.Title),
            nameof(VacancyRequest.ShortDescription)
        );
    }
    
    [Test]
    public async Task Then_With_Validation_And_Correct_Content_Created_Is_Returned_And_Value_Not_Added_To_Database()
    {
        //arrange
        Server.DataContext
            .Setup(x => x.ProhibitedContentEntities)
            .ReturnsDbSet(
            [
                new ProhibitedContentEntity
                    { Content = "Dangit", ContentType = Domain.Models.ProhibitedContentType.BannedPhrases },
                new ProhibitedContentEntity
                    { Content = "Dangit", ContentType = Domain.Models.ProhibitedContentType.Profanity }
            ]);
        
        // act
        var response = await Client.PutAsJsonAsync(new PutVacanciesByVacancyIdApiRequest { VacancyId = Guid.NewGuid(), ValidateOnly = true, RuleSet = Contracts.ApiResponses.VacancyRuleSet.Title }.PutUrl, new PutVacancyRequest {
            OwnerType = OwnerType.Employer,
            Status = VacancyStatus.Draft,
            Title = "Apprenticeship Short title"
        });
        
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location.ToString().Should().Be($"/api/vacancies/{vacancy.Id}");
        vacancy.VacancyReference.Should().Be(1000000001);
    }
    
    [Test]
    public async Task Then_The_Vacancy_Is_Added()
    {
        // arrange
        var id = Guid.NewGuid();
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(Fixture.CreateMany<UserEntity>(10));
        
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyEntity>(10).ToList());

        var request = Fixture.Create<PutVacancyRequest>();
        var expectedEntity = request.ToDomain(id);
        
        // act
        var response = await Client.PutAsJsonAsync(new PutVacanciesByVacancyIdApiRequest { VacancyId = id }.PutUrl, request);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        vacancy.Should().BeEquivalentTo(request, opt => opt
            .Excluding(x => x.Id)
            .Excluding(x => x.SubmittedByUserId)
            .Excluding(x => x.ReviewRequestedByUserId)
            .Excluding(x=>x.Wage!.ApprenticeMinimumWage)
            .Excluding(x=>x.Wage!.Under18NationalMinimumWage)
            .Excluding(x=>x.Wage!.Between18AndUnder21NationalMinimumWage)
            .Excluding(x=>x.Wage!.Between21AndUnder25NationalMinimumWage)
            .Excluding(x=>x.Wage!.Over25NationalMinimumWage)
            .Excluding(x=>x.Wage!.WageText)
        );

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location.ToString().Should().Be($"/api/vacancies/{vacancy.Id}");

        Server.DataContext.Verify(x => x.VacancyEntities.AddAsync(ItIs.EquivalentTo(expectedEntity), It.IsAny<CancellationToken>()), Times.Once());
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task Then_The_Vacancy_Is_Replaced()
    {
        // arrange
        var items = Fixture.CreateMany<VacancyEntity>(10).ToList();
        var targetItem = items[5];
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(items);
        
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(Fixture.CreateMany<UserEntity>(10));

        var request = Fixture.Create<PutVacancyRequest>();
        
        // act
        var response = await Client.PutAsJsonAsync(new PutVacanciesByVacancyIdApiRequest { VacancyId = targetItem.Id }.PutUrl, request);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        vacancy.Should().BeEquivalentTo(request, opt => opt
            .Excluding(x => x.Id)
            .Excluding(x => x.SubmittedByUserId)
            .Excluding(x => x.ReviewRequestedByUserId)
            .Excluding(x=>x.Wage!.ApprenticeMinimumWage)
            .Excluding(x=>x.Wage!.Under18NationalMinimumWage)
            .Excluding(x=>x.Wage!.Between18AndUnder21NationalMinimumWage)
            .Excluding(x=>x.Wage!.Between21AndUnder25NationalMinimumWage)
            .Excluding(x=>x.Wage!.Over25NationalMinimumWage)
            .Excluding(x=>x.Wage!.WageText)
        );

        Server.DataContext.Verify(x => x.SetValues(targetItem, ItIs.EquivalentTo(request.ToDomain(targetItem.Id))), Times.Once());
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Then_A_New_Vacancy_Without_VacancyReference_Is_Assigned_One()
    {
        // arrange
        var vacancyReference = Fixture.Create<VacancyReference>();

        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(Fixture.CreateMany<UserEntity>(10));

        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyEntity>(10).ToList());

        Server.DataContext
            .Setup(x => x.GetNextVacancyReferenceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancyReference.Value);

        var request = Fixture.Build<PutVacancyRequest>()
            .With(x => x.VacancyReference, (long?)null)
            .Create();

        // act
        var response = await Client.PutAsJsonAsync(new PutVacanciesByVacancyIdApiRequest { VacancyId = Guid.NewGuid() }.PutUrl, request);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        vacancy.VacancyReference.Should().Be(vacancyReference.Value);
        Server.DataContext.Verify(x => x.GetNextVacancyReferenceAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Then_An_Existing_Vacancy_Without_VacancyReference_Is_Assigned_One_On_Update()
    {
        // arrange
        var vacancyReference = Fixture.Create<VacancyReference>();
        var items = Fixture.CreateMany<VacancyEntity>(10).ToList();
        var targetItem = items[5];
        targetItem.VacancyReference = null;

        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(items);

        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(Fixture.CreateMany<UserEntity>(10));

        Server.DataContext
            .Setup(x => x.GetNextVacancyReferenceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancyReference.Value);

        var request = Fixture.Build<PutVacancyRequest>()
            .With(x => x.VacancyReference, (long?)null)
            .Create();

        // act
        var response = await Client.PutAsJsonAsync(new PutVacanciesByVacancyIdApiRequest { VacancyId = targetItem.Id }.PutUrl, request);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        vacancy.VacancyReference.Should().Be(vacancyReference.Value);
        Server.DataContext.Verify(x => x.GetNextVacancyReferenceAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}