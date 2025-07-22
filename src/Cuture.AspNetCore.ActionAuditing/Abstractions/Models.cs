using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ActionAuditing.Abstractions;

/// <summary>
/// 动作审计执行上下文
/// </summary>
/// <param name="HttpContext">请求上下文</param>
/// <param name="RequiredPermission">需求的权限</param>
/// <param name="ActionArguments">动作参数</param>
/// <param name="AuditValueStoreAccessor">审计值存储访问器</param>
public record ActionAuditingExecutingContext(HttpContext HttpContext,
                                             PermissionDescriptor RequiredPermission,
                                             IActionArguments ActionArguments,
                                             IAuditValueStoreAccessor AuditValueStoreAccessor)
{
    private ValueTuple<ActionAuditDescription?>? _auditDescription;

    /// <summary>
    /// 动作执行上下文(Controller Only)
    /// </summary>
    public ActionExecutingContext? ActionExecutingContext { get; set; }

    /// <summary>
    /// 动作执行后上下文(Controller Only)
    /// </summary>
    public ActionExecutedContext? ActionExecutedContext { get; set; }

    /// <summary>
    /// 审计描述
    /// </summary>
    public ActionAuditDescription? AuditDescription
    {
        get
        {
            if (!_auditDescription.HasValue)
            {
                if (HttpContext.GetEndpoint() is { } endpoint
                    && endpoint.Metadata.OfType<IActionDescriptionMetadata>().FirstOrDefault() is { } actionDescriptionMetadata)
                {
                    _auditDescription = new(ActionAuditDescriptionFormatter.Format(actionDescriptionMetadata.Description, AuditValueStoreAccessor.Current, ActionArguments));
                }
                else
                {
                    _auditDescription = new(null);
                }
            }
            return _auditDescription.Value.Item1;
        }
        set => _auditDescription = new(value);
    }

    /// <summary>
    /// 执行标识
    /// </summary>
    public ActionExecutionFlag ExecutionFlag { get; set; }

    /// <summary>
    /// 执行出现的异常
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// 执行权限审核结果
    /// </summary>
    public PermissionAuditResult? PermissionAuditResult { get; set; }

    /// <summary>
    /// 动作执行的结果
    /// </summary>
    public object? Result { get; set; }
}
