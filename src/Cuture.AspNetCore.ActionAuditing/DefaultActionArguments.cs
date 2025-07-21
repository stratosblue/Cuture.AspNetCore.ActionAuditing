using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Cuture.AspNetCore.ActionAuditing.Abstractions;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 默认的 <inheritdoc cref="IActionArguments"/>
/// </summary>
/// <param name="arguments"></param>
public class DefaultActionArguments(IDictionary<string, object?> arguments) : IActionArguments
{
    #region Public 属性

    /// <summary>
    /// 参数
    /// </summary>
    public IDictionary<string, object?> Arguments { get; } = arguments;

    /// <inheritdoc/>
    public int Count => Arguments.Count;

    #endregion Public 属性

    #region Public 方法

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => Arguments.GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(string name) => Arguments.Remove(name);

    /// <inheritdoc/>
    public bool Set(string name, object? value)
    {
        Arguments[name] = value;
        return true;
    }

    /// <inheritdoc/>
    public bool TryGetValue(string name, [MaybeNullWhen(false)] out object? value) => Arguments.TryGetValue(name, out value);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion Public 方法
}
