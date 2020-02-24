namespace ScimFilterParser.Parser.AbstractSyntaxTree
{
    public class ValuePath : Factor
    {
        public ValuePath(AttributePath attributePath, Expression valueFilter)
        {
            this.AttributePath = attributePath;
            this.ValueFilter = valueFilter;
        }

        public AttributePath AttributePath { get; }

        public Expression ValueFilter { get; }

        public override string ToString()
        {
            return $"{this.AttributePath.ToString()}[{this.ValueFilter.ToString()}]";
        }
    }
}
