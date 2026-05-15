using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ESC.CONCOST.Base.Engine
{
    public class FormulaEngine
    {
        private readonly string _expression;
        private readonly Dictionary<string, decimal> _variables;
        private readonly Dictionary<string, Func<decimal[], decimal>> _functions;
        private int _pos;
        private char _currentChar;

        public FormulaEngine(string expression, Dictionary<string, decimal> variables = null)
        {
            _expression = expression ?? string.Empty;
            _variables = variables != null 
                ? new Dictionary<string, decimal>(variables, StringComparer.OrdinalIgnoreCase) 
                : new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            
            _functions = new Dictionary<string, Func<decimal[], decimal>>(StringComparer.OrdinalIgnoreCase)
            {
                { "ROUND", args => args.Length == 1 ? Math.Round(args[0]) : Math.Round(args[0], (int)args[1]) },
                { "FLOOR", args => Math.Floor(args[0]) },
                { "CEILING", args => Math.Ceiling(args[0]) },
                { "ABS", args => Math.Abs(args[0]) },
                { "MIN", args => Math.Min(args[0], args[1]) },
                { "MAX", args => Math.Max(args[0], args[1]) }
            };

            _pos = -1;
            NextChar();
        }

        public decimal Evaluate()
        {
            if (string.IsNullOrWhiteSpace(_expression))
                return 0;

            var result = ParseExpression();
            if (_pos < _expression.Length)
            {
                throw new Exception($"Unexpected character at position {_pos}: '{_currentChar}'");
            }
            return result;
        }

        private void NextChar()
        {
            _pos++;
            _currentChar = _pos < _expression.Length ? _expression[_pos] : '\0';
        }

        private bool Eat(char charToEat)
        {
            while (_currentChar == ' ' || _currentChar == '\t') NextChar();
            if (_currentChar == charToEat)
            {
                NextChar();
                return true;
            }
            return false;
        }

        private decimal ParseExpression()
        {
            var x = ParseLogicalOr();
            return x;
        }

        private decimal ParseLogicalOr()
        {
            var x = ParseLogicalAnd();
            for (;;)
            {
                if (Eat('O') && Eat('R')) x = (x != 0 || ParseLogicalAnd() != 0) ? 1 : 0;
                else if (Eat('|') && Eat('|')) x = (x != 0 || ParseLogicalAnd() != 0) ? 1 : 0;
                else return x;
            }
        }

        private decimal ParseLogicalAnd()
        {
            var x = ParseComparison();
            for (;;)
            {
                if (Eat('A') && Eat('N') && Eat('D')) x = (x != 0 && ParseComparison() != 0) ? 1 : 0;
                else if (Eat('&') && Eat('&')) x = (x != 0 && ParseComparison() != 0) ? 1 : 0;
                else return x;
            }
        }

        private decimal ParseComparison()
        {
            var x = ParseTerm();
            for (;;)
            {
                if (Eat('>'))
                {
                    if (Eat('=')) x = x >= ParseTerm() ? 1 : 0;
                    else x = x > ParseTerm() ? 1 : 0;
                }
                else if (Eat('<'))
                {
                    if (Eat('=')) x = x <= ParseTerm() ? 1 : 0;
                    else x = x < ParseTerm() ? 1 : 0;
                }
                else if (Eat('='))
                {
                    if (Eat('=')) x = x == ParseTerm() ? 1 : 0;
                    else x = x == ParseTerm() ? 1 : 0; // Support single = as equal
                }
                else if (Eat('!'))
                {
                    if (Eat('=')) x = x != ParseTerm() ? 1 : 0;
                    else throw new Exception("Unexpected '!'");
                }
                else return x;
            }
        }

        private decimal ParseTerm()
        {
            var x = ParseFactor();
            for (;;)
            {
                if (Eat('+')) x += ParseFactor(); // addition
                else if (Eat('-')) x -= ParseFactor(); // subtraction
                else return x;
            }
        }

        private decimal ParseFactor()
        {
            var x = ParseUnary();
            for (;;)
            {
                if (Eat('*')) x *= ParseUnary(); // multiplication
                else if (Eat('/'))
                {
                    var y = ParseUnary();
                    if (y == 0) throw new DivideByZeroException("Formula division by zero.");
                    x /= y; // division
                }
                else return x;
            }
        }

        private decimal ParseUnary()
        {
            if (Eat('+')) return ParseUnary(); // unary plus
            if (Eat('-')) return -ParseUnary(); // unary minus

            decimal x = 0;
            var startPos = _pos;
            if (Eat('('))
            {
                x = ParseExpression();
                if (!Eat(')')) throw new Exception("Missing closing parenthesis");
            }
            else if ((_currentChar >= '0' && _currentChar <= '9') || _currentChar == '.')
            {
                while ((_currentChar >= '0' && _currentChar <= '9') || _currentChar == '.') NextChar();
                var str = _expression.Substring(startPos, _pos - startPos);
                if (!decimal.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out x))
                {
                    throw new Exception($"Invalid number format: {str}");
                }
            }
            else if (char.IsLetter(_currentChar) || _currentChar == '_')
            {
                while (char.IsLetterOrDigit(_currentChar) || _currentChar == '_') NextChar();
                var identifier = _expression.Substring(startPos, _pos - startPos);

                if (Eat('('))
                {
                    // Function call
                    var args = new List<decimal>();
                    if (!Eat(')'))
                    {
                        do
                        {
                            args.Add(ParseExpression());
                        } while (Eat(','));
                        if (!Eat(')')) throw new Exception("Missing closing parenthesis in function call");
                    }

                    if (_functions.TryGetValue(identifier, out var func))
                    {
                        try
                        {
                            x = func(args.ToArray());
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error evaluating function '{identifier}': {ex.Message}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Unknown function: {identifier}");
                    }
                }
                else
                {
                    // Variable
                    if (_variables.TryGetValue(identifier, out var val))
                    {
                        x = val;
                    }
                    else
                    {
                        throw new Exception($"Unknown variable: {identifier}");
                    }
                }
            }
            else
            {
                throw new Exception($"Unexpected character: {_currentChar}");
            }

            return x;
        }

        /// <summary>
        /// Evaluates an aggregate expression like SUM(Variable) across a collection of dictionaries.
        /// Useful for calculating CompositeFormula = SUM(WeightedRatio)
        /// </summary>
        public static decimal EvaluateAggregate(string expression, IEnumerable<Dictionary<string, decimal>> items)
        {
            if (string.IsNullOrWhiteSpace(expression)) return 0;
            
            var trimmed = expression.Trim();
            if (trimmed.StartsWith("SUM(", StringComparison.OrdinalIgnoreCase) && trimmed.EndsWith(")"))
            {
                var variableName = trimmed.Substring(4, trimmed.Length - 5).Trim();
                decimal total = 0;
                foreach (var item in items)
                {
                    if (item.TryGetValue(variableName, out var val))
                    {
                        total += val;
                    }
                    // Could throw if missing, but we assume 0 for safety
                }
                return total;
            }

            throw new NotSupportedException("Only SUM(variable) is currently supported for aggregate evaluation.");
        }
    }
}
