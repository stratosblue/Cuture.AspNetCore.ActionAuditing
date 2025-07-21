using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Moq;

namespace Cuture.AspNetCore.ActionAuditing.Test;

/// <summary>
/// <see cref="ActionAuditDescriptionFormatter"/> 的单元测试
/// </summary>
[TestClass]
public class ActionAuditDescriptionFormatterTest
{
    /// <summary>
    /// 测试当format包含多个插值表达式时正确处理所有表达式
    /// </summary>
    [TestMethod]
    public void Format_ShouldHandleMultipleInterpolations()
    {
        // Arrange
        const string format = "User {user} has {count} items";
        const string userValue = "testUser";
        const int countValue = 5;

        var mockValueStore = new Mock<IAuditValueStore>();
        mockValueStore.Setup(x => x.TryGetValue("user", out It.Ref<object?>.IsAny))
                     .Returns(new TryGetValueCallback((string key, out object value) =>
                     {
                         value = userValue;
                         return true;
                     }));
        mockValueStore.Setup(x => x.TryGetValue("count", out It.Ref<object?>.IsAny))
                     .Returns(new TryGetValueCallback((string key, out object value) =>
                     {
                         value = countValue;
                         return true;
                     }));

        // Act
        var result = ActionAuditDescriptionFormatter.Format(format, mockValueStore.Object, null);

        // Assert
        Assert.AreEqual($"User {userValue} has {countValue} items", result.Description);
    }

    [TestMethod]
    public void Format_ShouldReturnError_WhenInterpolationFails()
    {
        // Arrange
        const string format = "Invalid {obj.PropertyThatThrows}";
        var throwingObj = new ThrowingPropertyAccessor();

        var mockValueStore = new Mock<IAuditValueStore>();
        mockValueStore.Setup(x => x.TryGetValue("obj", out It.Ref<object?>.IsAny))
                     .Returns(new TryGetValueCallback((string key, out object value) =>
                     {
                         value = throwingObj;
                         return true;
                     }));

        // Act
        var result = ActionAuditDescriptionFormatter.Format(format, mockValueStore.Object, null);

        // Assert
        Assert.IsTrue(result.Description.Contains("[Error]"));
    }

    /// <summary>
    /// 测试当format包含插值表达式且valueStore有值时返回正确格式化结果
    /// </summary>
    [TestMethod]
    public void Format_ShouldReturnFormattedValue_WhenValueStoreHasValue()
    {
        // Arrange
        const string format = "Value is {value}";
        const string expectedValue = "testValue";

        var mockValueStore = new Mock<IAuditValueStore>();
        mockValueStore.Setup(x => x.TryGetValue("value", out It.Ref<object?>.IsAny))
                     .Returns(new TryGetValueCallback((string key, out object value) =>
                     {
                         value = expectedValue;
                         return true;
                     }));

        // Act
        var result = ActionAuditDescriptionFormatter.Format(format, mockValueStore.Object, null);

        // Assert
        Assert.AreEqual($"Value is {expectedValue}", result.Description);
    }

    /// <summary>
    /// 测试当format包含插值表达式但valueStore为null时返回null值
    /// </summary>
    [TestMethod]
    public void Format_ShouldReturnNullValue_WhenValueStoreIsNull()
    {
        // Arrange
        const string format = "Value is {value}";

        // Act
        var result = ActionAuditDescriptionFormatter.Format(format, null, null);

        // Assert
        Assert.AreEqual("Value is null", result.Description);
    }

    /// <summary>
    /// 测试当format不包含插值表达式时直接返回原始格式
    /// </summary>
    [TestMethod]
    public void Format_ShouldReturnOriginalFormat_WhenNoInterpolation()
    {
        // Arrange
        const string format = "Plain text format";

        // Act
        var result = ActionAuditDescriptionFormatter.Format(format, null, null);

        // Assert
        Assert.AreEqual(format, result.Description);
        Assert.AreEqual(format, result.Format);
    }

    /// <summary>
    /// 测试当format为null时抛出ArgumentNullException
    /// </summary>
    [TestMethod]
    public void Format_ShouldThrowArgumentNullException_WhenFormatIsNull()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => ActionAuditDescriptionFormatter.Format(null!, null, null));
    }

    /// <summary>
    /// 测试缓存机制是否正常工作
    /// </summary>
    [TestMethod]
    public void Format_ShouldUseCache_ForSameFormatString()
    {
        // Arrange
        const string format = "Cached format {value}";
        var mockValueStore = new Mock<IAuditValueStore>();

        // Act - First call should create cache
        var firstResult = ActionAuditDescriptionFormatter.Format(format, mockValueStore.Object, null);

        // Second call should use cache
        var secondResult = ActionAuditDescriptionFormatter.Format(format, mockValueStore.Object, null);

        // Assert
        Assert.AreEqual(firstResult.Description, secondResult.Description);
    }

    private class ThrowingPropertyAccessor
    {
        public object PropertyThatThrows
        {
            get { throw new InvalidOperationException("Property access failed"); }
        }
    }
}

/// <summary>
/// 用于模拟TryGetValue回调的委托
/// </summary>
internal delegate bool TryGetValueCallback(string key, out object value);
