using Cuture.AspNetCore.ActionAuditing.Abstractions;

namespace Cuture.AspNetCore.ActionAuditing.Test;

[TestClass]
public class PermissionDescriptorTest
{
    #region Public 方法

    [TestMethod]
    public void Constructor_ShouldHandleEmptyPermissions()
    {
        // Arrange
        var permissions = Array.Empty<string>();

        // Act
        var descriptor = new PermissionDescriptor(permissions);

        // Assert
        Assert.IsEmpty(descriptor.Permissions);
        Assert.IsFalse(descriptor.IsDefined);
    }

    [TestMethod]
    public void Constructor_ShouldHandleEmptyProperties()
    {
        // Arrange
        var permissions = new[] { "Read" };
        var emptyProperties = new Dictionary<string, string?>();

        // Act
        var descriptor1 = new PermissionDescriptor(permissions, emptyProperties);
        var descriptor2 = new PermissionDescriptor(permissions, null);

        // Assert
        Assert.IsEmpty(descriptor1.Properties);
        Assert.IsEmpty(descriptor2.Properties);
    }

    [TestMethod]
    public void Constructor_ShouldHandleWithDefault()
    {
        // Act
        PermissionDescriptor descriptor = default;

        // Assert
        Assert.IsFalse(descriptor.IsDefined);
        Assert.IsTrue(descriptor.Permissions.IsDefaultOrEmpty);
        Assert.IsEmpty(descriptor.Properties);
    }

    [TestMethod]
    public void Constructor_ShouldInitializeWithPermissions()
    {
        // Arrange
        var permissions = new[] { "Read", "Write" };

        // Act
        var descriptor = new PermissionDescriptor(permissions);

        // Assert
        Assert.HasCount(2, descriptor.Permissions);
        Assert.Contains("Read", descriptor.Permissions);
        Assert.Contains("Write", descriptor.Permissions);
        Assert.IsTrue(descriptor.IsDefined);
    }

    [TestMethod]
    public void Constructor_ShouldInitializeWithProperties()
    {
        // Arrange
        var permissions = new[] { "Read" };
        var properties = new Dictionary<string, string?>
        {
            ["Prop1"] = "Value1",
            ["Prop2"] = null
        };

        // Act
        var descriptor = new PermissionDescriptor(permissions, properties);

        // Assert
        Assert.HasCount(2, descriptor.Properties);
        Assert.AreEqual("Value1", descriptor.Properties["Prop1"]);
        Assert.IsNull(descriptor.Properties["Prop2"]);
    }

    [TestMethod]
    public void Constructor_ShouldRemoveDuplicatePermissions()
    {
        // Arrange
        var permissions = new[] { "Read", "Write", "Read" };

        // Act
        var descriptor = new PermissionDescriptor(permissions);

        // Assert
        Assert.HasCount(2, descriptor.Permissions);
    }

    [TestMethod]
    public void ToString_ShouldHandleEmptyPermissions()
    {
        // Arrange
        var descriptor = new PermissionDescriptor([]);

        // Act
        var result = descriptor.ToString();

        // Assert
        Assert.AreEqual("[]", result);
    }

    [TestMethod]
    public void ToString_ShouldReturnCorrectFormat()
    {
        // Arrange
        var permissions = new[] { "Read", "Write" };
        var descriptor = new PermissionDescriptor(permissions);

        // Act
        var result = descriptor.ToString();

        // Assert
        Assert.AreEqual("[Read,Write]", result);
    }

    #endregion Public 方法
}
