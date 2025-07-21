using Cuture.AspNetCore.ActionAuditing.Abstractions;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 默认的 <inheritdoc cref="IAuditValueStore"/>
/// </summary>
public sealed class DefaultAuditValueStore : Dictionary<string, object?>, IAuditValueStore
{
    #region Public 构造函数

    /// <inheritdoc cref="DefaultAuditValueStore"/>
    public DefaultAuditValueStore() : base(StringComparer.Ordinal)
    {
    }

    /// <inheritdoc cref="DefaultAuditValueStore"/>
    public DefaultAuditValueStore(IEnumerable<KeyValuePair<string, object?>> collection) : base(collection, StringComparer.Ordinal)
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public bool Set(string name, object? value)
    {
        this[name] = value;
        return true;
    }

    #endregion Public 方法
}
