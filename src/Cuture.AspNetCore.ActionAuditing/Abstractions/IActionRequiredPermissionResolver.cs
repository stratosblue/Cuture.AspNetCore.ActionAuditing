using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 动作所需权限解析器
/// </summary>
public interface IActionRequiredPermissionResolver
{
    #region Public 方法

    /// <summary>
    /// 解析上下文 <paramref name="httpContext"/> 需求的权限
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>需要的权限描述</returns>
    public ValueTask<PermissionDescriptor> ResolveAsync(HttpContext httpContext, CancellationToken cancellationToken = default);

    #endregion Public 方法
}
