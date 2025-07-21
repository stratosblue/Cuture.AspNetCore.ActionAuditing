using System.Runtime.CompilerServices;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 默认审核动作过滤器
/// </summary>
public class DefaultActionAuditingFilter(ILogger<DefaultActionAuditingFilter> logger) : IActionAuditingFilter
{
    #region Protected 属性

    /// <summary>
    /// 根据 Endpoint对象 的结果缓存
    /// </summary>
    protected ConditionalWeakTable<Endpoint, BooleanValue> ActionDescriptorIdResultCache { get; } = [];

    #endregion Protected 属性

    #region Public 方法

    /// <inheritdoc/>
    public ValueTask<bool> PredicateAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        if (httpContext.GetEndpoint() is { } endpoint)
        {
            if (ActionDescriptorIdResultCache.TryGetValue(endpoint, out var cachedResult))
            {
                return ValueTask.FromResult(cachedResult.Value);
            }

            return InnerPredicateAsync(endpoint);
        }

        logger.LogWarning("Request {Path} cannot get endpoint to predicate for auditing.", httpContext.Request.Path);
        return ValueTask.FromResult(false);
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 判断 endpoint 是否需要审核
    /// </summary>
    /// <param name="endpoint"></param>
    /// <returns></returns>
    protected static BooleanValue PredicateContext(Endpoint endpoint)
    {
        // has NoAuditingAttribute without any IPermissionProvider
        if (endpoint.Metadata.OfType<NoAuditingAttribute>().Any()
            || !endpoint.Metadata.OfType<IRequiredPermissionProvider>().Any())
        {
            return BooleanValue.False;
        }
        return BooleanValue.True;
    }

    #endregion Protected 方法

    #region Private 方法

    private ValueTask<bool> InnerPredicateAsync(Endpoint endpoint)
    {
        var result = PredicateContext(endpoint);
        ActionDescriptorIdResultCache.TryAdd(endpoint, result);
        return ValueTask.FromResult(result.Value);
    }

    #endregion Private 方法

    /// <summary>
    /// 引用的<see cref="bool"/>
    /// </summary>
    /// <param name="Value"></param>
    protected sealed record BooleanValue(bool Value)
    {
        /// <summary>
        /// <see langword="false"/>
        /// </summary>
        public static BooleanValue False { get; } = new(false);

        /// <summary>
        /// <see langword="true"/>
        /// </summary>
        public static BooleanValue True { get; } = new(true);
    }
}
