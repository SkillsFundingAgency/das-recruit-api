using System.Text.Json;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Domain.Extensions;

public static class VacancyExtensions
{
    public static string GetLocationText(this VacancyEntity vacancy, JsonSerializerOptions serializerOptions)
    {
        if (vacancy.EmployerLocationOption == AvailableWhere.AcrossEngland)
        {
            return "Recruiting nationally";
        }

        try
        {
            var addresses = JsonSerializer.Deserialize<List<Address>>(vacancy.EmployerLocations!, serializerOptions);
            return addresses is null
                ? string.Empty
                : addresses.GetCityNames();
        }
        catch (JsonException)
        {
            return string.Empty;
        }
    }
}