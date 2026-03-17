#pragma warning disable IDE0130

using System.Collections.Immutable;
using Cuture.AspNetCore.ActionAuditing.Abstractions;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 描述需要进行审计的权限
/// </summary>
/// <param name="permissions">权限列表 (不传递此值以表示审计但不验证权限)</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public sealed class PermissionRequiredAttribute(params string[]? permissions)
    : Attribute, IRequiredPermissionProvider
{
    #region Public 属性

    /// <inheritdoc/>
    public ImmutableArray<string> Permissions { get; } = permissions is null || permissions.Length == 0 ? [] : ImmutableArray.Create(permissions);

    #endregion Public 属性
}
