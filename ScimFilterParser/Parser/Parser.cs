namespace ScimFilterParser.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using ScimFilterParser.Lexer;
    using ScimFilterParser.Lexer.Config;
    using ScimFilterParser.Lexer.Error;
    using ScimFilterParser.Parser.AbstractSyntaxTree;
    using ScimFilterParser.Parser.AbstractSyntaxTree.Comparison;
    using ScimFilterParser.Parser.Error;

    public class Parser
    {
        private readonly Lexer lexer;

        public Parser(ParserMode? mode = null, SCIMVersion? version = null)
        {
            var tokenConfig = new List<KeyValuePair<string, Regex>>()
            {
                new KeyValuePair<string, Regex>(TokenType.Whitespace, new Regex("^\\s+")),

                // JSON number syntax : https://tools.ietf.org/html/rfc8259#section-6
                new KeyValuePair<string, Regex>(TokenType.Number, 
                    new Regex("^-? (0(?!\\d)|([1-9]\\d*)) (\\.\\d+)? ([e|E][+|-]?\\d+)?", RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace)),

                new KeyValuePair<string, Regex>(TokenType.String, new Regex("^\"(?:[^\"\\\\]|\\\\.)*\"")),
                new KeyValuePair<string, Regex>(TokenType.DotOperator, new Regex("^\\.")),
                new KeyValuePair<string, Regex>(TokenType.Colon, new Regex("^:")),
                new KeyValuePair<string, Regex>(TokenType.Slash, new Regex("^/")),
                new KeyValuePair<string, Regex>(TokenType.OpenParen, new Regex("^\\(")),
                new KeyValuePair<string, Regex>(TokenType.CloseParen, new Regex("^\\)")),
                new KeyValuePair<string, Regex>(TokenType.Name, new Regex("^[a-zA-Z0-9-_]+")),
            };

            // Value path syntax only valid after V1
            if (version != SCIMVersion.V1)
            {
                tokenConfig.AddRange(new List<KeyValuePair<string, Regex>>()
                {
                    new KeyValuePair<string, Regex>(TokenType.OpenBracket, new Regex("^\\[")),
                    new KeyValuePair<string, Regex>(TokenType.CloseBracket, new Regex("^\\]")),
                });
            }

            this.lexer = new Lexer(new LexerConfig(tokenConfig));
            
            this.Mode = mode ?? ParserMode.Filter;
            this.Version = version ?? SCIMVersion.V2;

            if (this.Mode == ParserMode.Path && this.Version == SCIMVersion.V1)
            {
                throw new ArgumentException("Path mode is available only in SCIM version 2");
            }
        }

        public ParserMode Mode { get; set; }

        public SCIMVersion Version { get; set; }
        
        public Root Parse(string input)
        {
            try
            {
                this.lexer.Scan(input);
            }
            catch (Exception ex)
            {
                throw new FilterException("Failed to tokenize filter string.", ex);
            }

            Root node;
            if (this.Mode == ParserMode.Filter) 
            {
                node = this.Expression();
            }
            else 
            {
                node = this.Path();
            }

            this.Match(null);

            return node;
        }

        private Expression Expression()
        {
            Term leftOperand = this.Term();
            
            var token = this.lexer.Token;
            if (token != null && token.Is(TokenType.Whitespace))
            {
                var nextToken = this.lexer.Glimpse();
                if (this.IsName("or", nextToken))
                {
                    this.Match(TokenType.Whitespace);
                    this.Match(TokenType.Name);
                    this.Match(TokenType.Whitespace);

                    var rightOperand = this.Expression();

                    return new Disjunction(leftOperand, rightOperand);
                }
            }

            return leftOperand;
        }

        private Path Path()
        {
            var attributePath = this.AttributePath();

            var token = this.lexer.Token;
            if (token != null && token.Is(TokenType.OpenBracket))
            {
                this.Match(TokenType.OpenBracket);

                var valueFilter = this.Expression();

                this.Match(TokenType.CloseBracket);

                var valuePath = new ValuePath(attributePath, valueFilter);

                token = this.lexer.Token;
                if (token != null && token.Is(TokenType.DotOperator))
                {
                    this.Match(TokenType.DotOperator);

                    string subAttribute = null;
                    token = this.lexer.Token;
                    if (token != null && token.Is(TokenType.Name))
                    {
                        subAttribute = this.lexer.Token.Value;
                        this.Match(TokenType.Name);
                    }

                    return new Path(valuePath, subAttribute);
                }

                return new Path(valuePath);
            }
            else if (token != null)
            {
                this.SyntaxError("Path");
            }

            return new Path(attributePath);
        }

        private Term Term()
        {
            Factor leftOperand = this.Factor();

            var token = this.lexer.Token;
            if (token != null && token.Is(TokenType.Whitespace))
            {
                var nextToken = this.lexer.Glimpse();
                if (this.IsName("and", nextToken))
                {
                    this.Match(TokenType.Whitespace);
                    this.Match(TokenType.Name);
                    this.Match(TokenType.Whitespace);

                    var rightOperand = this.Term();

                    return new Conjunction(leftOperand, rightOperand);
                }
            }

            return leftOperand;
        }

        private Factor Factor()
        {
            var token = this.lexer.Token;
            if (this.IsName("not", token))
            {
                return this.NegatedExpression();
            }
            else if (token.Is(TokenType.OpenParen))
            {
                return this.GroupedExpression();
            }
            else if (this.IsValuePathIncoming() && this.Version != SCIMVersion.V1)
            {
                return this.ValuePath();
            }
            else
            {
                return this.ComparisonExpression();
            }
        }

        private bool IsValuePathIncoming()
        {
            var tokenAfterAttributePath = this.lexer.PeekWhileTokens(new List<string>() { TokenType.Name, TokenType.DotOperator });
            this.lexer.ResetPeek();

            return tokenAfterAttributePath != null ? tokenAfterAttributePath.Is(TokenType.OpenBracket) : false;
        }

        private NegatedExpression NegatedExpression()
        {
            this.Match(TokenType.Name);
            this.Match(TokenType.Whitespace);

            return new NegatedExpression(this.GroupedExpression());
        }

        private GroupedExpression GroupedExpression()
        {
            this.Match(TokenType.OpenParen);

            var groupedFilter = new GroupedExpression(this.Expression());

            this.Match(TokenType.CloseParen);

            return groupedFilter;
        }

        private ValuePath ValuePath()
        {
            var attributePath = this.AttributePath();

            this.Match(TokenType.OpenBracket);

            var valueFilter = this.Expression();

            this.Match(TokenType.CloseBracket);

            return new ValuePath(attributePath, valueFilter);
        }

        private ComparisonExpression ComparisonExpression()
        {
            var attributePath = this.AttributePath();

            this.Match(TokenType.Whitespace);

            var @operator = this.ComparisonOperator();
            
            if (@operator is Present)
            {
                return new ComparisonExpression(attributePath, @operator);
            }

            this.Match(TokenType.Whitespace);
            
            var compareValue = this.CompareValue();

            return new ComparisonExpression(attributePath, @operator, compareValue);
        }

        private AttributePath AttributePath()
        {
            var validTokenTypes = new List<string>() { TokenType.Number, TokenType.Name, TokenType.Colon, TokenType.Slash, TokenType.DotOperator };
            var stoppingTokenTypes = new List<string>() { TokenType.Whitespace, TokenType.OpenBracket };

            var sb = new StringBuilder();
            while (true) {
                var token = this.lexer.Token;

                if (token == null)
                {
                    break;
                }

                var isValid = validTokenTypes.Any(validTokenName => validTokenName == token.Name);
                var isStopping = stoppingTokenTypes.Any(stoppingTokenName => stoppingTokenName == token.Name);

                if (isStopping)
                {
                    break;
                }

                if (!isValid) {
                    this.SyntaxError("attribute path");
                }

                sb.Append(token.Value);
                this.lexer.MoveNext();
            }

            if (sb.Length == 0)
            {
                this.SyntaxError("attribute path");
            }

            return ScimFilterParser.Parser.AbstractSyntaxTree.AttributePath.Parse(sb.ToString());
        }

        private ComparisonOperator ComparisonOperator()
        {
            if (this.lexer.Token == null || !this.lexer.Token.Is(TokenType.Name))
            {
                this.SyntaxError("comparision operator");
            }

            ComparisonOperator comparisonOperator = null;
            switch (this.lexer.Token.Value)
            {
                case "pr":
                    comparisonOperator = new Present();
                    break;
                case "eq":
                    comparisonOperator = new Equal();
                    break;
                case "ne":
                    comparisonOperator = new NotEqual();
                    break;
                case "co": 
                    comparisonOperator = new Contains();
                    break;
                case "sw":
                    comparisonOperator = new StartsWith();
                    break;
                case "ew": 
                    comparisonOperator = new EndsWith();
                    break;
                case "gt":
                    comparisonOperator = new GreaterThan();
                    break;
                case "lt": 
                    comparisonOperator = new LessThan();
                    break;
                case "ge":
                    comparisonOperator = new GreaterThanOrEqual();
                    break;
                case "le":
                    comparisonOperator = new LessThanOrEqual();
                    break;
                default:
                    this.SyntaxError("comparision operator");
                    break;
            }
            
            this.Match(this.lexer.Token.Name);
            return comparisonOperator;
        }

        private CompareValue CompareValue()
        {
            var token = this.lexer.Token;
            CompareValue compareValue = null;
            if (this.IsName("true", token))
            {
                compareValue = new CompareValue(true);
            }
            else if (this.IsName("false", token))
            {
                compareValue = new CompareValue(false);
            }
            else if (this.IsName("null", token))
            {
                compareValue = new CompareValue();
            }
            else if (token.Is(TokenType.Number))
            {
                if (double.TryParse(token.Value, out var @double))
                {
                    compareValue = new CompareValue(@double);
                }
                else
                {
                    // Invalid number
                    this.SyntaxError("compare value", token);
                }
            }
            else if (token.Is(TokenType.String))
            {
                var trimmedString = token.Value.Trim('\"');
                compareValue = new CompareValue(trimmedString);
            }
            else
            {
                this.SyntaxError("compare value", token);
            }

            this.Match(token.Name);

            return compareValue;
        }
        
        private bool IsName(string value, Token token)
        {
            if (token == null) {
                return false;
            }
            if (!token.Is(TokenType.Name)) {
                return false;
            }

            return token.Value == value;
        }

        private void Match(string tokenName)
        {
            var token = this.lexer.Token;
            if (string.IsNullOrWhiteSpace(tokenName))
            {
                if (token != null) {
                   this.SyntaxError("end of input");
                }
            } 
            else
            {
                if (token == null || !token.Is(tokenName))
                {
                    this.SyntaxError(tokenName);
                }

                this.lexer.MoveNext();
            }
        }

        private void SyntaxError(string expected, Token token = null)
        {
            if (token == null)
            {
                token = this.lexer.LookAhead;
            }

            int offset;
            if (token != null)
            {
                offset = token.Offset;
                BuildAndThrowException();
            }

            token = this.lexer.Token;
            if (token != null)
            {
                offset = token.Offset;
                BuildAndThrowException();
            }
            else
            {
                offset = this.lexer.Input.Length;
                BuildAndThrowException();
            }

            void BuildAndThrowException()
            {
                var sb = new StringBuilder();
                sb.Append($"line 0, col {offset}... Error: ");
                sb.Append(string.IsNullOrWhiteSpace(expected) ? $"Expected {{{expected}}}, got " : "Unexpected ");
                sb.Append(token == null ? "end of string" : $"'{token.Value}'");

                throw new FilterException(sb.ToString());
            }
        }
    }
}
