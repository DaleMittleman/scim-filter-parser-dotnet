namespace ScimFilterParser.Lexer
{
    public static class TokenType
    {
        public const string Whitespace = "whitespace";
        public const string Number = "number";
        public const string String = "string";
        public const string DotOperator = "dot";
        public const string Colon = "colon";
        public const string Slash = "slash";
        public const string OpenParen = "open_paren";
        public const string CloseParen = "close_paren";
        public const string Name = "name";
        public const string OpenBracket = "open_bracket";
        public const string CloseBracket = "close_bracket";
    }
}
