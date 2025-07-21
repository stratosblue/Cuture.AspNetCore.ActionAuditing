using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ActionAuditing.Filters;

/// <summary>
/// 默认的 Controller 权限审核过滤器
/// </summary>
public class DefaultAuditingActionFilter(IActionRequiredPermissionResolver actionRequiredPermissionResolver,
                                         IExecutingPermissionAuditor executingPermissionAuditor,
                                         IActionAuditingHandler actionAuditingHandler,
                                         IActionAuditingFilter actionAuditingFilter)
    : IAsyncActionFilter, IOrderedFilter
{
    #region Public 字段

    /// <summary>
    /// 过滤器排序
    /// </summary>
    public const int FilterOrder = -1;

    #endregion Public 字段

    #region Public 属性

    /// <inheritdoc/>
    public int Order => FilterOrder;

    #endregion Public 属性

    #region Public 方法

    /// <inheritdoc/>
    public virtual async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext, ActionExecutionDelegate next)
    {
        var httpContext = actionExecutingContext.HttpContext;
        var cancellationToken = httpContext.RequestAborted;

        if (!await actionAuditingFilter.PredicateAsync(httpContext, cancellationToken))
        {
            await next();
            return;
        }

        var requestServices = httpContext.RequestServices;

        var actionAuditingExecutingContextAccessor = requestServices.GetRequiredService<IActionAuditingExecutingContextAccessor>();
        var auditValueStoreAccessor = requestServices.GetRequiredService<IAuditValueStoreAccessor>();
        var requiredPermission = await actionRequiredPermissionResolver.ResolveAsync(httpContext, cancellationToken);

        var actionArguments = new DefaultActionArguments(actionExecutingContext.ActionArguments);
        var context = new ActionAuditingExecutingContext(httpContext, requiredPermission, actionArguments, auditValueStoreAccessor)
        {
            ActionExecutingContext = actionExecutingContext,
        };
        actionAuditingExecutingContextAccessor.Current = context;

        try
        {
            if (!requiredPermission.IsDefined)
            {
                context.ExecutionFlag |= ActionExecutionFlag.NoPermissionAuditRequired;
                await next();
                return;
            }

            var hasPermission = await executingPermissionAuditor.AuditingAsync(context, cancellationToken);
            if (!hasPermission)
            {
                context.ExecutionFlag |= ActionExecutionFlag.PermissionAuditDenied;
                actionExecutingContext.Result ??= new StatusCodeResult(StatusCodes.Status403Forbidden);
                await actionAuditingHandler.HandleDeniedAsync(context, cancellationToken);
                return;
            }

            context.ExecutionFlag |= ActionExecutionFlag.PermissionAuditApproved;

            var actionExecutedContext = await next();
            context.ActionExecutingContext = actionExecutingContext;

            if (actionExecutedContext.Exception is not null)
            {
                context.Exception = actionExecutedContext.Exception;
                context.ExecutionFlag |= ActionExecutionFlag.Failed;
                await actionAuditingHandler.HandleExceptionAsync(context, cancellationToken);
            }
            else
            {
                context.ExecutionFlag |= ActionExecutionFlag.Success;
                context.Result = actionExecutedContext.Result ?? actionExecutingContext.Result;
                await actionAuditingHandler.HandleSuccessAsync(context, cancellationToken);
                actionExecutingContext.Result = context.Result as IActionResult;
            }
        }
        catch (Exception ex)
        {
            context.Exception = ex;
            context.ExecutionFlag |= ActionExecutionFlag.Failed;
            await actionAuditingHandler.HandleExceptionAsync(context, cancellationToken);
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
