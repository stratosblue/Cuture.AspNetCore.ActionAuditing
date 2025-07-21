using Cuture.AspNetCore.ActionAuditing.Internal;

namespace Cuture.AspNetCore.ActionAuditing.Test.Internal;

[TestClass]
public class VariablePropertyAccessPathTest
{
    [TestMethod]
    public void Parse_ValidExpression_ReturnsCorrectPath()
    {
        // Arrange
        var expression = "user.address.street";

        // Act
        var result = VariablePropertyAccessPath.Parse(expression);

        // Assert
        Assert.AreEqual("user", result.VariableName);
        CollectionAssert.AreEqual(new[] { "address", "street" }, result.Paths.ToArray());
    }

    [TestMethod]
    public void Parse_ExpressionWithSpaces_TrimsCorrectly()
    {
        // Arrange
        var expression = " user . address . street ";

        // Act
        var result = VariablePropertyAccessPath.Parse(expression);

        // Assert
        Assert.AreEqual("user", result.VariableName);
        CollectionAssert.AreEqual(new[] { "address", "street" }, result.Paths.ToArray());
    }

    [TestMethod]
    public void Parse_SingleVariable_ReturnsEmptyPaths()
    {
        // Arrange
        var expression = "user";

        // Act
        var result = VariablePropertyAccessPath.Parse(expression);

        // Assert
        Assert.AreEqual("user", result.VariableName);
        Assert.AreEqual(0, result.Paths.Length);
    }

    [TestMethod]
    public void Parse_EmptyExpression_ThrowsArgumentException()
    {
        // Arrange
        var expression = "";

        // Act
        Assert.ThrowsExactly<ArgumentException>(() => VariablePropertyAccessPath.Parse(expression));
    }

    [TestMethod]
    public void Parse_WhitespaceExpression_ThrowsArgumentException()
    {
        // Arrange
        var expression = "   ";

        // Act
        Assert.ThrowsExactly<ArgumentException>(() => VariablePropertyAccessPath.Parse(expression));
    }

    [TestMethod]
    public void Parse_NullExpression_ThrowsArgumentException()
    {
        // Act
        Assert.ThrowsExactly<ArgumentNullException>(() => VariablePropertyAccessPath.Parse(null!));
    }

    [TestMethod]
    public void ThrowIfInvalid_ValidVariableName_DoesNotThrow()
    {
        // Arrange
        var path = new VariablePropertyAccessPath("valid", "valid", []);

        // Act & Assert (should not throw)
        path.ThrowIfInvalid();
    }

    [TestMethod]
    public void ThrowIfInvalid_EmptyVariableName_ThrowsInvalidOperationException()
    {
        // Arrange
        var path = new VariablePropertyAccessPath("", "", []);

        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => path.ThrowIfInvalid());
    }

    [TestMethod]
    public void ThrowIfInvalid_WhitespaceVariableName_ThrowsInvalidOperationException()
    {
        // Arrange
        var path = new VariablePropertyAccessPath("   ", "   ", []);

        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => path.ThrowIfInvalid());
    }
}
