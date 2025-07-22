namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 动作审计执行上下文访问器
/// </summary>
public interface IActionAuditingExecutingContextAccessor
{
    #region Public 属性

    /// <summary>
    /// 当前动作的审计执行上下文
    /// </summary>
    ActionAuditingExecutingContext? Current { get; set; }

    #endregion Public 属性
}
