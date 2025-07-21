using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 基础的 <inheritdoc cref="IActionAuditingHandler"/>
/// </summary>
public abstract class ActionAuditingHandler : IActionAuditingHandler
{
    #region Public 方法

    /// <inheritdoc cref="HandleDeniedAsync(ActionAuditingExecutingContext, CancellationToken)"/>
    public virtual ValueTask HandleDeniedAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc cref="HandleExceptionAsync(ActionAuditingExecutingContext,  CancellationToken)"/>
    public virtual ValueTask HandleExceptionAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc cref="HandleSuccessAsync(ActionAuditingExecutingContext, CancellationToken)"/>
    public virtual ValueTask HandleSuccessAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    #endregion Public 方法
}
