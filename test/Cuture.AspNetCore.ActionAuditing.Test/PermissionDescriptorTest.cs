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
        Assert.AreEqual(0, descriptor.Permissions.Length);
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
        Assert.AreEqual(0, descriptor1.Properties.Count);
        Assert.AreEqual(0, descriptor2.Properties.Count);
    }

    [TestMethod]
    public void Constructor_ShouldHandleWithDefault()
    {
        // Act
        PermissionDescriptor descriptor = default;

        // Assert
        Assert.IsFalse(descriptor.IsDefined);
        Assert.IsTrue(descriptor.Permissions.IsDefaultOrEmpty);
        Assert.AreEqual(0, descriptor.Properties.Count);
    }

    [TestMethod]
    public void Constructor_ShouldInitializeWithPermissions()
    {
        // Arrange
        var permissions = new[] { "Read", "Write" };

        // Act
        var descriptor = new PermissionDescriptor(permissions);

        // Assert
        Assert.AreEqual(2, descriptor.Permissions.Length);
        Assert.IsTrue(descriptor.Permissions.Contains("Read"));
        Assert.IsTrue(descriptor.Permissions.Contains("Write"));
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
        Assert.AreEqual(2, descriptor.Properties.Count);
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
        Assert.AreEqual(2, descriptor.Permissions.Length);
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
