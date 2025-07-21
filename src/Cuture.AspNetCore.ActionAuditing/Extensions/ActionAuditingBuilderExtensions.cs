#pragma warning disable IDE0130

using System.ComponentModel;
using Cuture.AspNetCore.ActionAuditing;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 构造拓展
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ActionAuditingBuilderExtensions
{
    #region Public 方法

    /// <summary>
    /// 移除默认的动作过滤器（当需要自行实现过滤器时调用此方法移除默认过滤器）
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ActionAuditingBuilder RemoveDefaultActionFilter(this ActionAuditingBuilder builder)
    {
        builder.MvcBuilder.AddMvcOptions(static options =>
        {
            var removeItems = options.Filters.OfType<TypeFilterAttribute>().Where(static m => m.ImplementationType == typeof(DefaultActionAuditingFilter)).ToList();
            foreach (var item in removeItems)
            {
                options.Filters.Remove(item);
            }
        });
        return builder;
    }

    /// <summary>
    /// 设置使用的 <see cref="IActionAuditingHandler"/>
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="builder"></param>
    /// <param name="serviceLifetime"></param>
    /// <returns></returns>
    public static ActionAuditingBuilder UseAuditingHandler<THandler>(this ActionAuditingBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where THandler : class, IActionAuditingHandler
    {
        var serviceDescriptor = ServiceDescriptor.Describe(typeof(IActionAuditingHandler), typeof(THandler), serviceLifetime);
        builder.Services.Replace(serviceDescriptor);
        return builder;
    }

    /// <summary>
    /// 设置使用的 <see cref="IExecutingPermissionAuditor"/>
    /// </summary>
    /// <typeparam name="TAuditor"></typeparam>
    /// <param name="builder"></param>
    /// <param name="serviceLifetime"></param>
    /// <returns></returns>
    public static ActionAuditingBuilder UsePermissionAuditor<TAuditor>(this ActionAuditingBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TAuditor : class, IExecutingPermissionAuditor
    {
        var serviceDescriptor = ServiceDescriptor.Describe(typeof(IExecutingPermissionAuditor), typeof(TAuditor), serviceLifetime);
        builder.Services.Replace(serviceDescriptor);
        return builder;
    }

    /// <summary>
    /// 设置使用的 <see cref="IActionRequiredPermissionResolver"/>
    /// </summary>
    /// <typeparam name="TResolver"></typeparam>
    /// <param name="builder"></param>
    /// <param name="serviceLifetime"></param>
    /// <returns></returns>
    public static ActionAuditingBuilder UseRequiredPermissionResolver<TResolver>(this ActionAuditingBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TResolver : class, IActionRequiredPermissionResolver
    {
        var serviceDescriptor = ServiceDescriptor.Describe(typeof(IActionRequiredPermissionResolver), typeof(TResolver), serviceLifetime);
        builder.Services.Replace(serviceDescriptor);
        return builder;
    }

    /// <summary>
    /// 设置使用的 <see cref="IAuditDataStorage"/>
    /// </summary>
    /// <typeparam name="TStorage"></typeparam>
    /// <param name="builder"></param>
    /// <param name="serviceLifetime"></param>
    /// <returns></returns>
    public static ActionAuditingBuilder UseStorage<TStorage>(this ActionAuditingBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TStorage : class, IAuditDataStorage
    {
        var serviceDescriptor = ServiceDescriptor.Describe(typeof(IAuditDataStorage), typeof(TStorage), serviceLifetime);
        builder.Services.Replace(serviceDescriptor);
        return builder;
    }

    #endregion Public 方法
}
