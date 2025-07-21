using Cuture.AspNetCore.ActionAuditing;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using SampleFullAuditWebApp.EntityFramework;

namespace SampleFullAuditWebApp.Auditing;

public class SystemAuditDataStorage(IServiceScopeFactory serviceScopeFactory, ILogger<SystemAuditDataStorage> logger)
    : AsyncAuditDataStorage<SystemAuditingLog>(logger)
{
    private readonly AsyncServiceScope _serviceScope = serviceScopeFactory.CreateAsyncScope();

    protected override ValueTask<SystemAuditingLog> CreateDataAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken)
    {
        //TODO get info from context
        const long UserId = 1234;
        var log = new SystemAuditingLog(Id: 0,
                                        Uid: UserId,
                                        UserName: "SampleUserName",
                                        FeatureName: context.HttpContext.GetEndpoint()?.Metadata.OfType<FeatureNameAttribute>()?.SingleOrDefault()?.FeatureName ?? "Unknown",
                                        Description: context.AuditDescription?.Description ?? $"Access Path: {context.HttpContext.Request.Path}",
                                        Detail: SystemAuditingLogDetail.CreateJsonString(context),
                                        Status: context.ExecutionFlag.ToString("G"),
                                        IpAddress: context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                                        OperateAt: DateTime.UtcNow);
        return ValueTask.FromResult(log);
    }

    protected override async Task SaveDataAsync(SystemAuditingLog data, CancellationToken cancellationToken)
    {
        const int RetryCount = 5;

        for (int i = 0; i < RetryCount; i++)
        {
            try
            {
                using var dataDbContext = _serviceScope.ServiceProvider.GetRequiredService<DataDbContext>();
                await dataDbContext.SystemAuditingLogs.AddAsync(data, cancellationToken);
                await dataDbContext.SaveChangesAsync(cancellationToken);
                break;
            }
            catch (TaskCanceledException tce) when (tce.CancellationToken == cancellationToken) { throw; }
            catch (Exception ex)
            {
                Logger.LogError(ex, "SystemAuditingLog save failed: {Data}", data);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _serviceScope.Dispose();
    }
}
