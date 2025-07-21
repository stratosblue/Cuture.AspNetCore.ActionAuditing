using System.Text.Json.Serialization;

namespace SampleFullAuditWebApp.Auditing.EntityFramework;

internal readonly struct DbContextOperationLog
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(RawJsonWriteOnlyJsonConverter))]
    public string? Details { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; }

    [JsonPropertyOrder(-900)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Target { get; }

    [JsonPropertyOrder(-1000)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DbContextOperationLogType Type { get; }

    [JsonConstructor]
    public DbContextOperationLog(DbContextOperationLogType type, string? target = null, string? details = null, string? message = null)
    {
        Type = type;
        Target = target;
        Details = details;
        Message = message;
    }

    public DbContextOperationLog(DbContextOperationLogType type, Type? target, string? details = null, string? message = null) : this(type, target: target?.FullName, details, message)
    {
    }
}
