using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ESC.CONCOST.Abstract;

public static class FormulaExpressionEngine
{
    private static readonly Regex VariableRegex = new(
        @"\b[A-Za-z_][A-Za-z0-9_]*\b",
        RegexOptions.Compiled
    );

    private static readonly HashSet<string> ReservedWords = new(
        new[]
        {
            "AND", "OR", "SUM", "ROUND", "FLOOR", "CEILING",
            "TRUE", "FALSE"
        },
        StringComparer.OrdinalIgnoreCase
    );

    public static decimal EvaluateDecimal(string formula, Dictionary<string, decimal> variables)
    {
        if (string.IsNullOrWhiteSpace(formula))
        {
            return 0m;
        }

        var trimmed = formula.Trim();

        if (IsConditionExpression(trimmed))
        {
            return EvaluateCondition(trimmed, variables) ? 1m : 0m;
        }

        if (trimmed.StartsWith("ROUND(", StringComparison.OrdinalIgnoreCase) && trimmed.EndsWith(")"))
        {
            var inner = ExtractFunctionInner(trimmed, "ROUND");
            return Math.Round(EvaluateDecimal(inner, variables), 0, MidpointRounding.AwayFromZero);
        }

        if (trimmed.StartsWith("FLOOR(", StringComparison.OrdinalIgnoreCase) && trimmed.EndsWith(")"))
        {
            var inner = ExtractFunctionInner(trimmed, "FLOOR");
            return Math.Floor(EvaluateDecimal(inner, variables));
        }

        if (trimmed.StartsWith("CEILING(", StringComparison.OrdinalIgnoreCase) && trimmed.EndsWith(")"))
        {
            var inner = ExtractFunctionInner(trimmed, "CEILING");
            return Math.Ceiling(EvaluateDecimal(inner, variables));
        }

        var expression = ReplaceVariables(trimmed, variables);
        var result = new DataTable().Compute(expression, string.Empty);

        if (result == null || result == DBNull.Value)
        {
            return 0m;
        }

        return Convert.ToDecimal(result, CultureInfo.InvariantCulture);
    }

    public static bool EvaluateCondition(string formula, Dictionary<string, decimal> variables)
    {
        if (string.IsNullOrWhiteSpace(formula))
        {
            return false;
        }

        var orParts = Regex.Split(formula, @"\s+OR\s+", RegexOptions.IgnoreCase);

        foreach (var orPart in orParts)
        {
            var andParts = Regex.Split(orPart, @"\s+AND\s+", RegexOptions.IgnoreCase);
            var andResult = true;

            foreach (var part in andParts)
            {
                andResult = andResult && EvaluateSingleCondition(part.Trim(), variables);
            }

            if (andResult)
            {
                return true;
            }
        }

        return false;
    }

    public static List<string> ExtractVariables(string formula)
    {
        if (string.IsNullOrWhiteSpace(formula))
        {
            return new List<string>();
        }

        return VariableRegex
            .Matches(formula)
            .Select(x => x.Value)
            .Where(x => !ReservedWords.Contains(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static void ValidateUnsafeExpression(string formula)
    {
        if (string.IsNullOrWhiteSpace(formula))
        {
            throw new InvalidOperationException("Formula is empty.");
        }

        var unsafeTokens = new[]
        {
            "System",
            "File",
            "Process",
            "Reflection",
            "Sql",
            "new ",
            "typeof",
            "while",
            "for",
            "foreach",
            "class",
            "namespace",
            "using",
            ";",
            "{",
            "}"
        };

        foreach (var token in unsafeTokens)
        {
            if (formula.Contains(token, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Invalid token in formula: {token}");
            }
        }
    }

    private static bool IsConditionExpression(string formula)
    {
        return formula.Contains(">=")
               || formula.Contains("<=")
               || formula.Contains("==")
               || formula.Contains("!=")
               || formula.Contains(">")
               || formula.Contains("<")
               || Regex.IsMatch(formula, @"\s+AND\s+", RegexOptions.IgnoreCase)
               || Regex.IsMatch(formula, @"\s+OR\s+", RegexOptions.IgnoreCase);
    }

    private static bool EvaluateSingleCondition(string condition, Dictionary<string, decimal> variables)
    {
        var operators = new[] { ">=", "<=", "==", "!=", ">", "<" };

        foreach (var op in operators)
        {
            var index = condition.IndexOf(op, StringComparison.OrdinalIgnoreCase);
            if (index <= -1)
            {
                continue;
            }

            var leftText = condition[..index].Trim();
            var rightText = condition[(index + op.Length)..].Trim();

            var left = EvaluateDecimal(leftText, variables);
            var right = EvaluateDecimal(rightText, variables);

            return op switch
            {
                ">=" => left >= right,
                "<=" => left <= right,
                "==" => left == right,
                "!=" => left != right,
                ">" => left > right,
                "<" => left < right,
                _ => false
            };
        }

        return EvaluateDecimal(condition, variables) != 0m;
    }

    private static string ReplaceVariables(string formula, Dictionary<string, decimal> variables)
    {
        return VariableRegex.Replace(formula, match =>
        {
            var key = match.Value;

            if (ReservedWords.Contains(key))
            {
                return key;
            }

            if (!variables.TryGetValue(key, out var value))
            {
                throw new InvalidOperationException($"Unknown variable: {key}");
            }

            return value.ToString(CultureInfo.InvariantCulture);
        });
    }

    private static string ExtractFunctionInner(string formula, string functionName)
    {
        var prefix = functionName + "(";

        if (!formula.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || !formula.EndsWith(")"))
        {
            return formula;
        }

        return formula.Substring(prefix.Length, formula.Length - prefix.Length - 1);
    }
}