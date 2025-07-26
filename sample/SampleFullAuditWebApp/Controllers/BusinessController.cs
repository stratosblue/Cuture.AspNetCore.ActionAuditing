using Cuture.AspNetCore.ActionAuditing;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleFullAuditWebApp.Auditing;
using SampleFullAuditWebApp.EntityFramework;

namespace SampleFullAuditWebApp.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[FeatureName("BusinessName")]
[PermissionRequired(PermissionDefine.SampleBusiness.ViewConstant)]
public class BusinessController(DataDbContext dbContext, IAuditValueStore auditValueStore) : ControllerBase
{
    [HttpGet]
    [AuditDescription("Get BusinessData with page: {page}, pageSize: {pageSize}. Returned: {result.Length}.")]
    public async Task<IEnumerable<BusinessData>> Get(int page, int pageSize, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        if (pageSize > 20)
        {
            pageSize = 20;
        }

        var result = await dbContext.BusinessDatas.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        auditValueStore.SetValue(result);

        return result;
    }

    [HttpPost]
    [PermissionRequired(PermissionDefine.SampleBusiness.WriteBusinessDataConstant)]
    [AuditDescription("Add BusinessData {businessData.Name}.")]
    public async Task<BusinessData> Set(BusinessData businessData, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(businessData, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return businessData;
    }

    [HttpPost]
    [PermissionRequired(PermissionDefine.SampleBusiness.WriteBusinessDataConstant)]
    [AuditDescription("Update BusinessData {businessData.Name}.")]
    public async Task<BusinessData> Update(BusinessData businessData, CancellationToken cancellationToken)
    {
        var data = await dbContext.BusinessDatas.FirstAsync(m => m.Id == businessData.Id, cancellationToken);

        data.Uid = businessData.Uid;
        data.Name = businessData.Name;
        data.Value1 = businessData.Value1;
        data.Value2 = businessData.Value2;
        data.Value3 = businessData.Value3;

        await dbContext.SaveChangesAsync(cancellationToken);

        return businessData;
    }


    [HttpPost]
    [PermissionRequired(PermissionDefine.SampleBusiness.WriteBusinessDataConstant)]
    [AuditDescription("Update BusinessData {id}.")]
    public async Task<bool> Delete(long id, CancellationToken cancellationToken)
    {
        var deleteCount = await dbContext.BusinessDatas.Where(m => m.Id == id).ExecuteDeleteAsync(cancellationToken);

        return deleteCount > 0;
    }
}
