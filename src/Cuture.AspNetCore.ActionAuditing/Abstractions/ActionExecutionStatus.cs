namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 动作执行标识
/// </summary>
[Flags]
public enum ActionExecutionFlag
{
    /// <summary>
    /// 未定义
    /// </summary>
    Undefined,

    /// <summary>
    /// 没有要求权限审核
    /// </summary>
    NoPermissionAuditRequired = 1 << 0,

    /// <summary>
    /// 权限审核通过
    /// </summary>
    PermissionAuditApproved = 1 << 1,

    /// <summary>
    /// 权限审核拒绝
    /// </summary>
    PermissionAuditDenied = 1 << 2,

    /// <summary>
    /// 执行成功
    /// </summary>
    Success = 1 << 3,

    /// <summary>
    /// 执行失败
    /// </summary>
    Failed = 1 << 4
}
