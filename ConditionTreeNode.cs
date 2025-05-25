using System;
using System.Collections.Generic;

public abstract class ConditionTreeNode<T>
{
    public abstract bool IsMatch(T context);

    public static ConditionTreeNode<T> Parse(string raw, Func<string, ConditionTreeNode<T>> leafFactory)
    {
        return new Parser(raw, leafFactory).ParseExpression();
    }

    private class Parser
    {
        private readonly List<string> _tokens;
        private int _index;
        private readonly Func<string, ConditionTreeNode<T>> _leafFactory;

        public Parser(string raw, Func<string, ConditionTreeNode<T>> leafFactory)
        {
            _tokens = Tokenize(raw);
            _index = 0;
            _leafFactory = leafFactory;
        }

        public ConditionTreeNode<T> ParseExpression()
        {
            if (_tokens.Count == 0)
                return new AlwaysTrueNode<T>();

            var node = ParseOr();

            if (_index < _tokens.Count)
                throw new Exception($"Unexpected token: {_tokens[_index]}");

            return node;
        }

        private ConditionTreeNode<T> ParseOr()
        {
            var node = ParseAnd();
            while (_index < _tokens.Count)
            {
                var token = _tokens[_index];
                if (!IsOr(token)) break;
                _index++;
                node = new OrNode<T>(node, ParseAnd());
            }
            return node;
        }

        private ConditionTreeNode<T> ParseAnd()
        {
            var node = ParseUnary();
            while (_index < _tokens.Count)
            {
                var token = _tokens[_index];
                if (!IsAnd(token)) break;
                _index++;
                node = new AndNode<T>(node, ParseUnary());
            }
            return node;
        }

        private ConditionTreeNode<T> ParseUnary()
        {
            if (_index < _tokens.Count && _tokens[_index] == "!")
            {
                _index++;
                return new NotNode<T>(ParseUnary());
            }
            return ParsePrimary();
        }

        private ConditionTreeNode<T> ParsePrimary()
        {
            if (_index < _tokens.Count && _tokens[_index] == "(")
            {
                _index++;
                var expr = ParseOr();
                if (_index >= _tokens.Count || _tokens[_index] != ")")
                    throw new Exception("Expected ')'");
                _index++;
                return expr;
            }

            if (_index >= _tokens.Count)
                throw new Exception("Unexpected end of expression");

            return _leafFactory(_tokens[_index++]);
        }

        private static bool IsAnd(string token) => token == "&&" || token == "&";
        private static bool IsOr(string token) => token == "||" || token == "|";

        private static List<string> Tokenize(string input)
        {
            var tokens = new List<string>(16);
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsWhiteSpace(c)) continue;

                if (i + 1 < input.Length && ((c == '&' && input[i + 1] == '&') || (c == '|' && input[i + 1] == '|')))
                {
                    if (sb.Length > 0) { tokens.Add(sb.ToString()); sb.Clear(); }
                    tokens.Add(new string(c, 2));
                    i++;
                }
                else if (c == '&' || c == '|' || c == '(' || c == ')' || c == '!')
                {
                    if (sb.Length > 0) { tokens.Add(sb.ToString()); sb.Clear(); }
                    tokens.Add(c.ToString());
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
                tokens.Add(sb.ToString());

            return tokens;
        }
    }
}

public class AndNode<T> : ConditionTreeNode<T>
{
    private readonly ConditionTreeNode<T> _left, _right;
    public AndNode(ConditionTreeNode<T> left, ConditionTreeNode<T> right)
    {
        _left = left;
        _right = right;
    }

    public override bool IsMatch(T context) => _left.IsMatch(context) && _right.IsMatch(context);
}

public class OrNode<T> : ConditionTreeNode<T>
{
    private readonly ConditionTreeNode<T> _left, _right;
    public OrNode(ConditionTreeNode<T> left, ConditionTreeNode<T> right)
    {
        _left = left;
        _right = right;
    }

    public override bool IsMatch(T context) => _left.IsMatch(context) || _right.IsMatch(context);
}

public class NotNode<T> : ConditionTreeNode<T>
{
    private readonly ConditionTreeNode<T> _node;
    public NotNode(ConditionTreeNode<T> node)
    {
        _node = node;
    }

    public override bool IsMatch(T context) => !_node.IsMatch(context);
}

public class LeafNode<T> : ConditionTreeNode<T>
{
    private readonly ConditionEvaluator<T> _condition;

    public LeafNode(ConditionEvaluator<T> condition, string raw = null)
    {
        if (raw != null) condition.Parse(raw);
        _condition = condition;
    }

    public override bool IsMatch(T context) => _condition.Evaluate(context);
    public override string ToString() => _condition.ToString();
}

public class AlwaysTrueNode<T> : ConditionTreeNode<T>
{
    public override bool IsMatch(T context) => true;
}
