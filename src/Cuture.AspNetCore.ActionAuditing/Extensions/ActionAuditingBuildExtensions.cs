#pragma warning disable IDE0130

using System.ComponentModel;
using Cuture.AspNetCore.ActionAuditing;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Cuture.AspNetCore.ActionAuditing.Filters;
using Cuture.AspNetCore.ActionAuditing.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 构造拓展
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ActionAuditingBuildExtensions
{
    #region Public 方法

    /// <summary>
    /// 添加动作审核功能
    /// </summary>
    /// <param name="mvcBuilder"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddActionAuditing(this IMvcBuilder mvcBuilder, Action<ActionAuditingBuilder> setupAction)
    {
        ArgumentNullException.ThrowIfNull(mvcBuilder);
        ArgumentNullException.ThrowIfNull(setupAction);

        mvcBuilder.AddMvcOptions(static options =>
        {
            if (options.Filters.OfType<TypeFilterAttribute>().Any(static m => m.ImplementationType == typeof(DefaultAuditingActionFilter)))
            {
                return;
            }
            options.Filters.Add<DefaultAuditingActionFilter>();
        });

        var services = mvcBuilder.Services;
        var builder = new ActionAuditingBuilder(mvcBuilder, services);

        setupAction(builder);

        if (!services.Any(static m => m.ServiceType == typeof(IExecutingPermissionAuditor)))
        {
            throw new InvalidOperationException($"Service \"{nameof(IExecutingPermissionAuditor)}\" must be set up. Please use \"{nameof(ActionAuditingBuilderExtensions.UsePermissionAuditor)}\" with the builder to set it.");
        }

        if (!services.Any(static m => m.ServiceType == typeof(IActionAuditingHandler)))
        {
            services.AddScoped<IActionAuditingHandler, DefaultActionAuditingHandler>();
        }

        services.AddHttpContextAccessor();
        services.TryAddSingleton<IAuditValueStoreAccessor, HttpContextAuditValueStoreAccessor>();
        services.TryAddTransient<IAuditValueStore>(static serviceProvider =>
        {
            var auditValueStoreAccessor = serviceProvider.GetRequiredService<IAuditValueStoreAccessor>();
            if (!auditValueStoreAccessor.Initialize())
            {
                throw new InvalidOperationException($"Cannot initialize \"{nameof(IAuditValueStoreAccessor)}\" - {auditValueStoreAccessor.GetType()}");
            }
            return auditValueStoreAccessor.Current;
        });

        services.TryAddSingleton<IActionRequiredPermissionResolver, DefaultActionRequiredPermissionResolver>();
        services.TryAddSingleton<IActionAuditingFilter, DefaultActionAuditingFilter>();
        services.TryAddScoped<IActionAuditingExecutingContextAccessor, HttpContextActionAuditingExecutingContextAccessor>();

        return services;
    }

    #endregion Public 方法
}
