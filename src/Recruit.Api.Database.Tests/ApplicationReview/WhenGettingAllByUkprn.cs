using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Recruit.Api.Data;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Database.Tests.DatabaseMock;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Database.Tests.ApplicationReview
{
    [TestFixture]
    public class WhenGettingAllByUkprn
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_The_ApplicationReviews_Are_Returned_By_Ukprn(
            int ukprn,
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
                application.Ukprn = ukprn;
            }

            var allApplications = new List<ApplicationReviewEntity>();
            allApplications.AddRange(applicationsReviews);

            context.Setup(x => x.ApplicationReviewEntities)
                .ReturnsDbSet(allApplications);

            var actual = await repository.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);

            actual.Should().BeEquivalentTo(applicationsReviews);
        }
    }
}