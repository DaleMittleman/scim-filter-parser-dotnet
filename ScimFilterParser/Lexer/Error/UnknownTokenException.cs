namespace ScimFilterParser.Lexer.Error
{
    using System;

    public class UnknownTokenException : Exception
    {
        public UnknownTokenException(string message) : base(message)
        {
        }
    }
}
