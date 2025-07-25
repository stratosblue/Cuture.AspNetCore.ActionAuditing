// 代码由 AI 自动生成

using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cuture.AspNetCore.ActionAuditing.Test;

[TestClass]
public class DefaultActionAuditingFilterTest
{
    #region Private 字段

    private readonly Mock<ILogger<DefaultActionAuditingFilter>> _loggerMock = new();

    #endregion Private 字段

    #region Public 方法

    [TestMethod]
    public async Task Should_Return_False_When_EndpointIsNull()
    {
        var filter = new DefaultActionAuditingFilter(_loggerMock.Object);
        var httpContext = new DefaultHttpContext();

        var result = await filter.PredicateAsync(httpContext, CancellationToken.None);

        Assert.IsFalse(result);
        _loggerMock.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("cannot get endpoint")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [TestMethod]
    public async Task Should_Return_False_When_HasNoAuditingAttribute()
    {
        var endpoint = CreateEndpoint(new NoAuditingAttribute());
        var result = await TestPredicateAsync(endpoint);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Should_Return_False_When_NoPermissionProvider()
    {
        var endpoint = CreateEndpoint();
        var result = await TestPredicateAsync(endpoint);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Should_Return_True_When_HasPermissionProvider()
    {
        var endpoint = CreateEndpoint(new Mock<IRequiredPermissionProvider>().Object);
        var result = await TestPredicateAsync(endpoint);

        Assert.IsTrue(result);
    }

    #endregion Public 方法

    #region Private 方法

    private static Endpoint CreateEndpoint(params object[] metadata)
    {
        return new Endpoint(
            _ => Task.CompletedTask,
            new EndpointMetadataCollection(metadata),
            "Test endpoint");
    }

    private static HttpContext CreateHttpContext(Endpoint endpoint)
    {
        var features = new FeatureCollection();
        features.Set<IEndpointFeature>(new EndpointFeature { Endpoint = endpoint });

        return new DefaultHttpContext(features);
    }

    private async Task<bool> TestPredicateAsync(Endpoint endpoint)
    {
        var filter = new DefaultActionAuditingFilter(_loggerMock.Object);
        var httpContext = CreateHttpContext(endpoint);

        return await filter.PredicateAsync(httpContext, CancellationToken.None);
    }

    #endregion Private 方法

    #region Private 类

    private class EndpointFeature : IEndpointFeature
    {
        #region Public 属性

        public Endpoint? Endpoint { get; set; }

        #endregion Public 属性
    }

    #endregion Private 类
}
