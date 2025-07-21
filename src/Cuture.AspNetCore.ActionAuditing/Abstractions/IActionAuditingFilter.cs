using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 动作审核过滤器
/// </summary>
public interface IActionAuditingFilter
{
    #region Public 方法

    /// <summary>
    /// 判断 <paramref name="httpContext"/> 是否需要进行审计
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>返回 <see langword="true"/> 则表示需要审计</returns>
    ValueTask<bool> PredicateAsync(HttpContext httpContext, CancellationToken cancellationToken);

    #endregion Public 方法
}
