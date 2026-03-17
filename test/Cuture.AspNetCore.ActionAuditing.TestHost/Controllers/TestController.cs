using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ActionAuditing.TestHost.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{
    #region Public 方法

    [HttpGet]
    [PermissionRequired]
    public string NoPermission1()
    {
        return Request.Path;
    }

    [HttpGet]
    [PermissionRequired("1Permission1")]
    public string Permission1()
    {
        return Request.Path;
    }

    [HttpGet]
    [PermissionRequired("1Permission1", "2Permission2")]
    public string Permission2()
    {
        return Request.Path;
    }

    [HttpGet]
    [PermissionRequired("1Permission1", "2Permission2", "3Permission3")]
    public string Permission3()
    {
        return Request.Path;
    }

    [HttpGet]
    [PermissionRequired("1Permission1", "2Permission2", "3Permission3", "4Permission4")]
    [PermissionRequired("1Permission1", "2Permission2", "3Permission3", "4Permission4")]
    public string Permission4()
    {
        return Request.Path;
    }

    #endregion Public 方法
}
