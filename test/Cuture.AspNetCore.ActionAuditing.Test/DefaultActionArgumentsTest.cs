// 代码由 AI 自动生成

using System.Collections;

namespace Cuture.AspNetCore.ActionAuditing.Test;

[TestClass]
public class DefaultActionArgumentsTest
{
    #region Public 方法

    /// <summary>
    /// 测试GetEnumerator方法
    /// </summary>
    [TestMethod]
    public void Should_GetEnumerator_Success()
    {
        var dictionary = CreateTestDictionary();
        var arguments = new DefaultActionArguments(dictionary);

        var enumerator = arguments.GetEnumerator();
        Assert.IsNotNull(enumerator);

        var count = 0;
        while (enumerator.MoveNext())
        {
            count++;
        }
        Assert.AreEqual(dictionary.Count, count);
    }

    /// <summary>
    /// 测试IEnumerable.GetEnumerator方法
    /// </summary>
    [TestMethod]
    public void Should_GetIEnumerableEnumerator_Success()
    {
        var dictionary = CreateTestDictionary();
        var arguments = new DefaultActionArguments(dictionary);

        var enumerator = ((IEnumerable)arguments).GetEnumerator();
        Assert.IsNotNull(enumerator);

        var count = 0;
        while (enumerator.MoveNext())
        {
            count++;
        }
        Assert.AreEqual(dictionary.Count, count);
    }

    /// <summary>
    /// 测试Remove不存在的键
    /// </summary>
    [TestMethod]
    public void Should_Remove_ReturnFalse_WhenKeyNotExists()
    {
        var dictionary = CreateTestDictionary();
        var arguments = new DefaultActionArguments(dictionary);

        var originalCount = dictionary.Count;
        var result = arguments.Remove("not_exist_key");

        Assert.IsFalse(result);
        Assert.AreEqual(originalCount, dictionary.Count);
    }

    /// <summary>
    /// 测试Remove方法
    /// </summary>
    [TestMethod]
    public void Should_Remove_Success()
    {
        var dictionary = CreateTestDictionary();
        var arguments = new DefaultActionArguments(dictionary);

        var originalCount = dictionary.Count;
        var result = arguments.Remove("key1");

        Assert.IsTrue(result);
        Assert.AreEqual(originalCount - 1, dictionary.Count);
    }

    /// <summary>
    /// 测试Count属性
    /// </summary>
    [TestMethod]
    public void Should_Return_CorrectCount()
    {
        var dictionary = CreateTestDictionary();
        var arguments = new DefaultActionArguments(dictionary);

        Assert.AreEqual(dictionary.Count, arguments.Count);
    }

    /// <summary>
    /// 测试Set新键值对
    /// </summary>
    [TestMethod]
    public void Should_Set_NewKey_Success()
    {
        var dictionary = CreateTestDictionary();
        var arguments = new DefaultActionArguments(dictionary);

        var newKey = "new_key";
        var newValue = "new_value";
        var originalCount = dictionary.Count;
        var result = arguments.Set(newKey, newValue);

        Assert.IsTrue(result);
        Assert.AreEqual(originalCount + 1, dictionary.Count);
        Assert.AreEqual(newValue, dictionary[newKey]);
    }

    /// <summary>
    /// 测试Set方法
    /// </summary>
    [TestMethod]
    public void Should_Set_Success()
    {
        var dictionary = CreateTestDictionary();
        var arguments = new DefaultActionArguments(dictionary);

        var newValue = "new_value";
        var result = arguments.Set("key1", newValue);

        Assert.IsTrue(result);
        Assert.AreEqual(newValue, dictionary["key1"]);
    }

    /// <summary>
    /// 测试TryGetValue不存在的键
    /// </summary>
    [TestMethod]
    public void Should_TryGetValue_ReturnFalse_WhenKeyNotExists()
    {
        var dictionary = CreateTestDictionary();
        var arguments = new DefaultActionArguments(dictionary);

        var result = arguments.TryGetValue("not_exist_key", out var value);

        Assert.IsFalse(result);
        Assert.IsNull(value);
    }

    /// <summary>
    /// 测试TryGetValue方法
    /// </summary>
    [TestMethod]
    public void Should_TryGetValue_Success()
    {
        var dictionary = CreateTestDictionary();
        var arguments = new DefaultActionArguments(dictionary);

        var result = arguments.TryGetValue("key1", out var value);

        Assert.IsTrue(result);
        Assert.AreEqual(dictionary["key1"], value);
    }

    #endregion Public 方法

    #region Private 方法

    /// <summary>
    /// 创建测试用的字典
    /// </summary>
    private static Dictionary<string, object?> CreateTestDictionary()
    {
        return new Dictionary<string, object?>
        {
            ["key1"] = "value1",
            ["key2"] = 2,
            ["key3"] = null
        };
    }

    #endregion Private 方法
}
