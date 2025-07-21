#pragma warning disable IDE0130

using System.ComponentModel;
using Cuture.AspNetCore.ActionAuditing.Filters;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// 动作审计终结点拓展
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ActionAuditingEndpointFilterExtensions
{
    #region Public 方法

    /// <summary>
    /// 使用动作审计
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static TBuilder WithActionAuditing<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilterFactory(DefaultAuditingEndpointFilterFactory.CreateEndpointFilterDelegate);
        return builder;
    }

    #endregion Public 方法
}
