using System.Reflection;
using System.Resources;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cuture.AspNetCore.ActionAuditing;

[Generator(LanguageNames.CSharp)]
public class ConstantsGenerator : IIncrementalGenerator
{
    #region Public 方法

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var declarationsProvider = context.SyntaxProvider.CreateSyntaxProvider(FilterGeneratedConstantsSyntaxNode, TransformSyntaxNode);

        context.RegisterImplementationSourceOutput(declarationsProvider, (context, descriptor) =>
        {
            if (descriptor is null)
            {
                return;
            }
            var builder = new StringBuilder();

            ConstantsCodeGenerateUtil.Generate(descriptor.Value, builder);

            var normalizeWhitespaceCode = GetNormalizeWhitespaceCode(builder.ToString());

            context.AddSource($"Cuture.AspNetCore.ActionAuditing.Generator.{descriptor.Value.ClassDeclarationSyntax.Identifier.Text}.g.cs", normalizeWhitespaceCode);
        });

        context.RegisterPostInitializationOutput(context =>
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cuture.AspNetCore.ActionAuditing.PredefinedAttributes.cs");
            using var reader = new StreamReader(stream);
            context.AddSource("Cuture.AspNetCore.ActionAuditing.PredefinedAttributes.g.cs", reader.ReadToEnd());
        });
    }

    #endregion Public 方法

    #region Private 方法

    private static string GetNormalizeWhitespaceCode(string code)
    {
        return CSharpSyntaxTree.ParseText(code).GetRoot().NormalizeWhitespace(elasticTrivia: true).ToString();
    }

    #endregion Private 方法

    #region Private 方法

    private bool FilterGeneratedConstantsSyntaxNode(SyntaxNode node, CancellationToken token)
    {
        if (node is ClassDeclarationSyntax classDeclarationSyntax
            && classDeclarationSyntax.Modifiers.Any(m => m.Text == "partial")
            && ConstantsCodeGenerateUtil.FindGeneratedConstantsSyntax(classDeclarationSyntax) is not null)
        {
            return true;
        }
        return false;
    }

    private GeneratedConstantsDescriptor? TransformSyntaxNode(GeneratorSyntaxContext context, CancellationToken token)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax, token) is not { } namedTypeSymbol
            || namedTypeSymbol.GetAttributes().Where(m => m.AttributeClass?.ToDisplayString() == typeof(GeneratedConstantsAttribute).FullName).FirstOrDefault() is not { } attribute
            || attribute.ConstructorArguments is not { IsDefaultOrEmpty: false, Length: 4 } constructorArguments
            || constructorArguments[0].Value is not int parseMode
            || constructorArguments[2].Value?.ToString() is not string constantSuffix)
        {
            return default;
        }
        return new(ClassDeclarationSyntax: classDeclarationSyntax,
                   ParseMode: (ConstantValueParseMode)parseMode,
                   Separator: constructorArguments[1].Value?.ToString(),
                   ConstantSuffix: constantSuffix,
                   ConstantValuePrefix: constructorArguments[3].Value?.ToString());
    }

    #endregion Private 方法
}

record struct GeneratedConstantsDescriptor(ClassDeclarationSyntax ClassDeclarationSyntax,
                                           ConstantValueParseMode ParseMode,
                                           string? Separator,
                                           string ConstantSuffix,
                                           string? ConstantValuePrefix);
