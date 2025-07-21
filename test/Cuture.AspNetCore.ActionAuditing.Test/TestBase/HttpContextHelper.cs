using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;

namespace Cuture.AspNetCore.ActionAuditing.Test.TestBase;

internal class HttpContextHelper
{
    #region Public 方法

    public static ActionContext CreateActionContext(params object[] attributes)
    {
        var actionDescriptor = new ActionDescriptor
        {
            EndpointMetadata = [.. attributes],
        };

        var context = new TestHttpContext();

        var endpoint = new Endpoint(null, new([.. attributes]), null);

        var endpointFeature = new Mock<IEndpointFeature>();
        endpointFeature.Setup(m => m.Endpoint).Returns(endpoint);

        context.Features.Set<IEndpointFeature>(endpointFeature.Object);

        var actionContext = new ActionContext
        {
            ActionDescriptor = actionDescriptor,
            HttpContext = context,
            RouteData = new(),
        };
        return actionContext;
    }

    public static ActionExecutingContext CreateActionExecutingContext(ActionContext actionContext)
    {
        return new ActionExecutingContext(
            actionContext,
            [],
            new Dictionary<string, object?>(),
            new object())
        {
            HttpContext = actionContext.HttpContext ?? new TestHttpContext()
        };
    }

    public static ActionExecutingContext CreateActionExecutingContext(params object[] attributes)
    {
        return CreateActionExecutingContext(CreateActionContext(attributes));
    }

    #endregion Public 方法
}
