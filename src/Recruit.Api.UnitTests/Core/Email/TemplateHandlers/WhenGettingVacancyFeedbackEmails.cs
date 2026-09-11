using System.Text;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Extensions;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.TemplateHandlers;

public class WhenGettingVacancyFeedbackEmails
{
    [Test, RecursiveMoqAutoData]
    public void Then_A_Single_Email_Can_Be_Generated(
        UserEntity user,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancyFeedbackEmailHandler sut)
    {
        // arrange
        var staticData = new Dictionary<string, string>() {
            ["firstName"] = user.Name,
            ["feedbackCount"] = "10",
        };
        
        var dynamicData = new Dictionary<string, string>() {
            ["advertTitle"] = "Advert_Title",
        };

        var expectedTokens = staticData.Concat(dynamicData).ToDictionary();
        expectedTokens["feedbackCount"] = "10 applicants";
        
        var notifications = new List<RecruitNotificationEntity> {
            new()
            {
                UserId = user.Id,
                User = user,
                EmailTemplateId = emailTemplateHelper.Object.TemplateIds.VacancyFeedbackRequired,
                SendWhen = DateTime.UtcNow,
                StaticData = ApiUtils.SerializeOrNull(staticData)!,
                DynamicData = ApiUtils.SerializeOrNull(dynamicData)!,
            }
        };

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();

        // assert
        results.Should().HaveCount(1);
        results[0].SourceIds.Should().BeNull();
        results[0].TemplateId.Should().Be(emailTemplateHelper.Object.TemplateIds.VacancyFeedbackRequired);
        results[0].RecipientAddress.Should().Be(user.Email);
        results[0].Tokens.Should().BeEquivalentTo(expectedTokens);
    }
    
    [Test]
    [RecursiveMoqInlineAutoData("10", "10 applicants")]
    [RecursiveMoqInlineAutoData("1", "1 applicant")]
    public void Then_The_Feedback_Count_Is_Pluralised_Correctly(
        string feedbackCount,
        string expectedFeedbackCount,
        UserEntity user,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancyFeedbackEmailHandler sut)
    {
        // arrange
        var staticData = new Dictionary<string, string>() {
            ["firstName"] = user.Name,
            ["feedbackCount"] = feedbackCount,
        };
        
        var dynamicData = new Dictionary<string, string>() {
            ["advertTitle"] = "Advert_Title",
        };

        var notifications = new List<RecruitNotificationEntity> {
            new()
            {
                UserId = user.Id,
                User = user,
                EmailTemplateId = emailTemplateHelper.Object.TemplateIds.VacancyFeedbackRequired,
                SendWhen = DateTime.UtcNow,
                StaticData = ApiUtils.SerializeOrNull(staticData)!,
                DynamicData = ApiUtils.SerializeOrNull(dynamicData)!,
            }
        };

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();

        // assert
        results[0].Tokens["feedbackCount"].Should().BeEquivalentTo(expectedFeedbackCount);
    }
    
    [Test, RecursiveMoqAutoData]
    public void Then_Multiple_Single_Emails_Can_Be_Generated(
        List<UserEntity> users,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancyFeedbackEmailHandler sut)
    {
        // arrange
        List<RecruitNotificationEntity> notifications = [];
        foreach (var user in users)
        {
            var staticData = new Dictionary<string, string>() {
                ["firstName"] = user.Name,
                ["feedbackCount"] = "10",
            };
            
            var dynamicData = new Dictionary<string, string>() {
                ["advertTitle"] = "Advert_Title",
            };

            notifications.Add(new RecruitNotificationEntity() {
                UserId = user.Id,
                User = user,
                EmailTemplateId = emailTemplateHelper.Object.TemplateIds.VacancyFeedbackRequired,
                SendWhen = DateTime.UtcNow,
                StaticData = ApiUtils.SerializeOrNull(staticData)!,
                DynamicData = ApiUtils.SerializeOrNull(dynamicData)!,
            });
        }

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();

        // assert
        results.Should().HaveCount(3);
    }
    
    [Test, RecursiveMoqAutoData]
    public void Then_Advert_Is_Pluralised_Correctly(
        Guid emailTemplateId,
        UserEntity user,
        [Greedy] VacancyFeedbackEmailHandler sut)
    {
        // arrange
        List<RecruitNotificationEntity> notifications = [];
        var staticData = new Dictionary<string, string>() {
            ["firstName"] = user.Name,
            ["feedbackCount"] = "10",
            ["advertNoun"] = "advert",
        };
        
        var dynamicData = new Dictionary<string, string>() {
            ["advertTitle"] = "Advert_Title",
        };

        notifications.Add(new RecruitNotificationEntity() {
            UserId = user.Id,
            User = user,
            EmailTemplateId = emailTemplateId,
            SendWhen = DateTime.UtcNow,
            StaticData = ApiUtils.SerializeOrNull(staticData)!,
            DynamicData = ApiUtils.SerializeOrNull(dynamicData)!,
        });
        
        notifications.Add(new RecruitNotificationEntity() {
            UserId = user.Id,
            User = user,
            EmailTemplateId = emailTemplateId,
            SendWhen = DateTime.UtcNow,
            StaticData = ApiUtils.SerializeOrNull(staticData)!,
            DynamicData = ApiUtils.SerializeOrNull(dynamicData)!,
        });

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();

        // assert
        results[0].Tokens["advertNoun"].Should().Be("adverts");
    }
    
    [Test, RecursiveMoqAutoData]
    public void Then_Vacancy_Is_Pluralised_Correctly(
        Guid emailTemplateId,
        UserEntity user,
        [Greedy] VacancyFeedbackEmailHandler sut)
    {
        // arrange
        List<RecruitNotificationEntity> notifications = [];
        var staticData = new Dictionary<string, string>() {
            ["firstName"] = user.Name,
            ["feedbackCount"] = "10",
            ["advertNoun"] = "vacancy",
        };
        
        var dynamicData = new Dictionary<string, string>() {
            ["advertTitle"] = "Advert_Title",
        };

        notifications.Add(new RecruitNotificationEntity() {
            UserId = user.Id,
            User = user,
            EmailTemplateId = emailTemplateId,
            SendWhen = DateTime.UtcNow,
            StaticData = ApiUtils.SerializeOrNull(staticData)!,
            DynamicData = ApiUtils.SerializeOrNull(dynamicData)!,
        });
        
        notifications.Add(new RecruitNotificationEntity() {
            UserId = user.Id,
            User = user,
            EmailTemplateId = emailTemplateId,
            SendWhen = DateTime.UtcNow,
            StaticData = ApiUtils.SerializeOrNull(staticData)!,
            DynamicData = ApiUtils.SerializeOrNull(dynamicData)!,
        });

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();

        // assert
        results[0].Tokens["advertNoun"].Should().Be("vacancies");
    }
    
    [Test, RecursiveMoqAutoData]
    public void Then_Single_Batched_Email_Can_Be_Generated(
        UserEntity user,
        [Frozen] Mock<IEmailTemplateHelper> emailTemplateHelper,
        [Greedy] VacancyFeedbackEmailHandler sut)
    {
        // arrange
        List<RecruitNotificationEntity> notifications = [];
        var staticData = new Dictionary<string, string> {
            ["firstName"] = user.Name,
            ["feedbackCount"] = "10",
            ["advertNoun"] = "advert",
        };

        var expectedTokens = new Dictionary<string, string> {
            ["firstName"] = user.Name,
            ["advertTitles"] = new StringBuilder().AppendLine("Advert_Title_1").AppendLine("Advert_Title_2").ToString(),
            ["advertNoun"] = "adverts",
        };

        notifications.Add(new RecruitNotificationEntity {
            UserId = user.Id,
            User = user,
            EmailTemplateId = emailTemplateHelper.Object.TemplateIds.VacancyFeedbackRequired,
            SendWhen = DateTime.UtcNow,
            StaticData = ApiUtils.SerializeOrNull(staticData)!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string> { ["advertTitle"] = "Advert_Title_1", })!,
        });

        notifications.Add(new RecruitNotificationEntity {
            UserId = user.Id,
            User = user,
            EmailTemplateId = emailTemplateHelper.Object.TemplateIds.VacancyFeedbackRequired,
            SendWhen = DateTime.UtcNow,
            StaticData = ApiUtils.SerializeOrNull(staticData)!,
            DynamicData = ApiUtils.SerializeOrNull(new Dictionary<string, string> { ["advertTitle"] = "Advert_Title_2", })!,
        });

        // act
        var results = sut.CreateNotificationEmails(notifications).ToList();

        // assert
        results.Should().HaveCount(1);
        results[0].SourceIds.Should().BeNull();
        results[0].TemplateId.Should().Be(emailTemplateHelper.Object.TemplateIds.VacancyFeedbackRequiredBatched);
        results[0].RecipientAddress.Should().Be(user.Email);
        results[0].Tokens.Should().BeEquivalentTo(expectedTokens);
    }
}