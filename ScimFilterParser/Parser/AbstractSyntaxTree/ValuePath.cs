namespace ScimFilterParser.Parser.AbstractSyntaxTree
{
    using ScimFilterParser.Parser.Error;

    public class ValuePath : Factor
    {
        public ValuePath(AttributePath attributePath, Expression valueFilter)
        {
            this.AttributePath = attributePath;
            this.ValueFilter = valueFilter;

            if (valueFilter is ValuePath
                || (valueFilter is GroupedExpression grouped && grouped.Expression is ValuePath)
                || (valueFilter is NegatedExpression negated && negated.Expression.Expression is ValuePath))
            {
                throw new ValueFilterParsingException();
            }
        }

        public AttributePath AttributePath { get; }

        public Expression ValueFilter { get; }

        public override string ToString()
        {
            return $"{this.AttributePath.ToString()}[{this.ValueFilter.ToString()}]";
        }
    }
}
