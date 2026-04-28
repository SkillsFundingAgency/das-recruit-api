using System.Net;
using System.Text.Json;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses.Report;
using SFA.DAS.Recruit.Contracts.ApiRequests;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.ReportControllerTests;

internal class WhenGeneratingQaReport : BaseFixture
{
    [Test]
    public async Task Then_The_Qa_Report_Is_Returned()
    {
        // arrange
        var vacancyReference = 1000000001L;
        var fromDate = DateTime.UtcNow.AddDays(-30);
        var toDate = DateTime.UtcNow;

        var reportEntity = Fixture.Build<ReportEntity>()
            .With(x => x.OwnerType, ReportOwnerType.Qa)
            .With(x => x.CreatedDate, DateTime.UtcNow)
            .With(x => x.DynamicCriteria, JsonSerializer.Serialize(new ReportCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            }))
            .Create();

        var review = Fixture.Build<VacancyReviewEntity>()
            .With(x => x.VacancyReference, vacancyReference)
            .With(x => x.CreatedDate, DateTime.UtcNow.AddDays(-5))
            .Create();

        var vacancy = Fixture.Build<VacancyEntity>()
            .With(x => x.VacancyReference, vacancyReference)
            .Create();

        Server.DataContext.Setup(x => x.ReportEntities).ReturnsDbSet([reportEntity]);
        Server.DataContext.Setup(x => x.VacancyReviewEntities).ReturnsDbSet([review]);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);

        // act
        var response = await Client.GetAsync(new GetReportsGenerateQaByReportIdApiRequest(reportEntity.Id).GetUrl);
        var result = await response.Content.ReadAsAsync<GetQaReportResponse>();

        // assert
        response.EnsureSuccessStatusCode();
        result.Should().NotBeNull();
        result.QaReports.Should().HaveCount(1);

        var report = result.QaReports[0];
        report.VacancyReference.Should().Be(vacancyReference);
        report.VacancyTitle.Should().Be(review.VacancyTitle);
        report.SubmissionNumber.Should().Be(review.SubmissionCount);
        report.DateSubmitted.Should().BeCloseTo(review.CreatedDate, TimeSpan.FromSeconds(1));
        report.SlaDeadline.Should().BeCloseTo(review.SlaDeadLine, TimeSpan.FromSeconds(1));
        report.ReviewStarted.Should().Be(review.ReviewedDate);
        report.ReviewCompleted.Should().Be(review.ClosedDate);
        report.Outcome.Should().Be(review.ManualOutcome);
        report.VacancySubmittedBy.Should().Be(review.OwnerType.ToString());
        report.VacancySubmittedByUser.Should().Be(review.SubmittedByUserEmail);
        report.Employer.Should().Be(vacancy.LegalEntityName);
        report.DisplayName.Should().Be(vacancy.EmployerName);
        report.TrainingProvider.Should().Be(vacancy.TrainingProvider_Name);
        report.ProgrammeId.Should().Be(vacancy.ProgrammeId);
        report.ReviewedBy.Should().Be(review.ReviewedByUserEmail);
        report.ReviewerComment.Should().Be(review.ManualQaComment);
    }

    [Test]
    public async Task Then_An_Empty_List_Is_Returned_When_Report_Entity_Not_Found()
    {
        // arrange
        Server.DataContext.Setup(x => x.ReportEntities).ReturnsDbSet([]);
        Server.DataContext.Setup(x => x.VacancyReviewEntities).ReturnsDbSet([]);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([]);

        // act
        var response = await Client.GetAsync(new GetReportsGenerateQaByReportIdApiRequest(Guid.NewGuid()).GetUrl);
        var result = await response.Content.ReadAsAsync<GetQaReportResponse>();

        // assert
        response.EnsureSuccessStatusCode();
        result.QaReports.Should().BeEmpty();
    }

    [Test]
    public async Task Then_Only_Reviews_Within_Date_Range_Are_Returned()
    {
        // arrange
        var vacancyReference = 1000000002L;
        var fromDate = DateTime.UtcNow.AddDays(-10);
        var toDate = DateTime.UtcNow.AddDays(-1);

        var reportEntity = Fixture.Build<ReportEntity>()
            .With(x => x.OwnerType, ReportOwnerType.Qa)
            .With(x => x.CreatedDate, DateTime.UtcNow)
            .With(x => x.DynamicCriteria, JsonSerializer.Serialize(new ReportCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            }))
            .Create();

        var reviewInRange = Fixture.Build<VacancyReviewEntity>()
            .With(x => x.VacancyReference, vacancyReference)
            .With(x => x.CreatedDate, DateTime.UtcNow.AddDays(-5))
            .Create();

        var reviewOutOfRange = Fixture.Build<VacancyReviewEntity>()
            .With(x => x.VacancyReference, vacancyReference)
            .With(x => x.CreatedDate, DateTime.UtcNow.AddDays(-20))
            .Create();

        var vacancy = Fixture.Build<VacancyEntity>()
            .With(x => x.VacancyReference, vacancyReference)
            .Create();

        Server.DataContext.Setup(x => x.ReportEntities).ReturnsDbSet([reportEntity]);
        Server.DataContext.Setup(x => x.VacancyReviewEntities).ReturnsDbSet([reviewInRange, reviewOutOfRange]);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);

        // act
        var response = await Client.GetAsync(new GetReportsGenerateQaByReportIdApiRequest(reportEntity.Id).GetUrl);
        var result = await response.Content.ReadAsAsync<GetQaReportResponse>();

        // assert
        response.EnsureSuccessStatusCode();
        result.QaReports.Should().HaveCount(1);
        result.QaReports[0].VacancyReference.Should().Be(vacancyReference);
        result.QaReports[0].DateSubmitted.Should().BeCloseTo(reviewInRange.CreatedDate, TimeSpan.FromSeconds(1));
    }

    [Test]
    public async Task Then_A_Non_Qa_Report_Does_Not_Return_Results()
    {
        // arrange
        var reportEntity = Fixture.Build<ReportEntity>()
            .With(x => x.OwnerType, ReportOwnerType.Provider)
            .With(x => x.CreatedDate, DateTime.UtcNow)
            .With(x => x.DynamicCriteria, JsonSerializer.Serialize(new ReportCriteria
            {
                FromDate = DateTime.UtcNow.AddDays(-30),
                ToDate = DateTime.UtcNow
            }))
            .Create();

        Server.DataContext.Setup(x => x.ReportEntities).ReturnsDbSet([reportEntity]);
        Server.DataContext.Setup(x => x.VacancyReviewEntities).ReturnsDbSet(Fixture.CreateMany<VacancyReviewEntity>(5).ToList());
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet(Fixture.CreateMany<VacancyEntity>(5).ToList());

        // act
        var response = await Client.GetAsync(new GetReportsGenerateQaByReportIdApiRequest(reportEntity.Id).GetUrl);
        var result = await response.Content.ReadAsAsync<GetQaReportResponse>();

        // assert
        response.EnsureSuccessStatusCode();
        result.QaReports.Should().BeEmpty();
    }
}
