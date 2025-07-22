using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ActionAuditing.Test.TestBase;

/// <summary>
/// 审查时进行回调的测试基类
/// </summary>
public abstract class AuditingCallbackTestBase : TestServerBaseTest
{
    #region Protected 属性

    /// <summary>
    /// 当前的回调
    /// </summary>
    protected CallbackExecutingPermissionAuditor.AuditingDelegate CurrentAuditingCallback
    {
        get => ServiceScope.ServiceProvider.GetRequiredService<CallbackExecutingPermissionAuditor>().AuditingCallbak;
        set => ServiceScope.ServiceProvider.GetRequiredService<CallbackExecutingPermissionAuditor>().AuditingCallbak = value;
    }

    /// <summary>
    /// 当前的同步回调
    /// </summary>
    protected Func<ActionAuditingExecutingContext, PermissionAuditResult> CurrentAuditingSyncCallback
    {
        set => ServiceScope.ServiceProvider.GetRequiredService<CallbackExecutingPermissionAuditor>().AuditingCallbak = (context, _) => Task.FromResult(value(context));
    }

    #endregion Protected 属性

    #region Protected 方法

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<CallbackExecutingPermissionAuditor>();
        services.AddSingleton<IExecutingPermissionAuditor>(serviceProvider => serviceProvider.GetRequiredService<CallbackExecutingPermissionAuditor>());
        services.AddControllers()
                .AddActionAuditing(options => { });
    }

    #endregion Protected 方法
}
