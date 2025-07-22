using Cuture.AspNetCore.ActionAuditing.Abstractions;

namespace Cuture.AspNetCore.ActionAuditing.Test.TestBase;

public class CallbackExecutingPermissionAuditor : IExecutingPermissionAuditor
{
    #region Public 委托

    public delegate Task<PermissionAuditResult> AuditingDelegate(ActionAuditingExecutingContext context, CancellationToken cancellationToken = default);

    #endregion Public 委托

    #region Public 属性

    public AuditingDelegate AuditingCallbak { get; set; } = null!;

    #endregion Public 属性

    #region Public 方法

    public async ValueTask<PermissionAuditResult> AuditingAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken = default)
    {
        return await AuditingCallbak(context, cancellationToken);
    }

    #endregion Public 方法
}
