using System;
using System.Threading.Tasks;
using System.Linq;
using FluentAssertions;
using SFA.DAS.Recruit.Api.Controllers;
using Xunit;

namespace SFA.DAS.Recruit.Api.UnitTests
{
    public class ValuesControllerTests
    {
        [Fact]
        public void GetCall_ReturnsTwoTestStrings()
        {
            var ctrlr = new ValuesController(null);
            var result = ctrlr.Get();
            result.Value.Count().Should().Be(2);
        }
    }
}