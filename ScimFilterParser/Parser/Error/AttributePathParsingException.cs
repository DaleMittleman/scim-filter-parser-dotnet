namespace ScimFilterParser.Parser.Error
{
    public class AttributePathParsingException : FilterException
    {
        public AttributePathParsingException(string message)
            : base($"Attribute path failed to parse: {message}")
        {
        }
    }
}
