using System.Collections.Immutable;

namespace Cuture.AspNetCore.ActionAuditing.Test.TestBase;

public record class RequiredPermissionPathTest(string Path, ImmutableArray<string> RequiredPermissions, ImmutableArray<string> ProvidedPermissions)
{
    public override string ToString() => $"{Path} [{string.Join('.', ProvidedPermissions)}]";
}
