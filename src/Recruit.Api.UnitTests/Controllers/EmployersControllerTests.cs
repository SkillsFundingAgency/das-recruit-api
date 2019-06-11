using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.Recruit.Api.Controllers;
using Xunit;
using Moq;
using MediatR;
using SFA.DAS.Recruit.Api.Queries;
using System.Threading;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers
{
    public class EmployersControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly EmployersController _sut;
        private GetEmployerSummaryQuery _queryPassed;

        public EmployersControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockMediator.Setup(x => x.Send(It.IsAny<GetEmployerSummaryQuery>(), CancellationToken.None))
                        .ReturnsAsync(new GetEmployerSummaryResponse())
                        .Callback<GetEmployerSummaryQuery, CancellationToken>((q, _) => _queryPassed = q);
            _sut = new EmployersController(_mockMediator.Object);
        }

        [Theory]
        [InlineData(" myjr4x")]
        [InlineData("MYJR4X")]
        [InlineData(" myjR4X ")]
        public async Task GetCall_EnsuresEmployerAccountIdPassedInQueryPassedToMediatorIsTrimmedAndUppercased(string input)
        {
            var result = await _sut.Get(input);
            string.Compare(_queryPassed.EmployerAccountId, "MYJR4X", false).Should().Be(0);
        }
    }
}