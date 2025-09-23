using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingEmailsFromNotifications
{
    private List<Mock<IEmailTemplateHandler>> _templateHandlers;
    private EmailFactory _sut;
    
    [SetUp]
    public void Setup()
    {
        _templateHandlers = [
            new Mock<IEmailTemplateHandler>(),
            new Mock<IEmailTemplateHandler>(),
        ];

        _sut = new EmailFactory(_templateHandlers.Select(x => x.Object));
    }
    
    [Test, RecursiveMoqAutoData]
    public void No_Handlers_For_A_Template_Returns_No_Results(Guid templateId, RecruitNotificationEntity notification)
    {
        // arrange
        _templateHandlers[0].Setup(x => x.CanHandle(templateId)).Returns(false);
        _templateHandlers[1].Setup(x => x.CanHandle(templateId)).Returns(false);
        notification.EmailTemplateId = templateId;
        
        // act
        var results = _sut.CreateFrom([notification]);

        // assert
        results.Should().BeEmpty();
    }
    
    [Test, RecursiveMoqAutoData]
    public void Single_Handler_For_A_Template_Returns_Results(Guid templateId, RecruitNotificationEntity notification, List<NotificationEmail> emails)
    {
        // arrange
        _templateHandlers[0].Setup(x => x.CanHandle(templateId)).Returns(false);
        _templateHandlers[1].Setup(x => x.CanHandle(templateId)).Returns(true);
        _templateHandlers[1].Setup(x => x.CreateNotificationEmails(It.IsAny<IEnumerable<RecruitNotificationEntity>>())).Returns(emails);
        
        notification.EmailTemplateId = templateId;
        
        // act
        var results = _sut.CreateFrom([notification]);

        // assert
        results.Should().BeEquivalentTo(emails);
    }
    
    [Test, RecursiveMoqAutoData]
    public void Multiple_Handlers_For_A_Template_Returns_Combined_Results(
        Guid templateId,
        RecruitNotificationEntity notification,
        List<NotificationEmail> emails1,
        List<NotificationEmail> emails2
        )
    {
        // arrange
        _templateHandlers[0].Setup(x => x.CanHandle(templateId)).Returns(true);
        _templateHandlers[0].Setup(x => x.CreateNotificationEmails(It.IsAny<IEnumerable<RecruitNotificationEntity>>())).Returns(emails1);
        _templateHandlers[1].Setup(x => x.CanHandle(templateId)).Returns(true);
        _templateHandlers[1].Setup(x => x.CreateNotificationEmails(It.IsAny<IEnumerable<RecruitNotificationEntity>>())).Returns(emails2);
        
        notification.EmailTemplateId = templateId;
        
        // act
        var results = _sut.CreateFrom([notification]);

        // assert
        results.Should().BeEquivalentTo([..emails1, ..emails2]);
    }
}