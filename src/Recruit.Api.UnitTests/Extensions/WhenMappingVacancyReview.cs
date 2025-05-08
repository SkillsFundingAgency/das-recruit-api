using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.VacancyReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Extensions;

public class WhenMappingVacancyReview
{
    [Test]
    public void ToDomain_Maps_A_Put_Request()
    {
        // arrange
        var id = Guid.NewGuid();
        var request = new PutVacancyReviewRequest {
            VacancyReference = 1234,
            VacancyTitle = "vacancyTitle",
            CreatedDate = DateTime.Now.AddDays(-1),
            SlaDeadLine = DateTime.Now,
            Status = ReviewStatus.New,
            SubmittedByUserEmail = "foo@af23fwdfwef_wefw3f_F23f23!.com",
            ManualQaFieldIndicators = ["one", "two"],
            DismissedAutomatedQaOutcomeIndicators = ["three", "four"],
            UpdatedFieldIdentifiers = [],
            VacancySnapshot = "vacancySnapshot",
            SubmissionCount = 2,
            AutomatedQaOutcome = "automatedQaOutcome",
            AutomatedQaOutcomeIndicators = "automatedQaOutcomeIndicators",
            ClosedDate = DateTime.Now.AddDays(1),
            ManualOutcome = "manualOutcome",
            ManualQaComment = "manualQaComment",
            ReviewedByUserEmail = "foo@af23fwdfwef_wefw3f_F23f23!.com",
            ReviewedDate = DateTime.Now,
        };
        
        // act
        var result = request.ToDomain(id);

        // assert
        result.Id.Should().Be(id);
        result.Should().BeEquivalentTo(request, options => options
            .Excluding(x => x.ManualQaFieldIndicators)
            .Excluding(x => x.UpdatedFieldIdentifiers)
            .Excluding(x => x.DismissedAutomatedQaOutcomeIndicators)
        );

        result.ManualQaFieldIndicators.Should().Be("[\"one\",\"two\"]");
        result.DismissedAutomatedQaOutcomeIndicators.Should().Be("[\"three\",\"four\"]");
        result.UpdatedFieldIdentifiers.Should().Be("[]");
    }
}