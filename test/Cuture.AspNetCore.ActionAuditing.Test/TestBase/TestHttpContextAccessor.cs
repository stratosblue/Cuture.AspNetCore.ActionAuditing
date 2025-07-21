using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ActionAuditing.Test.TestBase;

public class TestHttpContextAccessor : IHttpContextAccessor
{
    #region Public 属性

    public HttpContext? HttpContext { get; set; }

    #endregion Public 属性
}
