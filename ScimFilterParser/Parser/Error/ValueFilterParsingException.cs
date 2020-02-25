namespace ScimFilterParser.Parser.Error
{
    public class ValueFilterParsingException : FilterException
    {
        public ValueFilterParsingException() 
            : base("Value filter failed to parse")
        {
        }
    }
}
