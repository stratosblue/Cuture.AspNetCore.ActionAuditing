using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 默认的 <inheritdoc cref="IActionAuditingHandler"/>，仅包含基本的逻辑
/// </summary>
public class DefaultActionAuditingHandler(ILogger<DefaultActionAuditingHandler> logger)
    : ActionAuditingHandler
{
    #region Protected 方法

    /// <inheritdoc/>
    public override ValueTask HandleDeniedAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken)
    {
        logger.LogWarning("Unable to pass the audit and refuse to execute the action: {Description}", GetAuditDescription(context));
        return base.HandleDeniedAsync(context, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask HandleExceptionAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken)
    {
        logger.LogWarning("Audit approved but failed to execute action: {Description}", GetAuditDescription(context));
        return base.HandleExceptionAsync(context, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask HandleSuccessAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Audit approved and successfully executed action: {Description}", GetAuditDescription(context));
        return base.HandleSuccessAsync(context, cancellationToken);
    }

    /// <inheritdoc/>
    protected internal virtual ActionAuditDescription GetAuditDescription(ActionAuditingExecutingContext context)
    {
        return context.AuditDescription ??= new(Format: "Access Path: {context.HttpContext.Request.Path}", Description: $"Access Path: {context.HttpContext.Request.Path}");
    }

    #endregion Protected 方法
}
