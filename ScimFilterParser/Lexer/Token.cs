namespace ScimFilterParser.Lexer
{
    public class Token
    {
        public Token(string name, string value, int offset, int position)
        {
            this.Name = name;
            this.Value = value;
            this.Offset = offset;
            this.Position = position;
        }

        public string Name { get; protected set; }

        public string Value { get; protected set; }

        public int Offset { get; protected set; }

        public int Position { get; protected set; }

        public bool Is(Token token)
        {
            return token.Name == this.Name;
        }

        public bool Is(string tokenName)
        {
            return tokenName == this.Name;
        }
    }
}
