using System.Text.Json;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.TemplateHandlers;

public class WhenGettingApplicationSubmittedDelayedEmails
{
    [Test, MoqAutoData]
    public void It_Handles_The_Correct_Templates(Mock<IEmailTemplateHelper> emailTemplateHelper)
    {
        // arrange/act
        new ApplicationSubmittedDelayedEmailHandler(emailTemplateHelper.Object);
    
        // assert
        emailTemplateHelper.Verify(x => x.GetTemplateId(It.IsAny<NotificationTypes>(), It.IsAny<NotificationFrequency>()), Times.Exactly(2));
        emailTemplateHelper.Verify(x => x.GetTemplateId(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Daily), Times.Once);
        emailTemplateHelper.Verify(x => x.GetTemplateId(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Weekly), Times.Once);
    }
    
    [Test]
    [RecruitInlineAutoData(NotificationFrequency.Daily)]
    [RecruitInlineAutoData(NotificationFrequency.Weekly)]
    public void Notifications_Are_Generated_Correctly_For_Multiple_Users(
        NotificationFrequency frequency,
        UserEntity user1,
        UserEntity user2,
        Guid templateId,
        string advertTitle,
        string employerName,
        string location,
        string manageVacancyUrl,
        Dictionary<string, string> staticData,
        VacancyReference vacancyReference,
        Mock<IEmailTemplateHelper> emailTemplateHelper)
    {
        // arrange
        emailTemplateHelper.Setup(x => x.GetTemplateId(NotificationTypes.ApplicationSubmitted, frequency)).Returns(templateId);
        var sut = new ApplicationSubmittedDelayedEmailHandler(emailTemplateHelper.Object);

        var notifications = new List<RecruitNotificationEntity> {
            new()
            {
                Id = 1,
                UserId = user1.Id,
                User = user1,
                SendWhen = DateTime.Now,
                EmailTemplateId = templateId,
                StaticData = JsonSerializer.Serialize(staticData),
                DynamicData = JsonSerializer.Serialize(new Dictionary<string, string> {
                    ["advertTitle"] = advertTitle,
                    ["vacancyReference"] = vacancyReference.ToShortString(),
                    ["employerName"] = employerName,
                    ["location"] = location,
                    ["manageVacancyURL"] = manageVacancyUrl,
                })
            },
            new()
            {
                Id = 1,
                UserId = user2.Id,
                User = user2,
                SendWhen = DateTime.Now,
                EmailTemplateId = templateId,
                StaticData = JsonSerializer.Serialize(staticData),
                DynamicData = JsonSerializer.Serialize(new Dictionary<string, string> {
                    ["advertTitle"] = advertTitle,
                    ["vacancyReference"] = vacancyReference.ToShortString(),
                    ["employerName"] = employerName,
                    ["location"] = location,
                    ["manageVacancyURL"] = manageVacancyUrl,
                })
            }
        };

        string expectedSnippet = $"""
                                 #{advertTitle} ({vacancyReference.ToString()})
                                 {employerName}
                                 {location}
                                 [View applications]({manageVacancyUrl}) (1 new)
                                 
                                 
                                 """;
        
        var expectedTokens = new Dictionary<string, string>(staticData) {
            { "adverts", expectedSnippet } 
        };

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();

        // assert
        results.Should().HaveCount(2);
        
        results.Should().AllSatisfy(x =>
        {
            x.TemplateId.Should().Be(templateId);
            new List<string> { user1.Email, user2.Email }.Should().Contain(x.RecipientAddress);
            x.Tokens.Should().HaveCount(staticData.Count + 1);
            x.Tokens.Should().BeEquivalentTo(expectedTokens);
        });
    }
    
    [Test]
    [RecruitInlineAutoData(NotificationFrequency.Daily)]
    [RecruitInlineAutoData(NotificationFrequency.Weekly)]
    public void Notifications_For_A_Single_Vacancy_Are_Combined_Correctly(
        NotificationFrequency frequency,
        UserEntity user,
        Guid templateId,
        string advertTitle,
        string employerName,
        string location,
        string manageVacancyUrl,
        Dictionary<string, string> staticData,
        VacancyReference vacancyReference,
        List<RecruitNotificationEntity> notifications,
        Mock<IEmailTemplateHelper> emailTemplateHelper)
    {
        // arrange
        emailTemplateHelper.Setup(x => x.GetTemplateId(NotificationTypes.ApplicationSubmitted, frequency)).Returns(templateId);
        var sut = new ApplicationSubmittedDelayedEmailHandler(emailTemplateHelper.Object);
        notifications.ForEach(x =>
        {
            x.UserId = user.Id;
            x.User = user;
            x.EmailTemplateId = templateId;
            x.StaticData = JsonSerializer.Serialize(staticData);
            x.DynamicData = JsonSerializer.Serialize(new Dictionary<string, string> {
                ["advertTitle"] = advertTitle,
                ["vacancyReference"] = vacancyReference.ToShortString(),
                ["employerName"] = employerName,
                ["location"] = location,
                ["manageVacancyURL"] = manageVacancyUrl,
            });
        });

        string expectedSnippet = $"""
                                 #{advertTitle} ({vacancyReference.ToString()})
                                 {employerName}
                                 {location}
                                 [View applications]({manageVacancyUrl}) ({notifications.Count} new)
                                 
                                 
                                 """;
        
        var expectedTokens = new Dictionary<string, string>(staticData) {
            { "adverts", expectedSnippet } 
        };

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();

        // assert
        results.Should().HaveCount(1);
        var email = results[0];
        email.TemplateId.Should().Be(templateId);
        email.RecipientAddress.Should().Be(user.Email);
        email.Tokens.Should().HaveCount(staticData.Count + 1);
        email.Tokens.Should().BeEquivalentTo(expectedTokens);
    }
    
    [Test]
    [RecruitInlineAutoData(NotificationFrequency.Daily)]
    [RecruitInlineAutoData(NotificationFrequency.Weekly)]
    public void Notifications_For_Applications_To_Multiple_Vacancies_Combined_Correctly(
        NotificationFrequency frequency,
        UserEntity user,
        Guid templateId,
        long vacancyRef,
        string advertTitle,
        string employerName,
        string location,
        string manageVacancyUrl,
        Dictionary<string, string> staticData,
        List<RecruitNotificationEntity> notifications,
        Mock<IEmailTemplateHelper> emailTemplateHelper)
    {
        // arrange
        string expectedSnippet = $"""
                                  #{advertTitle} (VAC{vacancyRef})
                                  {employerName}
                                  {location}
                                  [View applications]({manageVacancyUrl}) (1 new)

                                  #{advertTitle} (VAC{vacancyRef+1})
                                  {employerName}
                                  {location}
                                  [View applications]({manageVacancyUrl}) (1 new)
                                  
                                  #{advertTitle} (VAC{vacancyRef+2})
                                  {employerName}
                                  {location}
                                  [View applications]({manageVacancyUrl}) (1 new)
                                  
                                  
                                  """;
        
        var expectedTokens = new Dictionary<string, string>(staticData) {
            { "adverts", expectedSnippet } 
        };
        
        emailTemplateHelper.Setup(x => x.GetTemplateId(NotificationTypes.ApplicationSubmitted, frequency)).Returns(templateId);
        var sut = new ApplicationSubmittedDelayedEmailHandler(emailTemplateHelper.Object);
        notifications.ForEach(x =>
        {
            x.UserId = user.Id;
            x.User = user;
            x.EmailTemplateId = templateId;
            x.StaticData = JsonSerializer.Serialize(staticData);
            x.DynamicData = JsonSerializer.Serialize(new Dictionary<string, string> {
                ["advertTitle"] = advertTitle,
                ["vacancyReference"] = new VacancyReference(vacancyRef++).ToShortString(),
                ["employerName"] = employerName,
                ["location"] = location,
                ["manageVacancyURL"] = manageVacancyUrl,
            });
        });

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();

        // assert
        results.Should().HaveCount(1);
        var email = results[0];
        email.TemplateId.Should().Be(templateId);
        email.RecipientAddress.Should().Be(user.Email);
        email.Tokens.Should().HaveCount(staticData.Count + 1);
        email.Tokens.Should().BeEquivalentTo(expectedTokens);
    }
    
    //[RecruitAutoData]
    public void Notifications (
        UserEntity user,
        Guid templateId,
        string advertTitle,
        string employerName,
        string location,
        string manageVacancyUrl,
        Dictionary<string, string> staticData,
        VacancyReference vacancyReference,
        List<RecruitNotificationEntity> notifications,
        Mock<IEmailTemplateHelper> emailTemplateHelper)
    {
        // arrange
        emailTemplateHelper.Setup(x => x.GetTemplateId(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Daily)).Returns(templateId);
        var sut = new ApplicationSubmittedDelayedEmailHandler(emailTemplateHelper.Object);
        notifications.ForEach(x =>
        {
            x.UserId = user.Id;
            x.User = user;
            x.EmailTemplateId = templateId;
            x.StaticData = JsonSerializer.Serialize(staticData);
            x.DynamicData = JsonSerializer.Serialize(new Dictionary<string, string> {
                ["advertTitle"] = advertTitle,
                ["vacancyReference"] = vacancyReference.ToShortString(),
                ["employerName"] = employerName,
                ["location"] = location,
                ["manageVacancyURL"] = manageVacancyUrl,
            });
        });

        string expectedSnippet = $"""
                                 #{advertTitle} ({vacancyReference.ToString()})
                                 {employerName}
                                 {location}
                                 [View applications]({manageVacancyUrl}) ({notifications.Count} new)
                                 
                                 
                                 """;
        
        var expectedTokens = new Dictionary<string, string>(staticData) {
            { "adverts", expectedSnippet } 
        };

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();

        // assert
        results.Should().HaveCount(1);
        var email = results[0];
        email.TemplateId.Should().Be(templateId);
        email.RecipientAddress.Should().Be(user.Email);
        email.Tokens.Should().HaveCount(staticData.Count + 1);
        email.Tokens.Should().BeEquivalentTo(expectedTokens);
    }
}