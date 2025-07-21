using System.Text.Json;
using System.Text.Json.Serialization;

namespace SampleFullAuditWebApp.Auditing;

internal class RawJsonWriteOnlyJsonConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Serialize only
        throw new NotSupportedException();
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteRawValue(value, true);
        }
    }
}
