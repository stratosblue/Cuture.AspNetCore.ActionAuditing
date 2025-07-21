namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 动作的审计描述
/// </summary>
/// <param name="Format">描述的格式字符串</param>
/// <param name="Description">描述</param>
public readonly record struct ActionAuditDescription(string Format, string Description)
{
    /// <inheritdoc/>
    public readonly override string ToString() => Description;
}
