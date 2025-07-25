// 代码由 AI 自动生成

using Cuture.AspNetCore.ActionAuditing.Internal;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Cuture.AspNetCore.ActionAuditing.Test.Internal;

[TestClass]
public class HttpContextAuditValueStoreAccessorTest
{
    private Mock<IHttpContextAccessor> _contextAccessorMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _contextAccessorMock = new Mock<IHttpContextAccessor>();
    }

    [TestMethod]
    public void Should_Initialize_Return_True_When_Already_Initialized()
    {
        var httpContext = new DefaultHttpContext();
        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        var accessor = new HttpContextAuditValueStoreAccessor(_contextAccessorMock.Object);

        accessor.Initialize();
        var result = accessor.Initialize();

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Should_Initialize_Return_False_When_HttpContext_Not_Exists()
    {
        _contextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null!);
        var accessor = new HttpContextAuditValueStoreAccessor(_contextAccessorMock.Object);

        var result = accessor.Initialize();

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Should_Initialize_Success_And_Set_Current()
    {
        var httpContext = new DefaultHttpContext();
        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        var accessor = new HttpContextAuditValueStoreAccessor(_contextAccessorMock.Object);

        var result = accessor.Initialize();

        Assert.IsTrue(result);
        Assert.IsNotNull(accessor.Current);
    }
}
