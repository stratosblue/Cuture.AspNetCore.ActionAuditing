#pragma warning disable IDE0130

using System.Collections.Immutable;
using Cuture.AspNetCore.ActionAuditing.Abstractions;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 描述需要的权限
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public sealed class PermissionRequiredAttribute
    : Attribute, IRequiredPermissionProvider
{
    #region Public 属性

    /// <inheritdoc/>
    public ImmutableArray<string> Permissions { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="PermissionRequiredAttribute"/>
    /// </summary>
    /// <param name="permissions">权限列表</param>
    public PermissionRequiredAttribute(params string[] permissions)
    {
        if (permissions is null
            || permissions.Length == 0)
        {
            throw new ArgumentException("Permissions must include values", nameof(permissions));
        }

        Permissions = ImmutableArray.Create(permissions);
    }

    #endregion Public 构造函数
}
