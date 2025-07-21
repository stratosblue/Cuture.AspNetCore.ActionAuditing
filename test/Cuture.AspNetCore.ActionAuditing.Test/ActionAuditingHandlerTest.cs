using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Cuture.AspNetCore.ActionAuditing.Test;

[TestClass]
public class ActionAuditingHandlerTest
{
    [TestMethod]
    public async Task HandleDeniedAsync_ShouldSet403AndCallOnDenied()
    {
        // Arrange
        var mockHandler = new Mock<ActionAuditingHandler>() { CallBase = true };
        var context = CreateActionAuditingExecutingContext();
        var permission = new PermissionDescriptor(["Test"]);

        // Act
        await mockHandler.Object.HandleDeniedAsync(context, default);

        // Assert
        Assert.AreEqual(StatusCodes.Status403Forbidden, context.HttpContext.Response.StatusCode);
        mockHandler.Verify(x => x.HandleDeniedAsync(context, default), Times.Once);
    }

    [TestMethod]
    public async Task HandleExceptionAsync_ShouldSet500AndCallOnException()
    {
        // Arrange
        var mockHandler = new Mock<ActionAuditingHandler>() { CallBase = true };
        var context = CreateActionAuditingExecutingContext();
        var permission = new PermissionDescriptor(["Test"]);
        var exception = new Exception("Test");

        // Act
        await mockHandler.Object.HandleExceptionAsync(context, default);

        // Assert
        Assert.AreEqual(StatusCodes.Status500InternalServerError, context.HttpContext.Response.StatusCode);
        mockHandler.Verify(x => x.HandleExceptionAsync(context, default), Times.Once);
    }

    [TestMethod]
    public async Task HandleSuccessAsync_ShouldCallOnSuccess()
    {
        // Arrange
        var mockHandler = new Mock<ActionAuditingHandler>() { CallBase = true };
        var context = CreateActionAuditingExecutingContext();
        var permission = new PermissionDescriptor(["Test"]);

        // Act
        await mockHandler.Object.HandleSuccessAsync(context, default);

        // Assert
        mockHandler.Verify(x => x.HandleSuccessAsync(context, default), Times.Once);
    }

    private static ActionAuditingExecutingContext CreateActionAuditingExecutingContext()
    {
        var httpContext = new DefaultHttpContext();

        return new ActionAuditingExecutingContext(httpContext,
                                                  default,
                                                  new DefaultActionArguments(new Dictionary<string, object?>()),
                                                  new TestAuditValueStoreAccessor());
    }

    private class TestAuditValueStoreAccessor : IAuditValueStoreAccessor
    {
        public IAuditValueStore? Current { get; set; }

        public bool Initialize() => false;
    }
}
