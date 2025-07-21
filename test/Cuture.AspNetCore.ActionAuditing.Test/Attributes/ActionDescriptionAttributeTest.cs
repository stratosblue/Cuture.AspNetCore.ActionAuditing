namespace Cuture.AspNetCore.ActionAuditing.Test.Attributes;

[TestClass]
public class ActionDescriptionAttributeTests
{
    #region Public 方法

    [TestMethod]
    public void Constructor_ShouldThrow_WhenDescriptionFormatIsNull()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new AuditDescriptionAttribute(null!));
    }

    #endregion Public 方法
}
