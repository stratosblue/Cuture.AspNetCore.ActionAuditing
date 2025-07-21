using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 动作审核构造器
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public record class ActionAuditingBuilder(IMvcBuilder MvcBuilder, IServiceCollection Services);
