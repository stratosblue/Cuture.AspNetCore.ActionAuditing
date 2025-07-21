namespace Cuture.AspNetCore.ActionAuditing.Test.Attributes;

[TestClass]
public class PermissionRequiredAttributeTest
{
    #region Public 方法

    [TestMethod]
    public void AttributeUsage_ShouldAllowMultiple()
    {
        // Arrange
        var attribute = new PermissionRequiredAttribute("Test");

        // Act
        var usageAttribute = Attribute.GetCustomAttribute(attribute.GetType(), typeof(AttributeUsageAttribute));

        // Assert
        Assert.IsNotNull(usageAttribute);
        Assert.IsTrue(usageAttribute is AttributeUsageAttribute);
        var usage = (AttributeUsageAttribute)usageAttribute;
        Assert.IsTrue(usage.AllowMultiple);
        Assert.IsTrue(usage.Inherited);
        Assert.AreEqual(AttributeTargets.Class | AttributeTargets.Method, usage.ValidOn);
    }

    [TestMethod]
    public void Constructor_ShouldHandleSinglePermission()
    {
        // Arrange
        var permission = "Admin";

        // Act
        var attribute = new PermissionRequiredAttribute(permission);

        // Assert
        Assert.AreEqual(1, attribute.Permissions.Length);
        Assert.AreEqual(permission, attribute.Permissions[0]);
    }

    [TestMethod]
    public void Constructor_ShouldInitializeWithPermissions()
    {
        // Arrange
        var permissions = new[] { "Read", "Write" };

        // Act
        var attribute = new PermissionRequiredAttribute(permissions);

        // Assert
        Assert.AreEqual(2, attribute.Permissions.Length);
        Assert.AreEqual("Read", attribute.Permissions[0]);
        Assert.AreEqual("Write", attribute.Permissions[1]);
    }

    [TestMethod]
    public void Constructor_ShouldThrow_WhenPermissionsIsEmpty()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => new PermissionRequiredAttribute([]));
    }

    [TestMethod]
    public void Constructor_ShouldThrow_WhenPermissionsIsNull()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => new PermissionRequiredAttribute(null!));
    }

    #endregion Public 方法
}
