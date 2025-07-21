#pragma warning disable IDE0130

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cuture.AspNetCore.ActionAuditing.Abstractions;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// <see cref="IAuditValueStore"/> 拓展
/// </summary>
public static class IAuditValueStoreExtensions
{
    #region Public 方法

    /// <summary>
    /// 获取值，如果不存在则设置为 <paramref name="value"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="valueStore"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T GetOrSet<T>(this IAuditValueStore valueStore,
                                string name,
                                T value)
    {
        if (valueStore.TryGetValue(name, out var storedValue)
            && storedValue is T typedValue)
        {
            return typedValue;
        }

        valueStore.Set(name, value);
        return value;
    }

    /// <summary>
    /// 获取值，如果不存在则设置为 <paramref name="valueFactory"/> 的返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="valueStore"></param>
    /// <param name="name"></param>
    /// <param name="valueFactory"></param>
    /// <returns></returns>
    public static T GetOrSet<T>(this IAuditValueStore valueStore,
                                string name,
                                Func<T> valueFactory)
    {
        if (valueStore.TryGetValue(name, out var storedValue)
            && storedValue is T typedValue)
        {
            return typedValue;
        }

        var value = valueFactory();
        valueStore.Set(name, value);
        return value;
    }

    /// <summary>
    /// 快速以 <paramref name="value"/> 的表达式为名称设置值
    /// </summary>
    /// <param name="valueStore"></param>
    /// <param name="value">需要设置的值</param>
    /// <param name="expression">表达式 (不要手段传递, 应当由编译器自动生成)</param>
    public static void SetValue(this IAuditValueStore valueStore,
                                object? value,
                                [CallerArgumentExpression(nameof(value))] string? expression = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expression);

        valueStore.Set(expression, value);
    }

    /// <inheritdoc cref="INamedValueStore.TryGetValue(string, out object?)"/>
    public static bool TryGetValue<T>(this IAuditValueStore valueStore,
                                      string name,
                                      [MaybeNullWhen(false)] out T? value)
    {
        if (valueStore.TryGetValue(name, out var storedValue)
            && storedValue is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }

    #endregion Public 方法
}
