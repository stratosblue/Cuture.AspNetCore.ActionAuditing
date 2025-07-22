using Cuture.AspNetCore.ActionAuditing.Abstractions;

namespace Cuture.AspNetCore.ActionAuditing.TestHost;

public sealed class NoopExecutingPermissionAuditor : IExecutingPermissionAuditor
{
    #region Public 方法

    public ValueTask<PermissionAuditResult> AuditingAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<PermissionAuditResult>(true);
    }

    #endregion Public 方法
}
