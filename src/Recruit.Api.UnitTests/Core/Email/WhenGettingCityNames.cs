using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingCityNames
{
    [Test]
    public void Then_The_City_Names_Should_Be_Appended_Correctly()
    {
        // Arrange
        var addresses = new List<Address>
        {
            new() { AddressLine1 = "123 Main St", AddressLine2 = "CityA", Postcode = "12345" },
            new() { AddressLine1 = "456 Elm St", AddressLine2 = "CityB", Postcode = "67890" },
            new() { AddressLine1 = "789 Oak St", AddressLine2 = "CityA", Postcode = "54321" }
        };

        // Act
        string result = addresses.GetCityNames();

        // Assert
        result.Should().Be("CityA, CityB");
    }

    [Test]
    public void Then_The_City_Names_Should_Be_Ordered_Correctly()
    {
        // Arrange
        var addresses1 = new List<Address>
        {
            new() { AddressLine1 = "456 Elm St", AddressLine2 = "CityB", Postcode = "67890" },
            new() { AddressLine1 = "123 Main St", AddressLine2 = "CityA", Postcode = "12345" },
            new() { AddressLine1 = "789 Oak St", AddressLine2 = "CityA", Postcode = "54321" }
        };
        
        var addresses2 = new List<Address>
        {
            new() { AddressLine1 = "789 Oak St", AddressLine2 = "CityA", Postcode = "54321" },
            new() { AddressLine1 = "123 Main St", AddressLine2 = "CityA", Postcode = "12345" },
            new() { AddressLine1 = "456 Elm St", AddressLine2 = "CityB", Postcode = "67890" },
        };

        // Act
        string result1 = addresses1.GetCityNames();
        string result2 = addresses2.GetCityNames();

        // Assert
        result1.Should().Be(result2);
    }

    [Test]
    public void Then_Addresses_In_The_Same_City_Should_Be_Collated_Correctly()
    {
        // Arrange
        var addresses = new List<Address>
        {
            new() { AddressLine1 = "456 Elm St", AddressLine2 = "CityA", Postcode = "67890" },
            new() { AddressLine1 = "123 Main St", AddressLine2 = "CityA", Postcode = "67890" },
            new() { AddressLine1 = "789 Oak St", AddressLine2 = "CityA", Postcode = "67890" }
        };

        // Act
        string result = addresses.GetCityNames();

        // Assert
        result.Should().Be("CityA (3 available locations)");
    }
}