using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ActionAuditing.Filters;

internal class DefaultAuditingEndpointFilterFactory
{
    #region Public 方法

    public static EndpointFilterDelegate CreateEndpointFilterDelegate(EndpointFilterFactoryContext filterFactoryContext, EndpointFilterDelegate next)
    {
        var argumentNameMap = DefaultEndpointActionArguments.CreateArgumentNameMap(filterFactoryContext);

        return invocationContext =>
        {
            var auditingEndpointFilter = ActivatorUtilities.CreateInstance<DefaultAuditingEndpointFilter>(invocationContext.HttpContext.RequestServices, argumentNameMap);

            return auditingEndpointFilter.InvokeAsync(invocationContext, next);
        };
    }

    #endregion Public 方法
}
