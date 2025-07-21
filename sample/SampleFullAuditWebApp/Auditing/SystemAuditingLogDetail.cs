using System.Reflection;
using System.Text.Json;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace SampleFullAuditWebApp.Auditing;

internal class SystemAuditingLogDetail
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public object? AuditingDetail { get; }

    public object? Parameters { get; }

    public string? RequestUrl { get; }

    private SystemAuditingLogDetail(object? auditingDetail, object? parameters, string? requestUrl)
    {
        AuditingDetail = auditingDetail;
        Parameters = parameters;
        RequestUrl = requestUrl;
    }

    public static string? CreateJsonString(ActionAuditingExecutingContext context)
    {
        object? parameters = null;
        if (context.ActionArguments.Count > 0
            && context.HttpContext.GetEndpoint() is { } endpoint)
        {
            if (endpoint.Metadata.GetMetadata<ControllerActionDescriptor>() is { } actionDescriptor)  //Controller
            {
                var keys = actionDescriptor.Parameters.Where(m => m.BindingInfo?.BindingSource?.IsFromRequest == true).Select(m => m.Name).ToList();
                parameters = context.ActionArguments.Where(m => keys.Contains(m.Key)).ToDictionary(m => m.Key, m => m.Value);
            }
            else if (endpoint.Metadata.GetMetadata<MethodInfo>() is { } methodInfo
                     && methodInfo.GetParameters() is { Length: > 0 } parameterInfos
                     && context.HttpContext.RequestServices.GetService<IServiceProviderIsService>() is { } serviceProviderIsService)    //minimalAPI
            {
                var keys = parameterInfos.Where(m => !serviceProviderIsService.IsService(m.ParameterType)).Select(m => m.Name).ToList();
                parameters = context.ActionArguments.Where(m => keys.Contains(m.Key)).ToDictionary(m => m.Key, m => m.Value);
            }
        }

        return JsonSerializer.Serialize(new SystemAuditingLogDetail(context.AuditValueStoreAccessor?.Current, parameters, context.HttpContext.Request.GetDisplayUrl()), s_jsonSerializerOptions);
    }
}
