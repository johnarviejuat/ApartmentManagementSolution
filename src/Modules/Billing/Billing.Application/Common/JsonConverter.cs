using System.Text.Json;
using System.Text.Json.Serialization;

namespace Billing.Application.Common;

[JsonConverter(typeof(TenantRefIdJsonConverter))]
public readonly record struct TenantRefId(Guid Value)
{
    public override string ToString() => Value.ToString();
    public static implicit operator Guid(TenantRefId id) => id.Value;
    public static explicit operator TenantRefId(Guid value) => new(value);
}
[JsonConverter(typeof(ApartmentRefIdJsonConverter))]
public readonly record struct ApartmentRefId(Guid Value)
{
    public override string ToString() => Value.ToString();
    public static implicit operator Guid(ApartmentRefId id) => id.Value;
    public static explicit operator ApartmentRefId(Guid value) => new(value);
}

public sealed class TenantRefIdJsonConverter : JsonConverter<TenantRefId>
{
    public override TenantRefId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && reader.TryGetGuid(out var g))
            return new TenantRefId(g);

        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (Guid.TryParse(s, out g)) return new TenantRefId(g);
            return new TenantRefId(Guid.Empty); // let validators catch empty/invalid
        }

        if (reader.TokenType == JsonTokenType.Null)
            return new TenantRefId(Guid.Empty);

        throw new JsonException($"Unexpected token {reader.TokenType} for {nameof(TenantRefId)}; expected string.");
    }

    public override void Write(Utf8JsonWriter writer, TenantRefId value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}

public sealed class ApartmentRefIdJsonConverter : JsonConverter<ApartmentRefId>
{
    public override ApartmentRefId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && reader.TryGetGuid(out var g))
            return new ApartmentRefId(g);

        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (Guid.TryParse(s, out g)) return new ApartmentRefId(g);
            return new ApartmentRefId(Guid.Empty); // let validators catch empty/invalid
        }

        if (reader.TokenType == JsonTokenType.Null)
            return new ApartmentRefId(Guid.Empty);

        throw new JsonException($"Unexpected token {reader.TokenType} for {nameof(ApartmentRefId)}; expected string.");
    }

    public override void Write(Utf8JsonWriter writer, ApartmentRefId value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
