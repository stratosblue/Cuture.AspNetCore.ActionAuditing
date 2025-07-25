// 代码由 AI 自动生成

namespace Cuture.AspNetCore.ActionAuditing.Test;

/// <summary>
/// ActionAuditDescriptionFormatter 测试类
/// </summary>
[TestClass]
public class ActionAuditDescriptionFormatterTest
{
    #region Private 字段

    private static readonly DefaultActionArguments s_mockArguments = new([]);

    private static readonly DefaultAuditValueStore s_mockValueStore = [];

    #endregion Private 字段

    #region Public 方法

    /// <summary>
    /// 测试格式化复杂属性路径
    /// </summary>
    [TestMethod]
    public void Should_Format_ComplexPropertyPath_Success()
    {
        var format = "用户信息: {User.Profile.Name}";
        var expectedName = "TestProfile";
        var user = new { Profile = new { Name = expectedName } };

        s_mockValueStore.Set("User", user);
        var result = ActionAuditDescriptionFormatter.Format(format, s_mockValueStore, null);

        Assert.AreEqual(format, result.Format);
        Assert.AreEqual($"用户信息: {expectedName}", result.Description);
    }

    /// <summary>
    /// 测试格式化混合静态文本和变量
    /// </summary>
    [TestMethod]
    public void Should_Format_MixedStaticAndVariables_Success()
    {
        var format = "用户: {User.Name}, 年龄: {User.Age}, 来自: {User.Address.City}";
        var user = new
        {
            Name = "Test",
            Age = 30,
            Address = new { City = "Beijing" }
        };

        s_mockValueStore.Set("User", user);
        var result = ActionAuditDescriptionFormatter.Format(format, s_mockValueStore, null);

        Assert.AreEqual(format, result.Format);
        Assert.AreEqual("用户: Test, 年龄: 30, 来自: Beijing", result.Description);
    }

    /// <summary>
    /// 测试格式化当属性访问出错时显示错误标记
    /// </summary>
    [TestMethod]
    public void Should_Format_ShowError_WhenPropertyAccessFailed()
    {
        var format = "用户: {User.ValidProperty.Property}";
        var user = new { ValidProperty = new ExceptionTestClass() };

        s_mockValueStore.Set("User", user);
        var result = ActionAuditDescriptionFormatter.Format(format, s_mockValueStore, null);

        Assert.AreEqual(format, result.Format);
        Assert.AreEqual("用户: [Error]", result.Description);
    }

    /// <summary>
    /// 测试格式化当值为null时显示null
    /// </summary>
    [TestMethod]
    public void Should_Format_ShowNull_WhenValueIsNull()
    {
        var format = "用户: {UserName}";

        var result = ActionAuditDescriptionFormatter.Format(format, s_mockValueStore, null);

        Assert.AreEqual(format, result.Format);
        Assert.AreEqual("用户: null", result.Description);
    }

    /// <summary>
    /// 测试格式化简单变量替换
    /// </summary>
    [TestMethod]
    public void Should_Format_SimpleVariable_Success()
    {
        var format = "用户: {UserName}";
        var expectedUserName = "TestUser";

        s_mockValueStore.Set("UserName", expectedUserName);
        var result = ActionAuditDescriptionFormatter.Format(format, s_mockValueStore, null);

        Assert.AreEqual(format, result.Format);
        Assert.AreEqual($"用户: {expectedUserName}", result.Description);
    }

    /// <summary>
    /// 测试格式化静态文本
    /// </summary>
    [TestMethod]
    public void Should_Format_StaticText_Success()
    {
        var format = "这是一个静态文本";
        var result = ActionAuditDescriptionFormatter.Format(format, null, null);

        Assert.AreEqual(format, result.Format);
        Assert.AreEqual(format, result.Description);
    }

    /// <summary>
    /// 测试格式化优先使用ValueStore中的值
    /// </summary>
    [TestMethod]
    public void Should_PreferValueStore_OverArguments()
    {
        var format = "值: {Value}";
        var valueStoreValue = "FromStore";
        var argumentsValue = "FromArguments";

        s_mockValueStore.Set("Value", valueStoreValue);
        s_mockArguments.Set("Value", argumentsValue);
        var result = ActionAuditDescriptionFormatter.Format(format, s_mockValueStore, s_mockArguments);

        Assert.AreEqual($"值: {valueStoreValue}", result.Description);
    }

    /// <summary>
    /// 测试格式化当ValueStore中不存在时使用Arguments中的值
    /// </summary>
    [TestMethod]
    public void Should_UseArguments_WhenValueStoreNotExists()
    {
        var format = "值: {Value}";
        var argumentsValue = "FromArguments";

        s_mockArguments.Set("Value", argumentsValue);
        var result = ActionAuditDescriptionFormatter.Format(format, null, s_mockArguments);

        Assert.AreEqual($"值: {argumentsValue}", result.Description);
    }

    /// <summary>
    /// 测试格式化缓存机制
    /// </summary>
    [TestMethod]
    public void Should_UseCache_ForSameFormatString()
    {
        var format = "测试缓存: {Value}";
        var value1 = "First";
        var value2 = "Second";

        s_mockValueStore.Set("Value", value1);
        var result1 = ActionAuditDescriptionFormatter.Format(format, s_mockValueStore, null);

        s_mockValueStore.Set("Value", value2);
        var result2 = ActionAuditDescriptionFormatter.Format(format, s_mockValueStore, null);

        Assert.AreEqual($"测试缓存: {value1}", result1.Description);
        Assert.AreEqual($"测试缓存: {value2}", result2.Description);
    }

    #endregion Public 方法

    #region Private 类

    private class ExceptionTestClass
    {
        #region Public 属性

        public string Property => throw new InvalidOperationException();

        #endregion Public 属性
    }

    #endregion Private 类
}
