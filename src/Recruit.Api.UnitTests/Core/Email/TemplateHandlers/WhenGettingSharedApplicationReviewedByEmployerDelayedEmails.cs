using System.Text.Json;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.TemplateHandlers;

public class WhenGettingSharedApplicationReviewedByEmployerDelayedEmails
{
    [Test, MoqAutoData]
    public void It_Handles_The_Correct_Templates(Mock<IEmailTemplateHelper> emailTemplateHelper, IEmailTemplateIds emailTemplateIds)
    {
        // arrange
        emailTemplateHelper.Setup(x => x.TemplateIds).Returns(emailTemplateIds);
        
        // act
        var sut = new SharedApplicationReviewedByEmployerDelayedEmailHandler(emailTemplateHelper.Object);
    
        // assert
        sut.CanHandle(emailTemplateIds.SharedApplicationReviewedByEmployer).Should().BeTrue();
    }
    
    [Test]
    [RecruitAutoData]
    public void Notifications_Are_Generated_Correctly_For_Multiple_Users(
        UserEntity user1,
        UserEntity user2,
        Guid templateId,
        string advertTitle,
        string employerName,
        string location,
        string manageVacancyUrl,
        Dictionary<string, string> staticData,
        VacancyReference vacancyReference,
        SharedApplicationReviewedByEmployerDelayedEmailHandler sut)
    {
        // arrange
        staticData.Add("vacancyReference",  vacancyReference.ToShortString());
        string staticDataString = JsonSerializer.Serialize(staticData);
        string dynamicDataString = JsonSerializer.Serialize(new Dictionary<string, string>());
        
        var notifications = new List<RecruitNotificationEntity> {
            new()
            {
                Id = 1,
                UserId = user1.Id,
                User = user1,
                SendWhen = DateTime.Now,
                EmailTemplateId = templateId,
                StaticData = staticDataString,
                DynamicData = dynamicDataString
            },
            new()
            {
                Id = 2,
                UserId = user2.Id,
                User = user2,
                SendWhen = DateTime.Now,
                EmailTemplateId = templateId,
                StaticData = staticDataString,
                DynamicData = dynamicDataString
            }
        };

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();

        // assert
        results.Should().HaveCount(2);
        results.Where(x => x.SourceIds!.Contains(1)).Should().AllSatisfy(x =>
        {
            x.TemplateId.Should().Be(templateId);
            x.RecipientAddress.Should().Be(user1.Email);
            x.Tokens.Should().BeEquivalentTo(staticData);
        });
        results.Where(x => x.SourceIds!.Contains(2)).Should().AllSatisfy(x =>
        {
            x.TemplateId.Should().Be(templateId);
            x.RecipientAddress.Should().Be(user2.Email);
            x.Tokens.Should().BeEquivalentTo(staticData);
        });
    }
    
    [Test]
    [RecruitAutoData]
    public void Notifications_For_A_Single_Vacancy_Are_Combined_Correctly(
        UserEntity user,
        Guid templateId,
        string advertTitle,
        string employerName,
        string location,
        string manageVacancyUrl,
        Dictionary<string, string> staticData,
        VacancyReference vacancyReference,
        SharedApplicationReviewedByEmployerDelayedEmailHandler sut)
    {
        // arrange
        staticData.Add("vacancyReference", vacancyReference.ToShortString());
        string staticDataString = JsonSerializer.Serialize(staticData);
        string dynamicDataString = JsonSerializer.Serialize(new Dictionary<string, string>());
        
        var notifications = new List<RecruitNotificationEntity> {
            new()
            {
                Id = 1,
                UserId = user.Id,
                User = user,
                SendWhen = DateTime.Now,
                EmailTemplateId = templateId,
                StaticData = staticDataString,
                DynamicData = dynamicDataString
            },
            new()
            {
                Id = 2,
                UserId = user.Id,
                User = user,
                SendWhen = DateTime.Now,
                EmailTemplateId = templateId,
                StaticData = staticDataString,
                DynamicData = dynamicDataString
            }
        };

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();
        var email = results.Single();

        // assert
        email.SourceIds.Should().Contain([1, 2]);
        email.TemplateId.Should().Be(templateId);
        email.Tokens.Should().BeEquivalentTo(staticData);
        email.RecipientAddress.Should().Be(user.Email);
    }
}