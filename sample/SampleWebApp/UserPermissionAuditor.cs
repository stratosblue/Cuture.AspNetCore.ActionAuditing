using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.EntityFrameworkCore;
using SampleWebApp.EntityFramework;

namespace SampleWebApp;

internal class UserPermissionAuditor(DataDbContext dataDbContext) : IExecutingPermissionAuditor
{
    public async ValueTask<bool> AuditingAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken = default)
    {
        //check permission with your logic
        const long UserId = 1234;
        var count = await dataDbContext.UserPermissions.CountAsync(m => m.Uid == UserId && context.RequiredPermission.Permissions.Contains(m.Permission), cancellationToken);
        return count == context.RequiredPermission.Permissions.Length;
    }
}
