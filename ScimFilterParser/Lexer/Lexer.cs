namespace ScimFilterParser.Lexer
{
    using System.Collections.Generic;
    using System.Linq;
    using ScimFilterParser.Lexer.Config;
    using ScimFilterParser.Lexer.Error;

    public class Lexer
    {
        private readonly LexerConfig config;
        private int peek;
        private List<Token> tokens;

        public Lexer(LexerConfig config)
        {
            this.config = config;
        }

        public string Input { get; private set; }

        public int Position { get; private set; }

        public Token LookAhead { get; private set; }

        public Token Token { get; private set; }

        public void Scan(string input)
        {
            this.Input = input;

            var offset = 0;
            var position = 0;

            var tokens = new List<Token>();

            while (input.Length > 0)
            {
                var anyMatch = false;
                foreach (var tokenDefinition in this.config.TokenDefinitions)
                {
                    if (tokenDefinition.Regex.IsMatch(input))
                    {
                        var match = tokenDefinition.Regex.Match(input);
                        tokens.Add(new Token(tokenDefinition.Name, match.Value, offset, position));
                        ++position;

                        input = input.Substring(match.Length);
                        anyMatch = true;
                        offset += match.Length;
                        break;
                    }
                }

                if (!anyMatch)
                {
                    throw new UnknownTokenException($"At offset {offset}: {(input.Length >= 16 ? input.Substring(0, 16) : input)}...");
                }
            }

            this.peek = 1;
            this.Position = 0;
            this.tokens = tokens;
            this.Token = tokens.ElementAtOrDefault(0);
            this.LookAhead = tokens.ElementAtOrDefault(1);
        }

        public void Reset()
        {
            this.Position = 0;
            this.peek = 0;
            this.Token = null;
            this.LookAhead = null;
        }
        public void ResetPeek()
        {
            this.peek = 1;
        }

        public void ResetPosition(int position = 0)
        {
            this.Position = position;
            this.Token = this.tokens.ElementAtOrDefault(position);
        }

        public bool IsToken(string tokenName)
        {
            return this.Token != null && this.Token.Is(tokenName);
        }

        public bool IsNextToken(string tokenName)
        {
            return this.LookAhead != null && this.LookAhead.Is(tokenName);
        }

        public bool IsTokenAny(IEnumerable<string> tokenNames)
        {
            return this.Token != null && tokenNames.Any(tn => this.Token.Is(tn));
        }

        public bool IsNextTokenAny(IEnumerable<string> tokenNames)
        {
            return this.LookAhead != null && tokenNames.Any(tn => this.LookAhead.Is(tn));
        }

        public bool MoveNext()
        {
            this.peek = 1;
            this.Position++;

            this.Token = this.tokens.ElementAtOrDefault(this.Position);
            this.LookAhead = this.tokens.ElementAtOrDefault(this.Position + 1);

            return this.Token != null;
        }

        public void SkipUntil(string tokenName)
        {
            while (this.Token != null && this.Token.Name != tokenName)
            {
                this.MoveNext();
            }
        }

        public void Skiptokens(IEnumerable<string> tokenNames)
        {
            while (this.Token != null && tokenNames.Any(tn => this.Token.Name == tn))
            {
                this.MoveNext();
            }
        }

        public Token Peek()
        {
            return this.tokens.ElementAtOrDefault(this.Position + this.peek++);
        }

        public Token PeekWhileTokens(IEnumerable<string> tokenNames)
        {
            var token = this.Peek();
            while (token != null)
            {
                if (!tokenNames.Any(tn => token.Name == tn))
                {
                    break;
                }

                token = this.Peek();
            }

            return token;
        }

        public Token Glimpse()
        {
            var token = this.Peek();
            this.ResetPeek();

            return token;
        }
    }
}
