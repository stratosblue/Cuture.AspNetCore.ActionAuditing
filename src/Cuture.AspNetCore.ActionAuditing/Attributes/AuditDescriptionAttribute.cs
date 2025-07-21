#pragma warning disable IDE0130

using Cuture.AspNetCore.ActionAuditing.Abstractions;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 审计描述信息
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed partial class AuditDescriptionAttribute
    : Attribute, IActionDescriptionMetadata
{
    #region Public 属性

    /// <inheritdoc/>
    public string Description { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="AuditDescriptionAttribute"/>
    /// </summary>
    public AuditDescriptionAttribute(string description)
    {
        ArgumentNullException.ThrowIfNull(description);
        Description = description;
    }

    #endregion Public 构造函数
}
