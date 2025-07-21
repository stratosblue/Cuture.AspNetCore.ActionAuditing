namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 动作审核处理器
/// </summary>
public interface IActionAuditingHandler
{
    #region Public 方法

    /// <summary>
    /// 处理拒绝
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask HandleDeniedAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken);

    /// <summary>
    /// 处理执行异常
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask HandleExceptionAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken);

    /// <summary>
    /// 处理执行成功
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask HandleSuccessAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken);

    #endregion Public 方法
}
