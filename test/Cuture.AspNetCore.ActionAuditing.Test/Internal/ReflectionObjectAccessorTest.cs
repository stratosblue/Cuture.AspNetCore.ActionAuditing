using Cuture.AspNetCore.ActionAuditing.Internal;
using Cuture.AspNetCore.ActionAuditing.Test.TestBase;

namespace Cuture.AspNetCore.ActionAuditing.Test.Internal;

[TestClass]
public class ReflectionObjectAccessorTest
{
    [TestMethod]
    public void Constructor_ValidPath_InitializesCorrectly()
    {
        // Arrange
        var path = new VariablePropertyAccessPath("test", "test", []);

        // Act
        var accessor = new ReflectionObjectAccessor(path);

        // Assert
        Assert.AreEqual(path, accessor.PropertyAccessPath);
    }

    [TestMethod]
    public void Constructor_InvalidPath_ThrowsException()
    {
        // Arrange
        var invalidPath = new VariablePropertyAccessPath("", "", []);

        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => new ReflectionObjectAccessor(invalidPath));
    }

    [TestMethod]
    public void CreateFromExpression_ValidExpression_CreatesAccessor()
    {
        // Act
        var accessor = ReflectionObjectAccessor.CreateFromExpression("test.property");

        // Assert
        Assert.AreEqual("test", accessor.PropertyAccessPath.VariableName);
        CollectionAssert.AreEqual(new[] { "property" }, accessor.PropertyAccessPath.Paths.ToArray());
    }

    [TestMethod]
    public void Access_NoPaths_ReturnsSameObject()
    {
        // Arrange
        var path = new VariablePropertyAccessPath("test", "test", []);
        var accessor = new ReflectionObjectAccessor(path);
        var testObj = new object();

        // Act
        var result = accessor.Access(testObj);

        // Assert
        Assert.AreSame(testObj, result);
    }

    [TestMethod]
    public void Access_NullTarget_ReturnsNull()
    {
        // Arrange
        var path = new VariablePropertyAccessPath("test.property", "test", ["property"]);
        var accessor = new ReflectionObjectAccessor(path);

        // Act
        var result = accessor.Access(null);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Access_PropertyPath_ReturnsPropertyValue()
    {
        // Arrange
        var path = new VariablePropertyAccessPath("test.Property", "test", ["Property"]);
        var accessor = new ReflectionObjectAccessor(path);
        var testObj = new TestEntityClass { Property = "value" };

        // Act
        var result = accessor.Access(testObj);

        // Assert
        Assert.AreEqual("value", result);
    }

    [TestMethod]
    public void Access_FieldPath_ReturnsFieldValue()
    {
        // Arrange
        var path = new VariablePropertyAccessPath("test.Field", "test", ["Field"]);
        var accessor = new ReflectionObjectAccessor(path);
        var testObj = new TestEntityClass { Field = "fieldValue" };

        // Act
        var result = accessor.Access(testObj);

        // Assert
        Assert.AreEqual("fieldValue", result);
    }

    [TestMethod]
    public void Access_NestedPath_ReturnsNestedValue()
    {
        // Arrange
        var path = new VariablePropertyAccessPath("test.Nested.Property", "test", ["Nested", "Property"]);
        var accessor = new ReflectionObjectAccessor(path);
        var testObj = new TestEntityClass
        {
            Nested = new TestEntityClass { Property = "nestedValue" }
        };

        // Act
        var result = accessor.Access(testObj);

        // Assert
        Assert.AreEqual("nestedValue", result);
    }

    [TestMethod]
    public void Access_NonExistentProperty_ReturnsNull()
    {
        // Arrange
        var path = new VariablePropertyAccessPath("test.NonExistent", "test", ["NonExistent"]);
        var accessor = new ReflectionObjectAccessor(path);
        var testObj = new TestEntityClass();

        // Act
        var result = accessor.Access(testObj);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Access_NullInPath_ReturnsNull()
    {
        // Arrange
        var path = new VariablePropertyAccessPath("test.Nested.Property", "test", ["Nested", "Property"]);
        var accessor = new ReflectionObjectAccessor(path);
        var testObj = new TestEntityClass { Nested = null };

        // Act
        var result = accessor.Access(testObj);

        // Assert
        Assert.IsNull(result);
    }
}
