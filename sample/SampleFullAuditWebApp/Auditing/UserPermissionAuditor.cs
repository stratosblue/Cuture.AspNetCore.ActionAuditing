using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SampleFullAuditWebApp.EntityFramework;

namespace SampleFullAuditWebApp.Auditing;

internal class UserPermissionAuditor(DataDbContext dataDbContext) : IExecutingPermissionAuditor
{
    public async ValueTask<PermissionAuditResult> AuditingAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken = default)
    {
        //check permission with your logic
        const long UserId = 1234;
        var count = await dataDbContext.UserPermissions.CountAsync(m => m.Uid == UserId && context.RequiredPermission.Permissions.Contains(m.Permission), cancellationToken);
        return count == context.RequiredPermission.Permissions.Length;
    }
}
