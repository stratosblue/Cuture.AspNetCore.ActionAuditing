using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cuture.AspNetCore.ActionAuditing;

internal static class ConstantsCodeGenerateUtil
{
    #region Public 方法

    public static IdentifierNameSyntax? FindGeneratedConstantsSyntax(ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.AttributeLists.SelectMany(m => m.Attributes)
                                                    .Select(m => m.Name)
                                                    .OfType<IdentifierNameSyntax>()
                                                    .FirstOrDefault(m => m.Identifier.Text == "GeneratedConstants");
    }

    public static void Generate(GeneratedConstantsDescriptor descriptor, StringBuilder builder)
    {
        var classDeclarationSyntax = descriptor.ClassDeclarationSyntax;

        var @namespace = "";
        var parenSyntax = classDeclarationSyntax.Parent;
        while (parenSyntax is not null)
        {
            if (parenSyntax is BaseNamespaceDeclarationSyntax namespaceDeclarationSyntax)
            {
                @namespace = (namespaceDeclarationSyntax.Name as IdentifierNameSyntax)?.Identifier.ValueText
                             ?? namespaceDeclarationSyntax.Name.ToString();
                break;
            }
            parenSyntax = classDeclarationSyntax.Parent;
        }

        if (!string.IsNullOrEmpty(@namespace))
        {
            builder.AppendLine($$"""
                               #pragma warning disable IDE0005

                               using System;
                               using System.Collections.Immutable;
                               using System.ComponentModel;
                               
                               namespace {{@namespace}};
                               """);
        }

        GenerateInnerType(descriptor, classDeclarationSyntax, null, builder, 0);
    }

    #endregion Public 方法

    #region Private 方法

    private static IEnumerable<ClassDeclarationSyntax> EnumerateInnerTypes(ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Members.OfType<ClassDeclarationSyntax>()
                                             .Where(m => m.Modifiers.Any(m => m.Text == "partial"));
    }

    private static void GenerateInnerType(GeneratedConstantsDescriptor descriptor,
                                          ClassDeclarationSyntax classDeclarationSyntax,
                                          string? parentClassConstantValue,
                                          StringBuilder builder,
                                          int depth)
    {
        var className = classDeclarationSyntax.Identifier.Text;
        var normalizeClassName = GenerateHelper.Normalize(className, descriptor.ParseMode);

        var currentClassConstantValue = depth == 0   //顶级类型不加类型前缀
                                        ? null
                                        : string.IsNullOrWhiteSpace(parentClassConstantValue)
                                          ? normalizeClassName
                                          : $"{parentClassConstantValue}{descriptor.Separator}{normalizeClassName}";

        builder.AppendLine($$"""

                           partial class {{className}}
                           {
                           """);

        var propertyDeclarationSyntaxes = classDeclarationSyntax.Members.OfType<PropertyDeclarationSyntax>();

        var currentClassConstantValuePrefix = currentClassConstantValue is null
                                              ? null
                                              : $"{currentClassConstantValue}{descriptor.Separator}";

        List<ConstantDescriptor> constantDescriptors = [];
        foreach (var propertyDeclarationSyntax in propertyDeclarationSyntaxes)
        {
            var summary = GenerateHelper.GetSummary(propertyDeclarationSyntax.GetLeadingTrivia());
            var propertyName = propertyDeclarationSyntax.Identifier.Text;
            var normalizePropertyName = GenerateHelper.Normalize(propertyName, descriptor.ParseMode);
            var currentValue = $"{descriptor.ConstantValuePrefix}{currentClassConstantValuePrefix}{normalizePropertyName}";

            constantDescriptors.Add(new(propertyName, currentValue, GenerateHelper.GetSummaryText(summary)));

            builder.AppendLine($$"""

                               /// <summary>{{(string.IsNullOrWhiteSpace(summary) ? null : $"\n{summary}<br/><br/>")}}
                               /// {{currentValue}}
                               /// </summary>
                               public const string {{propertyName}}{{descriptor.ConstantSuffix}} = "{{currentValue}}";
                               """);
        }

        List<string> innerTypeNames = [];
        foreach (var innerClassDeclarationSyntax in EnumerateInnerTypes(classDeclarationSyntax))
        {
            innerTypeNames.Add(innerClassDeclarationSyntax.Identifier.Text);
            GenerateInnerType(descriptor, innerClassDeclarationSyntax, currentClassConstantValue, builder, depth + 1);
        }

        var description = GenerateHelper.GetSummaryText(GenerateHelper.GetSummary(classDeclarationSyntax.GetLeadingTrivia()));
        builder.AppendLine($$"""

                                /// <summary>
                                /// 目录
                                /// </summary>
                                public static InnerTypes.ConstantCatalog Category { get; } = new
                                (Name: {{(depth == 0 ? "string.Empty" : $"\"{className}\"")}},
                                 Description: "{{description}}",
                                 Constants: [{{string.Join(", ", constantDescriptors.Select(m => $"new(\"{m.Name}\", \"{m.Value}\", \"{m.Description}\")"))}}],
                                 Children: [{{string.Join(", ", innerTypeNames.Select(m => $"{m}.Category"))}}]);

                                /// <summary>
                                /// 列举所有项
                                /// </summary>
                                public static IEnumerable<(InnerTypes.ConstantDescriptor Descriptor, InnerTypes.ConstantCatalog Catalog)> EnumerateItems() => {{descriptor.ClassDeclarationSyntax.Identifier.Text}}.EnumerateItems(Category);
                            """);

        if (depth == 0)
        {
            builder.AppendLine("""
                                   /// <summary>
                                   /// 内部类型
                                   /// </summary>
                                   [EditorBrowsable(EditorBrowsableState.Never)]
                                   public class InnerTypes
                                   {
                                       /// <summary>
                                       /// 常量描述符
                                       /// </summary>
                                       /// <param name="Name">名称</param>
                                       /// <param name="Value">值</param>
                                       /// <param name="Description">描述</param>
                                       [EditorBrowsable(EditorBrowsableState.Never)]
                                       public record struct ConstantDescriptor(string Name, string Value, string? Description);
                                       
                                       /// <summary>
                                       /// 常量目录
                                       /// </summary>
                                       /// <param name="Name">名称</param>
                                       /// <param name="Description">描述</param>
                                       /// <param name="Constants">常量列表</param>
                                       /// <param name="Children">子集列表</param>
                                       [EditorBrowsable(EditorBrowsableState.Never)]
                                       public record class ConstantCatalog(string Name, string? Description, ImmutableArray<ConstantDescriptor> Constants, ImmutableArray<ConstantCatalog> Children);
                                   }
                                   
                                   /// <summary>
                                   /// 列举所有项
                                   /// </summary>
                                   private static IEnumerable<(InnerTypes.ConstantDescriptor Descriptor, InnerTypes.ConstantCatalog Catalog)> EnumerateItems(InnerTypes.ConstantCatalog catalog)
                                   {
                                       foreach (var item in catalog.Constants)
                                       {
                                           yield return (item, catalog);
                                       }
                                       foreach (var child in catalog.Children)
                                       {
                                           foreach (var item in EnumerateItems(child))
                                           {
                                               yield return item;
                                           }
                                       }
                                   }
                               """);
        }

        builder.AppendLine("}");
    }

    #endregion Private 方法

    private record struct ConstantDescriptor(string Name, string Value, string? Description);
}
