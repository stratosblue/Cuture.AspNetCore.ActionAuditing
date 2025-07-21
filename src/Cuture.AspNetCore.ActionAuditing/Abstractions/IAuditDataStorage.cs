namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 审计数据存储器
/// </summary>
public interface IAuditDataStorage
{
    #region Public 方法

    /// <summary>
    /// 添加审计数据
    /// </summary>
    /// <param name="context">审计的执行上下文</param>
    /// <param name="cancellationToken">当次请求的取消令牌</param>
    /// <returns></returns>
    public Task AddAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken);

    #endregion Public 方法
}
