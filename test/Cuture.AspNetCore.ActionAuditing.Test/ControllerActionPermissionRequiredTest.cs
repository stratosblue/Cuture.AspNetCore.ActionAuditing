using Cuture.AspNetCore.ActionAuditing.Test.TestBase;

namespace Cuture.AspNetCore.ActionAuditing.Test;

[TestClass]
public class ControllerActionPermissionRequiredTest : AuditingCallbackTestBase
{
    #region Public 方法

    public static IEnumerable<RequiredPermissionPathTest> ShouldApprovedTests()
    {
        return [
            new("/ClassTest/Permission1", ["1Permission1"], ["1Permission1"]),
            new("/ClassTest/Permission1", ["1Permission1"], ["1Permission1", "2Permission2"]),
            new("/ClassTest/Permission2", ["1Permission1", "2Permission2"], ["1Permission1", "2Permission2"]),
            new("/ClassTest/Permission2", ["1Permission1", "2Permission2"], ["1Permission1", "2Permission2", "3Permission3"]),
            new("/ClassTest/Permission3", ["1Permission1", "2Permission2", "3Permission3"], ["1Permission1", "2Permission2", "3Permission3"]),
            new("/ClassTest/Permission3", ["1Permission1", "2Permission2", "3Permission3"], ["1Permission1", "2Permission2", "3Permission3", "4Permission4"]),
            new("/ClassTest/Permission4", ["1Permission1", "2Permission2", "3Permission3", "4Permission4"], ["1Permission1", "2Permission2", "3Permission3", "4Permission4"]),
            new("/ClassTest/Permission4", ["1Permission1", "2Permission2", "3Permission3", "4Permission4"], ["1Permission1", "2Permission2", "3Permission3", "4Permission4", "AnyThing"]),
        ];
    }

    public static IEnumerable<RequiredPermissionPathTest> ShouldDeniedTests()
    {
        return [
            new("/ClassTest/Permission1", ["1Permission1"], []),
            new("/ClassTest/Permission1", ["1Permission1"], ["2Permission2"]),
            new("/ClassTest/Permission2", ["1Permission1", "2Permission2"], ["1Permission1"]),
            new("/ClassTest/Permission2", ["1Permission1", "2Permission2"], ["2Permission2"]),
            new("/ClassTest/Permission2", ["1Permission1", "2Permission2"], ["2Permission2", "3Permission3"]),
            new("/ClassTest/Permission3", ["1Permission1", "2Permission2", "3Permission3"], ["1Permission1", "2Permission2"]),
            new("/ClassTest/Permission3", ["1Permission1", "2Permission2", "3Permission3"], ["1Permission1", "2Permission2", "4Permission4"]),
            new("/ClassTest/Permission3", ["1Permission1", "2Permission2", "3Permission3"], ["2Permission2", "3Permission3", "4Permission4"]),
            new("/ClassTest/Permission4", ["1Permission1", "2Permission2", "3Permission3", "4Permission4"], ["1Permission1", "2Permission2", "3Permission3"]),
            new("/ClassTest/Permission4", ["1Permission1", "2Permission2", "3Permission3", "4Permission4"], ["1Permission1", "4Permission4"]),
            new("/ClassTest/Permission4", ["1Permission1", "2Permission2", "3Permission3", "4Permission4"], ["1Permission1", "2Permission2", "3Permission3", "AnyThing"]),
        ];
    }

    #endregion Public 方法

    #region Public 方法

    [TestMethod]
    [DynamicData(nameof(ShouldApprovedTests), DynamicDataSourceType.Method)]
    public async Task Should_Approved(RequiredPermissionPathTest test)
    {
        CurrentAuditingSyncCallback = (context) =>
        {
            var requiredPermission = context.RequiredPermission;
            Assert.IsTrue(requiredPermission.IsDefined);
            CollectionAssert.AreEqual(test.RequiredPermissions, requiredPermission.Permissions);
            return !requiredPermission.Permissions.Except(test.ProvidedPermissions).Any();
        };

        var response = await TestClient.GetAsync(test.Path);
        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(test.Path, await response.Content.ReadAsStringAsync());
    }

    [TestMethod]
    [DynamicData(nameof(ShouldDeniedTests), DynamicDataSourceType.Method)]
    public async Task Should_Denied(RequiredPermissionPathTest test)
    {
        CurrentAuditingSyncCallback = (context) =>
        {
            var requiredPermission = context.RequiredPermission;
            Assert.IsTrue(requiredPermission.IsDefined);
            CollectionAssert.AreEqual(test.RequiredPermissions, requiredPermission.Permissions);
            return !requiredPermission.Permissions.Except(test.ProvidedPermissions).Any();
        };

        var response = await TestClient.GetAsync(test.Path);
        Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion Public 方法
}
