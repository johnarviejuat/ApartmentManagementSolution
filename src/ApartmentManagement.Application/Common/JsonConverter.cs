using ApartmentManagement.Domain.Leasing.Apartments;
using ApartmentManagement.Domain.Leasing.Tenants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApartmentManagement.Application.Common;
public sealed class TenantIdJsonConverter : JsonConverter<TenantId>
{
    public override TenantId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // Fast path
            if (reader.TryGetGuid(out var g)) return new TenantId(g);

            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s))
                return new TenantId(Guid.Empty);              // let FV catch it

            if (Guid.TryParse(s, out g))
                return new TenantId(g);

            // still invalid
            return new TenantId(Guid.Empty);                  // let FV catch it
        }

        if (reader.TokenType == JsonTokenType.Null)
            return new TenantId(Guid.Empty);                  // let FV catch it

        throw new JsonException($"Unexpected token {reader.TokenType} for {nameof(TenantId)}; expected string.");
    }

    public override void Write(Utf8JsonWriter writer, TenantId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}

public sealed class ApartmentIdJsonConverter : JsonConverter<ApartmentId>
{
    public override ApartmentId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (reader.TryGetGuid(out var g)) return new ApartmentId(g);

            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s))
                return new ApartmentId(Guid.Empty);           // let FV catch it

            if (Guid.TryParse(s, out g))
                return new ApartmentId(g);

            return new ApartmentId(Guid.Empty);               // let FV catch it
        }

        if (reader.TokenType == JsonTokenType.Null)
            return new ApartmentId(Guid.Empty);               // let FV catch it

        throw new JsonException($"Unexpected token {reader.TokenType} for {nameof(ApartmentId)}; expected string.");
    }

    public override void Write(Utf8JsonWriter writer, ApartmentId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
