using System.Reflection;
using Cuture.AspNetCore.ActionAuditing;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SampleFullAuditWebApp.Auditing.EntityFramework;

/// <summary>
/// 可审计的 <see cref="DbContext"/>
/// </summary>
public abstract class AuditableDbContext<TDbContext>(DbContextOptions<TDbContext> options, IAuditValueStoreAccessor auditValueStoreAccessor) : DbContext(options)
    where TDbContext : DbContext
{
    public const string OperationLogFlowAuditValueStoreKey = "DbContextOperationLogFlow";

    /// <inheritdoc/>
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        if (ChangeTracker.HasChanges()
            && auditValueStoreAccessor.Initialize())
        {
            var auditValueStore = auditValueStoreAccessor.Current;
            var dbContextOperationLogFlow = auditValueStore.GetOrSet(OperationLogFlowAuditValueStoreKey, static () => new DbContextOperationLogFlow());

            var entries = ChangeTracker.Entries().Where(m => m.Entity.GetType().GetCustomAttribute<NoAuditingAttribute>() is null);

            dbContextOperationLogFlow.StartSaveChanges();
            try
            {
                dbContextOperationLogFlow.LogEntries(entries);
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            catch (Exception ex)
            {
                dbContextOperationLogFlow.Error(ex);
                throw;
            }
            finally
            {
                dbContextOperationLogFlow.SaveChangesEnd();
            }
        }
        else
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }

}
