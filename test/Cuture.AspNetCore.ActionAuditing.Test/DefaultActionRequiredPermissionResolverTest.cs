using System.Collections.Immutable;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Cuture.AspNetCore.ActionAuditing.Test.TestBase;

namespace Cuture.AspNetCore.ActionAuditing.Test;

[TestClass]
public class DefaultActionRequiredPermissionResolverTests
{
    #region Public 方法

    [TestMethod]
    public async Task ResolveAsync_ShouldCombinePermissions_FromMultipleAttributes()
    {
        // Arrange
        var resolver = new DefaultActionRequiredPermissionResolver();
        var attribute1 = new PermissionRequiredAttribute("Read");
        var attribute2 = new PermissionRequiredAttribute("Write");

        var actionExecutingContext = HttpContextHelper.CreateActionExecutingContext(attribute1, attribute2);

        // Act
        var result = await resolver.ResolveAsync(actionExecutingContext.HttpContext);

        // Assert
        Assert.AreEqual(2, result.Permissions.Length);
        Assert.IsTrue(result.Permissions.Contains("Read"));
        Assert.IsTrue(result.Permissions.Contains("Write"));
    }

    [TestMethod]
    public async Task ResolveAsync_ShouldHandleDuplicatePermissions()
    {
        // Arrange
        var resolver = new DefaultActionRequiredPermissionResolver();
        var attribute1 = new PermissionRequiredAttribute("Read", "Write");
        var attribute2 = new PermissionRequiredAttribute("Write", "Execute");

        var actionExecutingContext = HttpContextHelper.CreateActionExecutingContext(attribute1, attribute2);

        // Act
        var result = await resolver.ResolveAsync(actionExecutingContext.HttpContext);

        // Assert
        Assert.AreEqual(3, result.Permissions.Length);
        Assert.AreEqual(1, result.Permissions.Count(p => p == "Read"));
        Assert.AreEqual(1, result.Permissions.Count(p => p == "Write"));
        Assert.AreEqual(1, result.Permissions.Count(p => p == "Execute"));
    }

    [TestMethod]
    public async Task ResolveAsync_ShouldHandleMixedProviders()
    {
        // Arrange
        var resolver = new DefaultActionRequiredPermissionResolver();
        var attribute = new PermissionRequiredAttribute("Read");
        var customPermissionProvider = new CustomPermissionProvider("Write");

        var actionExecutingContext = HttpContextHelper.CreateActionExecutingContext(attribute, customPermissionProvider);

        // Act
        var result = await resolver.ResolveAsync(actionExecutingContext.HttpContext);

        // Assert
        Assert.AreEqual(2, result.Permissions.Length);
        Assert.IsTrue(result.Permissions.Contains("Read"));
        Assert.IsTrue(result.Permissions.Contains("Write"));
    }

    [TestMethod]
    public async Task ResolveAsync_ShouldReturnEmptyPermissions_WhenNoProviders()
    {
        // Arrange
        var resolver = new DefaultActionRequiredPermissionResolver();
        var actionExecutingContext = HttpContextHelper.CreateActionExecutingContext();

        // Act
        var result = await resolver.ResolveAsync(actionExecutingContext.HttpContext);

        // Assert
        Assert.AreEqual(0, result.Permissions.Length);
    }

    [TestMethod]
    public async Task ResolveAsync_ShouldReturnPermissionDescriptor_WithPermissionRequiredAttribute()
    {
        // Arrange
        var resolver = new DefaultActionRequiredPermissionResolver();
        var permissions = new[] { "Read", "Write" };
        var attribute = new PermissionRequiredAttribute(permissions);

        var actionExecutingContext = HttpContextHelper.CreateActionExecutingContext(attribute);

        // Act
        var result = await resolver.ResolveAsync(actionExecutingContext.HttpContext);

        // Assert
        CollectionAssert.AreEquivalent(permissions, result.Permissions.ToList());
    }

    #endregion Public 方法

    #region Private 类

    private class CustomPermissionProvider(params string[] permissions) : IRequiredPermissionProvider
    {
        #region Public 属性

        public ImmutableArray<string> Permissions { get; } = permissions.ToImmutableArray();

        #endregion Public 属性
    }

    #endregion Private 类
}
