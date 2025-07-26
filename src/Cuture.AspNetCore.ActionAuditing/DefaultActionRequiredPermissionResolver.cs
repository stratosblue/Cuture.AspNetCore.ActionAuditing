using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 默认的 <inheritdoc cref="IActionRequiredPermissionResolver"/>
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class DefaultActionRequiredPermissionResolver : IActionRequiredPermissionResolver
{
    #region Private 字段

    private readonly ConditionalWeakTable<Endpoint, PermissionDescriptorValue> _permissionDescriptorCache = [];

    #endregion Private 字段

    #region Public 方法

    /// <inheritdoc/>
    public ValueTask<PermissionDescriptor> ResolveAsync(HttpContext httpContext, CancellationToken cancellationToken = default)
    {
        if (httpContext.GetEndpoint() is { } endpoint)
        {
            if (_permissionDescriptorCache.TryGetValue(endpoint, out var cachedResult))
            {
                return ValueTask.FromResult(cachedResult.Value);
            }

            var descriptor = CreateDescriptor(endpoint, httpContext);
            _permissionDescriptorCache.TryAdd(endpoint, descriptor);

            return ValueTask.FromResult(descriptor.Value);
        }
        return ValueTask.FromResult<PermissionDescriptor>(default);

        static PermissionDescriptorValue CreateDescriptor(Endpoint endpoint, HttpContext httpContext)
        {
            var permissions = endpoint.Metadata.OfType<IRequiredPermissionProvider>()
                                               .SelectMany(static m => m.Permissions);

            return new(new(permissions));
        }
    }

    #endregion Public 方法

    /// <summary>
    /// 引用的<see cref="PermissionDescriptor"/>
    /// </summary>
    /// <param name="Value"></param>
    private sealed record PermissionDescriptorValue(PermissionDescriptor Value)
    {
        /// <summary>
        /// Empty
        /// </summary>
        public static PermissionDescriptorValue Empty { get; } = new(new PermissionDescriptor([]));
    }
}
