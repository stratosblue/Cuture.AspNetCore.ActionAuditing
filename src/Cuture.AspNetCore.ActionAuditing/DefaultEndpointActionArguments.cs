using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 默认的 <inheritdoc cref="IActionArguments"/>
/// </summary>
/// <param name="argumentNameMap">参数名称映射</param>
/// <param name="arguments">参数列表</param>
public class DefaultEndpointActionArguments(IReadOnlyDictionary<string, int> argumentNameMap, IList<object?> arguments) : IActionArguments
{
    #region Public 属性

    /// <summary>
    /// 参数名称映射
    /// </summary>
    public IReadOnlyDictionary<string, int> ArgumentNameMap { get; } = argumentNameMap;

    /// <summary>
    /// 参数列表
    /// </summary>
    public IList<object?> Arguments { get; } = arguments;

    /// <inheritdoc/>
    public int Count => ArgumentNameMap.Count;

    #endregion Public 属性

    #region Public 方法

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        return Enumerate().GetEnumerator();

        IEnumerable<KeyValuePair<string, object?>> Enumerate()
        {
            foreach (var (name, index) in ArgumentNameMap)
            {
                yield return new(name, Arguments[index]);
            }
        }
    }

    /// <inheritdoc/>
    public bool Remove(string name)
    {
        if (ArgumentNameMap.TryGetValue(name, out var index))
        {
            Arguments[index] = null;
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public bool Set(string name, object? value)
    {
        if (ArgumentNameMap.TryGetValue(name, out var index))
        {
            Arguments[index] = value;
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetValue(string name, [MaybeNullWhen(false)] out object? value)
    {
        if (ArgumentNameMap.TryGetValue(name, out var index))
        {
            value = Arguments[index];
            return true;
        }
        value = default;
        return false;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion Public 方法

    /// <summary>
    /// 通过 <paramref name="filterFactoryContext"/> 创建参数名称索引映射
    /// </summary>
    /// <param name="filterFactoryContext"></param>
    public static IReadOnlyDictionary<string, int> CreateArgumentNameMap(EndpointFilterFactoryContext filterFactoryContext)
    {
        return filterFactoryContext.MethodInfo.GetParameters()
                                              .Select((info, index) => (info.Name, index))
                                              .ToDictionary(m => m.Name!, m => m.index, StringComparer.Ordinal)
                                              .AsReadOnly();
    }
}
