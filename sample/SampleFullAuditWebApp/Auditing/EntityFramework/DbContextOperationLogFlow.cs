using System.Collections;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SampleFullAuditWebApp.Auditing.EntityFramework;

internal class DbContextOperationLogFlow : IEnumerable<DbContextOperationLog>
{
    private static readonly JavaScriptEncoder s_javaScriptEncoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    private readonly List<DbContextOperationLog> _logs = [];

    public void Error(string errorMessage) => Add(new(type: DbContextOperationLogType.Error, message: errorMessage));

    public void Error(Exception exception)
    {
        if (exception is AggregateException aggregateException)
        {
            Error(string.Join("\n-------\n", aggregateException.InnerExceptions.Select(m => m.Message)));
        }
        else
        {
            Error(exception.Message);
        }
    }

    public void LogEntries(IEnumerable<EntityEntry> entries)
    {
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case Microsoft.EntityFrameworkCore.EntityState.Deleted:
                    EntryDeleted(entry);
                    break;

                case Microsoft.EntityFrameworkCore.EntityState.Modified:
                    EntryModified(entry);
                    break;

                case Microsoft.EntityFrameworkCore.EntityState.Added:
                    EntryAdded(entry);
                    break;
            }
        }
    }

    public void SaveChangesEnd() => Add(new(type: DbContextOperationLogType.SaveChangesEnd));

    public void StartSaveChanges() => Add(new(type: DbContextOperationLogType.StartSaveChanges));

    private static string? EscapingToJSONString(object? value)
    {
        return value is not null
               ? value is string originString
                 ? s_javaScriptEncoder.Encode(originString)
                 : value.ToString() is string valueString
                   ? s_javaScriptEncoder.Encode(valueString)
                   : null
               : null;
    }

    private static StringBuilder ProcessArrayTail(StringBuilder builder)
    {
        if (builder.Length > 1)
        {
            builder.Remove(builder.Length - 1, 1);
        }
        return builder.Append(']');
    }

    private void Add(DbContextOperationLog log) => _logs.Add(log);

    private void EntryAdded(EntityEntry entry)
    {
        var builder = new StringBuilder("[", 256);
        foreach (var item in entry.Members)
        {
            builder.Append($"{{\"{item.Metadata.Name}\":\"{EscapingToJSONString(item.CurrentValue)}\"}},");
        }

        Add(new(type: DbContextOperationLogType.Add, target: entry.Metadata.ClrType, details: ProcessArrayTail(builder).ToString()));
    }

    private void EntryDeleted(EntityEntry entry)
    {
        var key = string.Empty;
        try
        {
            //TODO More ways to obtain primary key
            key = entry.IsKeySet
                  ? entry.Property("Id").OriginalValue?.ToString()
                  : string.Empty;
        }
        catch { }
        Add(new(type: DbContextOperationLogType.Delete, target: entry.Metadata.ClrType, details: $"[{{\"Key\":\"{key}\"}}]"));
    }

    private void EntryModified(EntityEntry entry)
    {
        var builder = new StringBuilder("[", 256);
        foreach (var item in entry.Members.Where(m => m.IsModified).OfType<PropertyEntry>())
        {
            builder.Append($"{{\"{item.Metadata.Name}\":\"{EscapingToJSONString(item.OriginalValue)} => {EscapingToJSONString(item.CurrentValue)}\"}},");
        }

        Add(new(type: DbContextOperationLogType.Modify, target: entry.Metadata.ClrType, details: ProcessArrayTail(builder).ToString()));
    }

    #region IEnumerable

    public IEnumerator<DbContextOperationLog> GetEnumerator() => _logs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion IEnumerable
}
