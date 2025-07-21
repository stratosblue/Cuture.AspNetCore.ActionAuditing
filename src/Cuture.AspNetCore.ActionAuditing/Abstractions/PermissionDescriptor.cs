using System.Collections.Immutable;
using System.Diagnostics;

namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 权限描述符
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct PermissionDescriptor
{
    #region Private 字段

    private static readonly ImmutableDictionary<string, string?> s_emptyProperties = ImmutableDictionary<string, string?>.Empty;

    private readonly ImmutableDictionary<string, string?>? _properties;

    #endregion Private 字段

    #region Public 属性

    /// <summary>
    /// 是否已经定义权限
    /// </summary>
    public bool IsDefined => !Permissions.IsDefaultOrEmpty;

    /// <summary>
    /// 权限列表
    /// </summary>
    public ImmutableArray<string> Permissions { get; }

    /// <summary>
    /// 附加属性
    /// </summary>
    public ImmutableDictionary<string, string?> Properties => _properties ?? s_emptyProperties;

    #endregion Public 属性

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="PermissionDescriptor"/>
    /// </summary>
    /// <param name="permissions">权限列表</param>
    /// <param name="properties">附加属性</param>
    public PermissionDescriptor(IEnumerable<string> permissions, IEnumerable<KeyValuePair<string, string?>>? properties = null)
    {
        Permissions = [.. permissions.Distinct(StringComparer.Ordinal)];

        if (properties?.ToImmutableDictionary(StringComparer.Ordinal) is { Count: > 0 } validProperties)
        {
            _properties = validProperties;
        }
        else
        {
            _properties = s_emptyProperties;
        }
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public override string ToString() => $"[{string.Join(',', Permissions)}]";

    #endregion Public 方法
}
