using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ActionAuditing.Internal;

#region Abstraction

internal abstract class HttpContextItemsAccessor<T>(IHttpContextAccessor contextAccessor, string httpContextItemsKey)
    : HttpContextItemsAccessor<T, T>(contextAccessor, httpContextItemsKey)
    where T : class
{ }

internal abstract class HttpContextItemsAccessor<T, TImplementation>(IHttpContextAccessor contextAccessor, string httpContextItemsKey)
    where T : class where TImplementation : T
{
    #region Public 属性

    public T? Current
    {
        get
        {
            if (ContextAccessor.HttpContext?.Items.TryGetValue(httpContextItemsKey, out var value) == true
                && value is not null)
            {
                return (TImplementation)value;
            }
            return default;
        }

        set
        {
            if (ContextAccessor.HttpContext is not { } httpContext)
            {
                throw new InvalidOperationException("Can not access context now.");
            }
            httpContext.Items[httpContextItemsKey] = value;
        }
    }

    /// <inheritdoc cref="IHttpContextAccessor"/>
    public IHttpContextAccessor ContextAccessor { get; } = contextAccessor;

    #endregion Public 属性
}

#endregion Abstraction

internal sealed class HttpContextAuditValueStoreAccessor(IHttpContextAccessor contextAccessor)
    : HttpContextItemsAccessor<IAuditValueStore, DefaultAuditValueStore>(contextAccessor, ActionAuditingConstants.AuditValueStoreHttpContextItemsKey)
    , IAuditValueStoreAccessor
{
    public bool Initialize()
    {
        if (Current is not null)
        {
            return true;
        }
        if (ContextAccessor.HttpContext is null)
        {
            return false;
        }

        Current = new DefaultAuditValueStore();

        return true;
    }
}

internal sealed class HttpContextActionAuditingExecutingContextAccessor(IHttpContextAccessor contextAccessor)
    : HttpContextItemsAccessor<ActionAuditingExecutingContext>(contextAccessor, ActionAuditingConstants.ActionAuditingExecutingContextHttpContextItemsKey)
    , IActionAuditingExecutingContextAccessor
{ }
