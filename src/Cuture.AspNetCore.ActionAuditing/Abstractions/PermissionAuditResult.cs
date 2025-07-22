namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 执行权限审核结果
/// </summary>
/// <param name="IsApproved">已通过</param>
/// <param name="Reason">原因描述</param>
public record struct PermissionAuditResult(bool IsApproved, string? Reason = null)
{
    /// <summary>
    /// 隐式转换
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator PermissionAuditResult(bool value)
    {
        return new(value);
    }

    /// <summary>
    /// 隐式转换
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator bool(PermissionAuditResult value)
    {
        return value.IsApproved;
    }
}
