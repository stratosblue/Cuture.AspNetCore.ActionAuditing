using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Cuture.AspNetCore.ActionAuditing;

internal class GenerateHelper
{
    #region Private 字段

    private static readonly Regex s_normalizeRegex = new("(?<=.)([A-Z])", RegexOptions.Compiled);

    private static readonly Regex s_summaryRegex = new("[ \\t]*/// *\\</?summary\\>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex s_summaryTextRegex = new(@"(\s*///\s*)|(\r\n|\n|\r)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex s_xmlTagRegex = new(@"<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    #endregion Private 字段

    #region Public 方法

    public static string? GetSummary(SyntaxTriviaList syntaxTrivias)
    {
        var lines = syntaxTrivias.ToString().Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var builder = new StringBuilder();
        var isStarted = false;

        foreach (var trivia in lines)
        {
            var triviaString = trivia.ToString();
            if (s_summaryRegex.IsMatch(triviaString))
            {
                if (isStarted)
                {
                    break;
                }
                isStarted = true;
                continue;
            }
            if (isStarted)
            {
                builder.AppendLine(triviaString.Trim());
            }
        }

        return builder.ToString().TrimEnd();
    }

    public static string? GetSummaryText(string? summary)
    {
        if (string.IsNullOrEmpty(summary))
        {
            return summary;
        }

        return s_xmlTagRegex.Replace(s_summaryTextRegex.Replace(summary, " "), string.Empty).Trim();
    }

    public static string Normalize(string variableName, ConstantValueParseMode mode = ConstantValueParseMode.Default)
    {
        if (string.IsNullOrEmpty(variableName))
            return variableName;

        return mode switch
        {
            ConstantValueParseMode.LowerCase => variableName.ToLowerInvariant(),
            ConstantValueParseMode.UpperCase => variableName.ToUpperInvariant(),
            ConstantValueParseMode.SnakeCase => s_normalizeRegex.Replace(variableName, "_$1").ToLowerInvariant(),// 将帕斯卡命名转换为蛇形命名
            ConstantValueParseMode.KebabCase => s_normalizeRegex.Replace(variableName, "-$1").ToLowerInvariant(),// 将帕斯卡命名转换为短横线命名
            _ => s_normalizeRegex.Replace(variableName, "_$1").ToUpperInvariant(),// 将帕斯卡命名转换为常量大写
        };
    }

    #endregion Public 方法
}
