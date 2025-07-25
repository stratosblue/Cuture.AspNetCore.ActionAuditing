// 代码由 AI 自动生成

using Cuture.AspNetCore.ActionAuditing.Abstractions;

namespace Cuture.AspNetCore.ActionAuditing.Test.Extensions;

/// <summary>
/// IAuditValueStoreExtensions 测试
/// </summary>
[TestClass]
public class IAuditValueStoreExtensionsTest
{
    #region Private 字段

    private IAuditValueStore _store = null!;

    #endregion Private 字段

    #region Public 方法

    /// <summary>
    /// 测试初始化
    /// </summary>
    [TestInitialize]
    public void Initialize()
    {
        _store = new DefaultAuditValueStore();
    }

    /// <summary>
    /// 应该能够获取或设置值
    /// </summary>
    [TestMethod]
    public void Should_GetOrSet_Value()
    {
        var value = _store.GetOrSet("key1", "value1");
        Assert.AreEqual("value1", value);

        var value2 = _store.GetOrSet("key1", "value2");
        Assert.AreEqual("value1", value2);
    }

    /// <summary>
    /// 应该能够获取或设置工厂生成的值
    /// </summary>
    [TestMethod]
    public void Should_GetOrSet_ValueFromFactory()
    {
        var value = _store.GetOrSet("key1", () => "value1");
        Assert.AreEqual("value1", value);

        var value2 = _store.GetOrSet("key1", () => "value2");
        Assert.AreEqual("value1", value2);
    }

    /// <summary>
    /// 应该能够设置表达式值
    /// </summary>
    [TestMethod]
    public void Should_SetValue_WithExpression()
    {
        var testValue = "testValue";
        _store.SetValue(testValue);

        Assert.IsTrue(_store.TryGetValue("testValue", out var value));
        Assert.AreEqual(testValue, value);
    }

    /// <summary>
    /// 应该能够获取类型化的值
    /// </summary>
    [TestMethod]
    public void Should_TryGetValue_Typed()
    {
        _store.Set("key1", "value1");
        Assert.IsTrue(_store.TryGetValue<string>("key1", out var value));
        Assert.AreEqual("value1", value);
    }

    /// <summary>
    /// 应该能够处理类型不匹配的情况
    /// </summary>
    [TestMethod]
    public void Should_TryGetValue_Typed_Mismatch()
    {
        _store.Set("key1", 123);
        Assert.IsFalse(_store.TryGetValue<string>("key1", out var value));
        Assert.IsNull(value);
    }

    #endregion Public 方法
}
