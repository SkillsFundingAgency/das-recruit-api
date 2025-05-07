using System.Globalization;
using System.Text.Json.Serialization;
using SFA.DAS.Recruit.Api.Domain.Json;

namespace SFA.DAS.Recruit.Api.Domain.Models;

public class InvalidVacancyReferenceException(string vacancyReference) : Exception($"The value '{vacancyReference}' is not a valid Vacancy Reference");

[JsonConverter(typeof(VacancyReferenceJsonConverter))]
public sealed record VacancyReference: IEquatable<long>, IParsable<VacancyReference>
{
    public static readonly VacancyReference None = new();

    public long Value { get; }

    private VacancyReference()
    {
        Value = -1;
    }

    public VacancyReference(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = -1;
            return;
        }

        string baseValue = value.Replace("VAC", null, StringComparison.InvariantCultureIgnoreCase);
        if (!long.TryParse(baseValue, out long result) || result < 1)
        {
            throw new InvalidVacancyReferenceException(value);
        }

        Value = result;
    }
    
    public VacancyReference(long vacancyReference)
    {
        if (vacancyReference < 1)
        {
            throw new InvalidVacancyReferenceException($"{vacancyReference}");
        }
        
        Value = vacancyReference;
    }

    public static bool operator ==(VacancyReference? left, long right)
    {
        return left is not null && left.Equals(right);
    }

    public static bool operator !=(VacancyReference? left, long right)
    {
        return left is null || !left.Equals(right);
    }
    
    public static bool operator ==(long left, VacancyReference? right)
    {
        return right == left;
    }

    public static bool operator !=(long left, VacancyReference? right)
    {
        return right != left;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public bool Equals(long other)
    {
        return Value == other;
    }

    public bool Equals(VacancyReference? other)
    {
        return other switch
        {
            null when Value == -1 => true,
            null => false,
            _ => Value.Equals(other.Value)
        };
    }

    public override string ToString()
    {
        return this == None
            ? string.Empty
            : $"VAC{Value}";
    }
    
    public string ToShortString()
    {
        return this == None
            ? string.Empty
            : $"{Value}";
    }

    public static implicit operator VacancyReference(long value)
    {
        return new VacancyReference(value);
    }
    
    public static implicit operator VacancyReference(string value)
    {
        return new VacancyReference(value);
    }

    public static VacancyReference Parse(string value, IFormatProvider? provider)
    {
        if (TryParse(value, provider, out var result))
        {
            return result;
        }

        throw new InvalidVacancyReferenceException(value);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out VacancyReference result)
    {
        if (value is null)
        {
            result = None;
            return true;
        }

        try
        {
            result = new VacancyReference(value);
            return true;
        }
        catch
        {
            result = None;
            return false;
        }
    }

    public static bool TryParse(string? value, out VacancyReference result)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out result);
    }
}