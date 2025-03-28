using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Recruit.Api.Data;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Database.Tests.DatabaseMock;
using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Enums;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Database.Tests.ApplicationReview
{
    [TestFixture]
    public class WhenGettingAllByAccountId
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_The_ApplicationReviews_Are_Returned_By_AccountId(
            long accountId,
            int pageNumber,
            int pageSize,
            string sortColumn,
            bool isAscending,
            CancellationToken token,
            List<ApplicationReviewEntity> applicationsReviews,
            [Frozen] Mock<IRecruitDataContext> context,
            [Greedy] ApplicationReviewRepository repository)
        {
            sortColumn = "CreatedDate";
            pageNumber = 1;
            pageSize = 10;
            foreach (var application in applicationsReviews)
            {
                application.AccountId = accountId;
            }

            var allApplications = new List<ApplicationReviewEntity>();
            allApplications.AddRange(applicationsReviews);

            context.Setup(x => x.ApplicationReviewEntities)
                .ReturnsDbSet(allApplications);

            var actual = await repository.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

            actual.Items.Should().BeEquivalentTo(applicationsReviews);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_The_ApplicationReviews_Are_Returned_By_AccountId(
            long accountId,
            ApplicationStatus status,
            CancellationToken token,
            List<ApplicationReviewEntity> applicationsReviews,
            [Frozen] Mock<IRecruitDataContext> context,
            [Greedy] ApplicationReviewRepository repository)
        {
            foreach (var application in applicationsReviews)
            {
                application.AccountId = accountId;
                application.Status = nameof(status);
            }

            var allApplications = new List<ApplicationReviewEntity>();
            allApplications.AddRange(applicationsReviews);

            context.Setup(x => x.ApplicationReviewEntities)
                .ReturnsDbSet(allApplications);

            var actual = await repository.GetAllByAccountId(accountId, nameof(status) , token);

            actual.Should().BeEquivalentTo(applicationsReviews);
        }
    }
}