// 代码由 AI 自动生成

using Cuture.AspNetCore.ActionAuditing.Internal;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Cuture.AspNetCore.ActionAuditing.Test.Internal;

[TestClass]
public class HttpContextItemsAccessorTest
{
    #region Private 字段

    private const string TestKey = "TestKey";

    private Mock<IHttpContextAccessor> _contextAccessorMock = null!;

    #endregion Private 字段

    #region Private 接口

    private interface ITestInterface
    { }

    #endregion Private 接口

    #region Public 方法

    [TestInitialize]
    public void Setup()
    {
        _contextAccessorMock = new Mock<IHttpContextAccessor>();
    }

    [TestMethod]
    public void Should_Return_Null_When_HttpContext_Not_Exists()
    {
        _contextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null!);

        var accessor = new TestAccessor(_contextAccessorMock.Object, TestKey);

        Assert.IsNull(accessor.Current);
    }

    [TestMethod]
    public void Should_Return_Null_When_Item_Not_Exists()
    {
        var httpContext = new DefaultHttpContext();
        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var accessor = new TestAccessor(_contextAccessorMock.Object, TestKey);

        Assert.IsNull(accessor.Current);
    }

    [TestMethod]
    public void Should_Return_Value_When_Item_Exists()
    {
        var expectedValue = new TestImplementation();
        var httpContext = new DefaultHttpContext();
        httpContext.Items[TestKey] = expectedValue;
        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var accessor = new TestAccessor(_contextAccessorMock.Object, TestKey);

        Assert.AreEqual(expectedValue, accessor.Current);
    }

    [TestMethod]
    public void Should_Set_Value_Success()
    {
        var httpContext = new DefaultHttpContext();
        _contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        var expectedValue = new TestImplementation();

        var accessor = new TestAccessor(_contextAccessorMock.Object, TestKey)
        {
            Current = expectedValue
        };

        Assert.AreEqual(expectedValue, httpContext.Items[TestKey]);
    }

    [TestMethod]
    public void Should_Throw_When_Set_Value_Without_HttpContext()
    {
        _contextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null!);

        var accessor = new TestAccessor(_contextAccessorMock.Object, TestKey);

        Assert.ThrowsExactly<InvalidOperationException>(() => accessor.Current = new TestImplementation());
    }

    #endregion Public 方法

    #region Private 类

    private class TestAccessor(IHttpContextAccessor contextAccessor, string httpContextItemsKey)
        : HttpContextItemsAccessor<ITestInterface, TestImplementation>(contextAccessor, httpContextItemsKey)
    {
    }

    private class TestImplementation : ITestInterface
    { }

    #endregion Private 类
}
