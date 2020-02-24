namespace ScimFilterParser.Parser.Error
{
    using System;

    public class FilterException : Exception
    {
        public FilterException(string message) : base(message)
        {
        }
    }
}
