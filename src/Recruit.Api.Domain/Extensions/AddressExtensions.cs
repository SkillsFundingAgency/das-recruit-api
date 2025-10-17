using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Domain.Extensions;

public static class AddressExtensions
{
    public static string? GetLastNonEmptyField(this Address address)
    {
        return new[]
        {
            address.AddressLine4,
            address.AddressLine3,
            address.AddressLine2,
            address.AddressLine1,
        }.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
    }
    
    public static string GetCityNames(this List<Address> addresses)
    {
        var cityNames = addresses
            .Select(address => address.GetLastNonEmptyField())
            .Distinct()
            .ToList();

        return cityNames.Count == 1 && addresses.Count > 1
            ? $"{cityNames[0]} ({addresses.Count} available locations)"
            : string.Join(", ", cityNames.OrderBy(x => x));
    }
}