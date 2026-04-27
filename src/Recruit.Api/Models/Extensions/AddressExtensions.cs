using SFA.DAS.Recruit.Api.Validators;

namespace SFA.DAS.Recruit.Api.Models.Extensions;

public static class AddressExtensions
{
    public static string Flatten(this Address address)
    {
        return new[]
        {
            address.AddressLine1,
            address.AddressLine2,
            address.AddressLine3,
            address.AddressLine4,
            address.Postcode
        }.Where(x => !string.IsNullOrWhiteSpace(x)).ToDelimitedString(", ");
    }
}