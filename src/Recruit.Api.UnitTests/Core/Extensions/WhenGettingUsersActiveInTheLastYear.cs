using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Extensions;

public class WhenGettingUsersActiveInTheLastYear
{
    [Test]
    public void Then_Users_Who_Have_Not_Logged_In_Are_Excluded()
    {
        // arrange
        var user = new UserEntity
        {
            LastSignedInDate = null,
            Name = "John Smith",
            Email = "john.smith"
        };

        // act
        var result = UserEntityExtensions.ActiveInTheLastYear([user]);

        // assert
        result.Should().BeEmpty();
    }
    
    [Test]
    public void Then_Users_Who_Have_Not_Logged_In_Within_The_Last_Year_Are_Excluded()
    {
        // arrange
        var user = new UserEntity
        {
            LastSignedInDate = DateTime.UtcNow.AddYears(-1).AddDays(-1),
            Name = "John Smith",
            Email = "john.smith"
        };

        // act
        var result = UserEntityExtensions.ActiveInTheLastYear([user]);

        // assert
        result.Should().BeEmpty();
    }
    
    [Test]
    public void Then_Users_Who_Have_Logged_In_Within_The_Last_Year_Are_Included()
    {
        // arrange
        var user = new UserEntity
        {
            LastSignedInDate = DateTime.UtcNow.AddYears(-1).AddSeconds(1),
            Name = "John Smith",
            Email = "john.smith"
        };

        // act
        var result = UserEntityExtensions.ActiveInTheLastYear([user]);

        // assert
        result.Should().BeEquivalentTo([user]);
    }
}