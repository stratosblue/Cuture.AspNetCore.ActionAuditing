using Cuture.AspNetCore.ActionAuditing.Test.TestBase;

namespace Cuture.AspNetCore.ActionAuditing.Test;

[TestClass]
public class DefaultAuditValueStoreTest
{
    #region Public 方法

    [TestMethod]
    public void Constructor_ShouldInitializeEmptyStore()
    {
        // Act
        var store = new DefaultAuditValueStore();

        // Assert
        Assert.IsEmpty(store);
    }

    [TestMethod]
    public void Constructor_WithCollection_ShouldInitializeWithItems()
    {
        // Arrange
        var items = new Dictionary<string, object?>
        {
            ["key1"] = "value1",
            ["key2"] = 123
        };

        // Act
        var store = new DefaultAuditValueStore(items);

        // Assert
        Assert.HasCount(2, store);
        Assert.AreEqual("value1", store["key1"]);
        Assert.AreEqual(123, store["key2"]);
    }

    [TestMethod]
    public void Keys_ShouldBeCaseSensitive()
    {
        // Arrange
        var store = new DefaultAuditValueStore();

        // Act
        store.Set("TestKey", "value1");
        store.Set("testkey", "value2");

        // Assert
        Assert.HasCount(2, store);
        Assert.AreEqual("value1", store["TestKey"]);
        Assert.AreEqual("value2", store["testkey"]);
    }

    [TestMethod]
    public void Set_ShouldAddOrUpdateValue()
    {
        // Arrange
        var store = new DefaultAuditValueStore();

        // Act
        store.Set("testKey", "testValue");

        // Assert
        Assert.HasCount(1, store);
        Assert.AreEqual("testValue", store["testKey"]);
    }

    [TestMethod]
    public void Set_ShouldHandleNullValue()
    {
        // Arrange
        var store = new DefaultAuditValueStore();

        // Act
        store.Set("nullKey", null);

        // Assert
        Assert.HasCount(1, store);
        Assert.IsNull(store["nullKey"]);
    }

    [TestMethod]
    public void Set_ShouldUpdateExistingValue()
    {
        // Arrange
        var store = new DefaultAuditValueStore
        {
            ["existingKey"] = "oldValue"
        };

        // Act
        store.Set("existingKey", "newValue");

        // Assert
        Assert.HasCount(1, store);
        Assert.AreEqual("newValue", store["existingKey"]);
    }

    [TestMethod]
    public void SetValue_ShouldHandleExpression()
    {
        // Arrange
        var store = new DefaultAuditValueStore();
        var testObj = new TestEntityClass
        {
            Field = "Field",
            Property = "Property",
            Nested = new()
            {
                Field = "NestedField",
                Property = "NestedProperty",
            }
        };

        // Act
        store.SetValue(testObj);
        store.SetValue(testObj.Property);
        store.SetValue(testObj.Field);
        store.SetValue(testObj.Nested);
        store.SetValue(testObj.Nested.Field);
        store.SetValue(testObj.Nested.Property);

        // Assert
        Assert.HasCount(6, store);

        Assert.IsTrue(store.TryGetValue("testObj", out object? value));
        Assert.AreEqual(testObj, value);
        Assert.IsTrue(store.TryGetValue("testObj.Property", out value));
        Assert.AreEqual(testObj.Property, value);
        Assert.IsTrue(store.TryGetValue("testObj.Field", out value));
        Assert.AreEqual(testObj.Field, value);
        Assert.IsTrue(store.TryGetValue("testObj.Nested", out value));
        Assert.AreEqual(testObj.Nested, value);
        Assert.IsTrue(store.TryGetValue("testObj.Nested.Field", out value));
        Assert.AreEqual(testObj.Nested.Field, value);
        Assert.IsTrue(store.TryGetValue("testObj.Nested.Property", out value));
        Assert.AreEqual(testObj.Nested.Property, value);
    }

    #endregion Public 方法
}
