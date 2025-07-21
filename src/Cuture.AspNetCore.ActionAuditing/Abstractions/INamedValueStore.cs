using System.Diagnostics.CodeAnalysis;

namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 具名值存储
/// </summary>
public interface INamedValueStore : IEnumerable<KeyValuePair<string, object?>>
{
    #region Public 属性

    /// <summary>
    /// 数量
    /// </summary>
    public int Count { get; }

    #endregion Public 属性

    #region Public 方法

    /// <summary>
    /// 移除 <paramref name="name"/> 的存储
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Remove(string name);

    /// <summary>
    /// 设置 <paramref name="name"/> 的存放值为 <paramref name="value"/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Set(string name, object? value);

    /// <summary>
    /// 尝试获取 <paramref name="name"/> 的值
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue(string name, [MaybeNullWhen(false)] out object? value);

    #endregion Public 方法
}

#region typed

/// <summary>
/// 动作参数
/// </summary>
public interface IActionArguments : INamedValueStore
{ }

/// <summary>
/// 审查数据值存储
/// </summary>
public interface IAuditValueStore : INamedValueStore
{ }

#endregion typed
