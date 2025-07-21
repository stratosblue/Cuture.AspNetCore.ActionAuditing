using System.Collections.Immutable;

namespace Cuture.AspNetCore.ActionAuditing.Internal;

/// <summary>
/// 变量属性访问路径
/// </summary>
/// <param name="Expression"></param>
/// <param name="VariableName"></param>
/// <param name="Paths"></param>
internal readonly record struct VariablePropertyAccessPath(string Expression, string VariableName, ImmutableArray<string> Paths)
{
    /// <summary>
    /// 是否为直接变量访问
    /// </summary>
    public readonly bool IsDirectVariableAccess { get; } = string.Equals(Expression, VariableName);

    /// <summary>
    /// 解析变量访问表达式
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static VariablePropertyAccessPath Parse(string expression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expression);

        var items = expression.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (items.Length == 0)
        {
            throw new ArgumentException($"Expression \"{expression}\" is invalid.", nameof(expression));
        }
        return new(expression.Trim(), items[0], [.. items[1..]]);
    }

    /// <summary>
    /// 当前对象无效时抛出异常
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public readonly void ThrowIfInvalid()
    {
        if (string.IsNullOrWhiteSpace(VariableName))
        {
            throw new InvalidOperationException($"AccessPath {this} is invalid");
        }
    }

    /// <inheritdoc/>
    public readonly override string ToString() => Expression;
}
