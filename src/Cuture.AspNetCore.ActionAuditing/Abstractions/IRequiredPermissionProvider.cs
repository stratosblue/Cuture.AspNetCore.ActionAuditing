using System.Collections.Immutable;

namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 需求权限提供器
/// </summary>
public interface IRequiredPermissionProvider
{
    #region Public 属性

    /// <summary>
    /// 权限列表
    /// </summary>
    ImmutableArray<string> Permissions { get; }

    #endregion Public 属性
}
