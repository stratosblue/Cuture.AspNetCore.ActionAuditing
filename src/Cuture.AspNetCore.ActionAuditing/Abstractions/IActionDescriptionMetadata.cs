namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 动作描述元数据
/// </summary>
public interface IActionDescriptionMetadata
{
    #region Public 方法

    /// <summary>
    /// 描述的格式字符串
    /// </summary>
    public string Description { get; }

    #endregion Public 方法
}
