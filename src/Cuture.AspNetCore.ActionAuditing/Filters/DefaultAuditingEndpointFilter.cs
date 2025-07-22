using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ActionAuditing.Filters;

/// <summary>
/// 默认的 MinimalAPI 权限审核过滤器
/// </summary>
public class DefaultAuditingEndpointFilter(IActionRequiredPermissionResolver actionRequiredPermissionResolver,
                                           IExecutingPermissionAuditor executingPermissionAuditor,
                                           IActionAuditingHandler actionAuditingHandler,
                                           IActionAuditingFilter actionAuditingFilter,
                                           IReadOnlyDictionary<string, int> argumentNameMap)
    : IEndpointFilter
{
    #region Public 方法

    /// <inheritdoc/>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext invocationContext, EndpointFilterDelegate next)
    {
        var httpContext = invocationContext.HttpContext;
        var cancellationToken = httpContext.RequestAborted;

        if (!await actionAuditingFilter.PredicateAsync(httpContext, cancellationToken))
        {
            return await next(invocationContext);
        }

        var requestServices = httpContext.RequestServices;

        var actionAuditingExecutingContextAccessor = requestServices.GetRequiredService<IActionAuditingExecutingContextAccessor>();
        var auditValueStoreAccessor = requestServices.GetRequiredService<IAuditValueStoreAccessor>();
        var requiredPermission = await actionRequiredPermissionResolver.ResolveAsync(httpContext, cancellationToken);

        var actionArguments = new DefaultEndpointActionArguments(argumentNameMap, invocationContext.Arguments);
        var context = new ActionAuditingExecutingContext(httpContext, requiredPermission, actionArguments, auditValueStoreAccessor)
        {
        };
        actionAuditingExecutingContextAccessor.Current = context;

        try
        {
            if (!requiredPermission.IsDefined)
            {
                context.ExecutionFlag |= ActionExecutionFlag.NoPermissionAuditRequired;
                return await next(invocationContext);
            }

            var permissionAuditResult = await executingPermissionAuditor.AuditingAsync(context, cancellationToken);
            context.PermissionAuditResult = permissionAuditResult;
            if (!permissionAuditResult)
            {
                context.ExecutionFlag |= ActionExecutionFlag.PermissionAuditDenied;
                await actionAuditingHandler.HandleDeniedAsync(context, cancellationToken);
                return context.Result ?? Results.Problem(statusCode: StatusCodes.Status403Forbidden);
            }

            context.ExecutionFlag |= ActionExecutionFlag.PermissionAuditApproved;

            object? result;
            try
            {
                result = await next(invocationContext);
            }
            catch (Exception ex)
            {
                context.Exception = ex;
                context.ExecutionFlag |= ActionExecutionFlag.Failed;
                await actionAuditingHandler.HandleExceptionAsync(context, cancellationToken);
                return context.Result ?? Results.Problem();
            }
            context.ExecutionFlag |= ActionExecutionFlag.Success;
            context.Result = result;
            await actionAuditingHandler.HandleSuccessAsync(context, cancellationToken);
            return context.Result;
        }
        catch (Exception ex)
        {
            context.Exception = ex;
            context.ExecutionFlag |= ActionExecutionFlag.Failed;
            await actionAuditingHandler.HandleExceptionAsync(context, cancellationToken);
            return Results.Problem();
        }
        finally
        {
            await AuditActionExecutionCompletedAsync(context, cancellationToken);
            actionAuditingExecutingContextAccessor.Current = null;
        }
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 已审计的动作执行完成
    /// </summary>
    /// <param name="context">上下文 (在当前异步方法返回后不能再持有其引用)</param>
    /// <param name="cancellationToken">当此请求的取消令牌</param>
    /// <returns></returns>
    protected virtual Task AuditActionExecutionCompletedAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken)
    {
        if (context.HttpContext.RequestServices.GetService<IAuditDataStorage>() is not { } auditDataStorage)
        {
            return Task.CompletedTask;
        }

        return auditDataStorage.AddAsync(context, cancellationToken);
    }

    #endregion Protected 方法
}
