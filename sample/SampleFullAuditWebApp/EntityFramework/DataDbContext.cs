using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cuture.AspNetCore.ActionAuditing;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.EntityFrameworkCore;
using SampleFullAuditWebApp.Auditing.EntityFramework;

namespace SampleFullAuditWebApp.EntityFramework;

public class DataDbContext(DbContextOptions<DataDbContext> options, IAuditValueStoreAccessor auditValueStoreAccessor)
    : AuditableDbContext<DataDbContext>(options, auditValueStoreAccessor)
{
    public virtual DbSet<BusinessData> BusinessDatas { get; set; }

    public virtual DbSet<SystemAuditingLog> SystemAuditingLogs { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }
}

[Index(nameof(Uid))]
public record class UserPermission([property: Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] long Id, long Uid, string Permission);

[NoAuditing]
public record class SystemAuditingLog([property: Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] long Id,
                                      long Uid,
                                      string UserName,
                                      string FeatureName,
                                      string Description,
                                      string? Detail,
                                      string Status,
                                      string IpAddress,
                                      DateTime OperateAt);

[Index(nameof(Uid))]
public class BusinessData
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long Uid { get; set; }

    public string? Name { get; set; }

    public int Value1 { get; set; }

    public string? Value2 { get; set; }

    public double Value3 { get; set; }
}
