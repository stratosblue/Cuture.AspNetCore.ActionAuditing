// 代码由 AI 自动生成

using System.Collections;
using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ActionAuditing.Test;

[TestClass]
public class DefaultEndpointActionArgumentsTest
{
    #region Public 方法

    [TestMethod]
    public void Should_CreateArgumentNameMap_Success()
    {
        var context = new EndpointFilterFactoryContext
        {
            MethodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!
        };

        var result = DefaultEndpointActionArguments.CreateArgumentNameMap(context);

        Assert.AreEqual(3, result.Count);
        Assert.IsTrue(result.ContainsKey("param1"));
        Assert.IsTrue(result.ContainsKey("param2"));
        Assert.IsTrue(result.ContainsKey("param3"));
    }

    [TestMethod]
    public void Should_Enumerate_AllItems()
    {
        var (nameMap, arguments) = CreateTestData();
        var actionArguments = new DefaultEndpointActionArguments(nameMap, arguments);

        var enumeratedItems = actionArguments.ToList();

        Assert.AreEqual(3, enumeratedItems.Count);
        Assert.AreEqual("param1", enumeratedItems[0].Key);
        Assert.AreEqual("value1", enumeratedItems[0].Value);
        Assert.AreEqual("param2", enumeratedItems[1].Key);
        Assert.AreEqual(123, enumeratedItems[1].Value);
        Assert.AreEqual("param3", enumeratedItems[2].Key);
        Assert.IsNull(enumeratedItems[2].Value);
    }

    [TestMethod]
    public void Should_GetEnumerator_WorkCorrectly()
    {
        var (nameMap, arguments) = CreateTestData();
        var actionArguments = new DefaultEndpointActionArguments(nameMap, arguments);

        var enumerator = actionArguments.GetEnumerator();
        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual("param1", enumerator.Current.Key);
        Assert.AreEqual("value1", enumerator.Current.Value);

        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual("param2", enumerator.Current.Key);
        Assert.AreEqual(123, enumerator.Current.Value);

        Assert.IsTrue(enumerator.MoveNext());
        Assert.AreEqual("param3", enumerator.Current.Key);
        Assert.IsNull(enumerator.Current.Value);

        Assert.IsFalse(enumerator.MoveNext());
    }

    [TestMethod]
    public void Should_GetIEnumerableEnumerator_WorkCorrectly()
    {
        var (nameMap, arguments) = CreateTestData();
        var actionArguments = new DefaultEndpointActionArguments(nameMap, arguments);

        var enumerator = ((IEnumerable)actionArguments).GetEnumerator();
        Assert.IsTrue(enumerator.MoveNext());
        var current = (KeyValuePair<string, object?>)enumerator.Current!;
        Assert.AreEqual("param1", current.Key);
        Assert.AreEqual("value1", current.Value);
    }

    [TestMethod]
    public void Should_Remove_ReturnFalse_WhenNameNotExist()
    {
        var (nameMap, arguments) = CreateTestData();
        var actionArguments = new DefaultEndpointActionArguments(nameMap, arguments);

        var result = actionArguments.Remove("not_exist");

        Assert.IsFalse(result);
        Assert.AreEqual("value1", arguments[0]);
        Assert.AreEqual(123, arguments[1]);
        Assert.IsNull(arguments[2]);
    }

    [TestMethod]
    public void Should_Remove_ReturnTrue_AndSetNull_WhenNameExist()
    {
        var (nameMap, arguments) = CreateTestData();
        var actionArguments = new DefaultEndpointActionArguments(nameMap, arguments);

        var result = actionArguments.Remove("param1");

        Assert.IsTrue(result);
        Assert.IsNull(arguments[0]);
        Assert.AreEqual(123, arguments[1]);
        Assert.IsNull(arguments[2]);
    }

    [TestMethod]
    public void Should_Set_ReturnFalse_WhenNameNotExist()
    {
        var (nameMap, arguments) = CreateTestData();
        var actionArguments = new DefaultEndpointActionArguments(nameMap, arguments);

        var result = actionArguments.Set("not_exist", "new_value");

        Assert.IsFalse(result);
        Assert.AreEqual("value1", arguments[0]);
        Assert.AreEqual(123, arguments[1]);
        Assert.IsNull(arguments[2]);
    }

    [TestMethod]
    public void Should_Set_ReturnTrue_AndUpdateValue_WhenNameExist()
    {
        var (nameMap, arguments) = CreateTestData();
        var actionArguments = new DefaultEndpointActionArguments(nameMap, arguments);

        var result = actionArguments.Set("param1", "new_value");

        Assert.IsTrue(result);
        Assert.AreEqual("new_value", arguments[0]);
        Assert.AreEqual(123, arguments[1]);
        Assert.IsNull(arguments[2]);
    }

    [TestMethod]
    public void Should_TryGetValue_ReturnFalse_WhenNameNotExist()
    {
        var (nameMap, arguments) = CreateTestData();
        var actionArguments = new DefaultEndpointActionArguments(nameMap, arguments);

        var result = actionArguments.TryGetValue("not_exist", out var value);

        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void Should_TryGetValue_ReturnTrue_AndGetValue_WhenNameExist()
    {
        var (nameMap, arguments) = CreateTestData();
        var actionArguments = new DefaultEndpointActionArguments(nameMap, arguments);

        var result = actionArguments.TryGetValue("param1", out var value);

        Assert.IsTrue(result);
        Assert.AreEqual("value1", value);
    }

    #endregion Public 方法

    #region Private 方法

    /// <summary>
    /// 创建测试用的参数名称映射和参数列表
    /// </summary>
    private static (IReadOnlyDictionary<string, int> nameMap, IList<object?> arguments) CreateTestData()
    {
        var nameMap = new Dictionary<string, int>
        {
            ["param1"] = 0,
            ["param2"] = 1,
            ["param3"] = 2
        }.AsReadOnly();

        var arguments = new List<object?> { "value1", 123, null };

        return (nameMap, arguments);
    }

    #endregion Private 方法

    #region Private 类

    private class TestClass
    {
        #region Public 方法

        public void TestMethod(string param1, int param2, object? param3)
        { }

        #endregion Public 方法
    }

    #endregion Private 类
}
