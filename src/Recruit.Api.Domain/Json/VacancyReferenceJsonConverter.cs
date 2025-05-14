using System.Text.Json;
using System.Text.Json.Serialization;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Domain.Json;

public class VacancyReferenceJsonConverter : JsonConverter<VacancyReference>
{
    public override VacancyReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => new VacancyReference(reader.GetString()),
            JsonTokenType.Number => new VacancyReference(reader.GetInt64()),
            _ => VacancyReference.None
        };
    }

    public override void Write(Utf8JsonWriter writer, VacancyReference value, JsonSerializerOptions options)
    {
        if (VacancyReference.None.Equals(value))
        {
            writer.WriteNullValue();
            return;
        }
        
        writer.WriteStringValue(value.ToString());
    }
    
    public override bool HandleNull => true;
}