using System.Reflection;

namespace Cuture.AspNetCore.ActionAuditing.Internal;

internal sealed class ReflectionObjectAccessor
{
    #region Private 字段

    private readonly Func<object?, object?>? _accessDelegate;

    #endregion Private 字段

    #region Public 属性

    public VariablePropertyAccessPath PropertyAccessPath { get; }

    #endregion Public 属性

    #region Public 构造函数

    public ReflectionObjectAccessor(VariablePropertyAccessPath propertyAccessPath)
    {
        propertyAccessPath.ThrowIfInvalid();
        PropertyAccessPath = propertyAccessPath;

        if (propertyAccessPath.Paths.Length > 0)
        {
            _accessDelegate = AccessObject;
        }
    }

    #endregion Public 构造函数

    #region Public 方法

    public static ReflectionObjectAccessor CreateFromExpression(string expression)
    {
        var path = VariablePropertyAccessPath.Parse(expression);
        return new(path);
    }

    public object? Access(object? target)
    {
        if (_accessDelegate is null)
        {
            return target;
        }
        return _accessDelegate(target);
    }

    /// <inheritdoc/>
    public override string ToString() => PropertyAccessPath.ToString();

    #endregion Public 方法

    #region Private 方法

    private object? AccessObject(object? target)
    {
        if (target is null)
        {
            return null;
        }

        foreach (var propertyName in PropertyAccessPath.Paths)
        {
            if (target is null)
            {
                return null;
            }

            var type = target.GetType();

            target = type.GetRuntimeProperty(propertyName)?.GetValue(target)
                     ?? type.GetRuntimeField(propertyName)?.GetValue(target);
        }
        return target;
    }

    #endregion Private 方法
}
