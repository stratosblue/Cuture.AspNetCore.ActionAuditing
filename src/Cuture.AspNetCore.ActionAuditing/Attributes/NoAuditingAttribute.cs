#pragma warning disable IDE0130

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 不需要审计
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class NoAuditingAttribute() : Attribute
{
}
