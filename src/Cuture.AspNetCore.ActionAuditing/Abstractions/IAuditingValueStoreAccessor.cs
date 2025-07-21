using System.Diagnostics.CodeAnalysis;

namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// <see cref="IAuditValueStore"/> 访问器
/// </summary>
public interface IAuditValueStoreAccessor
{
    #region Public 属性

    /// <summary>
    /// 当前的 <see cref="IAuditValueStore"/>
    /// </summary>
    public IAuditValueStore? Current { get; }

    /// <summary>
    /// 初始化 <see cref="Current"/>
    /// </summary>
    /// <returns></returns>
    [MemberNotNullWhen(true, nameof(Current))]
    public bool Initialize();

    #endregion Public 属性
}
