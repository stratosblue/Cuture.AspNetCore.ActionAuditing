using Cuture.AspNetCore.ActionAuditing.Internal;
using Cuture.AspNetCore.ActionAuditing.Test.TestBase;

namespace Cuture.AspNetCore.ActionAuditing.Test.Internal;

[TestClass]
public class HttpContextAuditValueStoreAccessorTest
{
    #region Public 方法

    [TestMethod]
    public void Current_Get_ShouldReturnNull_WhenHttpContextIsNull()
    {
        // Arrange
        var contextAccessor = new TestHttpContextAccessor();
        var accessor = new HttpContextAuditValueStoreAccessor(contextAccessor);

        // Act
        var result = accessor.Current;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Current_Get_ShouldReturnNull_WhenItemNotExists()
    {
        // Arrange
        var contextAccessor = new TestHttpContextAccessor()
        {
            HttpContext = new TestHttpContext(),
        };
        var accessor = new HttpContextAuditValueStoreAccessor(contextAccessor);

        // Act
        var result = accessor.Current;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Current_Get_ShouldReturnStore_WhenItemExists()
    {
        // Arrange
        var store = new DefaultAuditValueStore();
        var contextAccessor = new TestHttpContextAccessor()
        {
            HttpContext = new TestHttpContext()
            {
                Items = new Dictionary<object, object?>
                {
                    [ActionAuditingConstants.AuditValueStoreHttpContextItemsKey] = store
                }
            },
        };

        var accessor = new HttpContextAuditValueStoreAccessor(contextAccessor);

        // Act
        var result = accessor.Current;

        // Assert
        Assert.AreEqual(store, result);
    }

    [TestMethod]
    public void Current_Set_ShouldStoreValue()
    {
        // Arrange
        var store = new DefaultAuditValueStore();
        var contextAccessor = new TestHttpContextAccessor()
        {
            HttpContext = new TestHttpContext()
        };
        var accessor = new HttpContextAuditValueStoreAccessor(contextAccessor);

        // Act
        accessor.Current = store;

        // Assert
        Assert.AreEqual(store, contextAccessor.HttpContext.Items[ActionAuditingConstants.AuditValueStoreHttpContextItemsKey]);
    }

    [TestMethod]
    public void Current_Set_ShouldThrow_WhenHttpContextIsNull()
    {
        // Arrange
        var contextAccessor = new TestHttpContextAccessor();
        var accessor = new HttpContextAuditValueStoreAccessor(contextAccessor);

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => accessor.Current = new DefaultAuditValueStore());
    }

    [TestMethod]
    public void Current_SetNull_ShouldStoreNull()
    {
        // Arrange
        var contextAccessor = new TestHttpContextAccessor()
        {
            HttpContext = new TestHttpContext()
        };
        var accessor = new HttpContextAuditValueStoreAccessor(contextAccessor);

        // Act
        accessor.Current = null;

        // Assert
        Assert.IsNull(contextAccessor.HttpContext.Items[ActionAuditingConstants.AuditValueStoreHttpContextItemsKey]);
    }

    #endregion Public 方法
}
