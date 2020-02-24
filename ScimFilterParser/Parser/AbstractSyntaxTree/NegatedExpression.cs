namespace ScimFilterParser.Parser.AbstractSyntaxTree
{
    public class NegatedExpression : Factor
    {
        public NegatedExpression(GroupedExpression internalGroupedExpression)
        {
            this.Expression = internalGroupedExpression;
        }

        public GroupedExpression Expression { get; }

        public override string ToString()
        {
            return $"not {this.Expression.ToString()}";
        }
    }
}
