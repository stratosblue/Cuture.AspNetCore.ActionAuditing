namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 动作参数访问器
/// </summary>
public interface IActionAuditingExecutingContextAccessor
{
    #region Public 属性

    /// <summary>
    /// 当前动作的参数
    /// </summary>
    ActionAuditingExecutingContext? Current { get; set; }

    #endregion Public 属性
}
