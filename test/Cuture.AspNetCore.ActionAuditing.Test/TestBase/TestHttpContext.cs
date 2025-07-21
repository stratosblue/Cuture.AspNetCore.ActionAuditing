#pragma warning disable CS8618

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Cuture.AspNetCore.ActionAuditing.Test.TestBase;

internal class TestHttpContext : HttpContext
{
    #region Public 属性

    public override ConnectionInfo Connection { get; }

    public override IFeatureCollection Features { get; } = new FeatureCollection();

    public override IDictionary<object, object?> Items { get; set; } = new Dictionary<object, object?>();

    public override HttpRequest Request { get; }

    public override CancellationToken RequestAborted { get; set; }

    public override IServiceProvider RequestServices { get; set; }

    public override HttpResponse Response { get; }

    public override ISession Session { get; set; }

    public override string TraceIdentifier { get; set; }

    public override ClaimsPrincipal User { get; set; }

    public override WebSocketManager WebSockets { get; }

    #endregion Public 属性

    #region Public 方法

    public override void Abort()
    {
    }

    #endregion Public 方法
}
