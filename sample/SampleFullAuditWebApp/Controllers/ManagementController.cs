using Cuture.AspNetCore.ActionAuditing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleFullAuditWebApp.Auditing;
using SampleFullAuditWebApp.EntityFramework;

namespace SampleFullAuditWebApp.Controllers;

[ApiController]
[Route("[controller]/[Action]")]
[FeatureName("Management")]
public class ManagementController(DataDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [PermissionRequired("ReadPermission")]
    [AuditDescription("Get SystemAuditingLog with page: {page}, pageSize: {pageSize}.")]
    public Task<List<SystemAuditingLog>> GetLogsAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        return dbContext.SystemAuditingLogs.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }
}
