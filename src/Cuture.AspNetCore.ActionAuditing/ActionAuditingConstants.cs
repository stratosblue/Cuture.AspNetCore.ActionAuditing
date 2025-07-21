using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// ActionAuditing 常量
/// </summary>
public static class ActionAuditingConstants
{
    #region Public 字段

    /// <summary>
    /// <see cref="ActionAuditingExecutingContext"/> 在 <see cref="HttpContext.Items"/> 中的默认存放键
    /// </summary>
    public const string ActionAuditingExecutingContextHttpContextItemsKey = "@AuditingExecutingContext";

    /// <summary>
    /// <see cref="IAuditValueStore"/> 在 <see cref="HttpContext.Items"/> 中的默认存放键
    /// </summary>
    public const string AuditValueStoreHttpContextItemsKey = "@AuditValueStore";

    #endregion Public 字段
}
