using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Cuture.AspNetCore.ActionAuditing.Internal;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// <see cref="ActionAuditDescription"/> 格式化器
/// </summary>
public static partial class ActionAuditDescriptionFormatter
{
    #region Private 字段

    /// <summary>
    /// 针对格式化描述文本的引用缓存，通过Attribute产生时，对应字符串会驻留，此缓存生效，其它方式产生格式化描述文本时缓存失效，会有资源浪费
    /// </summary>
    private static readonly ConditionalWeakTable<string, ActionDescriptionFormatterCache> s_formatterCache = [];

    #endregion Private 字段

    #region Public 方法

    /// <summary>
    /// 获取 <paramref name="format"/> 格式化后的描述
    /// </summary>
    /// <param name="format"></param>
    /// <param name="valueStore"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static ActionAuditDescription Format(string format, IAuditValueStore? valueStore, IActionArguments? arguments)
    {
        if (s_formatterCache.TryGetValue(format, out var formatterCache))
        {
            return formatterCache.Format(valueStore, arguments);
        }

        formatterCache = new ActionDescriptionFormatterCache(format);
        s_formatterCache.TryAdd(format, formatterCache);
        return formatterCache.Format(valueStore, arguments);
    }

    #endregion Public 方法

    #region Internal 类

    internal sealed partial class ActionDescriptionFormatterCache
    {
        #region Internal 字段

        internal readonly ReflectionObjectAccessor[] _objectAccessors;

        #endregion Internal 字段

        #region Private 字段

        private const string Error = "[Error]";

        private const string Null = "null";

        private static readonly Regex s_interpolationExpressionRegex = GetInterpolationExpressionRegex();

        private readonly string?[] _descriptionFormatSegments;

        #endregion Private 字段

        #region Public 属性

        public string DescriptionFormat { get; }

        #endregion Public 属性

        #region Public 构造函数

        public ActionDescriptionFormatterCache(string descriptionFormat)
        {
            ArgumentNullException.ThrowIfNull(descriptionFormat);
            DescriptionFormat = descriptionFormat;

            var matches = s_interpolationExpressionRegex.EnumerateMatches(descriptionFormat);
            var sourceIndex = 0;
            List<string?> segments = [];
            List<ReflectionObjectAccessor> propertyAccessors = [];

            while (matches.MoveNext())
            {
                var current = matches.Current;
                if (current.Index > sourceIndex)
                {
                    segments.Add(descriptionFormat[sourceIndex..current.Index]);
                    sourceIndex = current.Index;
                }

                propertyAccessors.Add(ReflectionObjectAccessor.CreateFromExpression(descriptionFormat[(sourceIndex + 1)..(current.Index + current.Length - 1)]));
                segments.Add(null);

                sourceIndex += current.Length;
            }

            if (sourceIndex == 0)
            {
                _descriptionFormatSegments = [];
                _objectAccessors = [];
            }
            else
            {
                if (sourceIndex < descriptionFormat.Length)
                {
                    segments.Add(descriptionFormat[sourceIndex..]);
                }

                _descriptionFormatSegments = [.. segments];
                _objectAccessors = [.. propertyAccessors];
            }
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public ActionAuditDescription Format(IAuditValueStore? valueStore, IActionArguments? arguments)
        {
            if (_objectAccessors.Length == 0)
            {
                return new(DescriptionFormat, DescriptionFormat);
            }

            int propertyIndex = 0;
            var builder = new StringBuilder(DescriptionFormat.Length * 2);
            for (var i = 0; i < _descriptionFormatSegments.Length; i++)
            {
                if (_descriptionFormatSegments[i] is { } currentSegment)
                {
                    builder.Append(currentSegment);
                }
                else
                {
                    try
                    {
                        Debug.Assert(propertyIndex < _objectAccessors.Length);

                        var propertyAccessor = _objectAccessors[propertyIndex++];
                        var accessPath = propertyAccessor.PropertyAccessPath;

                        object? value = null;
                        if (accessPath.IsDirectVariableAccess)
                        {
                            //存储表达式 -> 参数
                            if (valueStore?.TryGetValue(accessPath.Expression, out value) != true)
                            {
                                arguments?.TryGetValue(accessPath.Expression, out value);
                            }
                        }
                        else if (valueStore?.TryGetValue(accessPath.Expression, out value) != true)
                        {
                            //存储表达式 -> 存储变量 -> 参数
                            if (valueStore?.TryGetValue(accessPath.VariableName, out value) == true
                                || arguments?.TryGetValue(accessPath.VariableName, out value) == true)
                            {
                                value = propertyAccessor.Access(value);
                            }
                        }

                        builder.Append(value ?? Null);
                    }
                    catch
                    {
                        builder.Append(Error);
                    }
                }
            }
            return new(DescriptionFormat, builder.ToString());
        }

        #endregion Public 方法

        #region Private 方法

        [GeneratedRegex("{.+?}", RegexOptions.CultureInvariant)]
        private static partial Regex GetInterpolationExpressionRegex();

        #endregion Private 方法
    }

    #endregion Internal 类
}
