namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 执行权限审核器
/// </summary>
public interface IExecutingPermissionAuditor
{
    #region Public 方法

    /// <summary>
    /// 审查上下文 <paramref name="context"/> 是否有所描述的权限
    /// </summary>
    /// <param name="context">上下文</param>
    /// <param name="cancellationToken"></param>
    /// <returns>返回 <see langword="false"/> 则表示无权限，拒绝执行</returns>
    ValueTask<bool> AuditingAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken = default);

    #endregion Public 方法
}
