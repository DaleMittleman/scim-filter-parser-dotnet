namespace ScimFilterParser.Parser.AbstractSyntaxTree
{
    public class GroupedExpression : Factor
    {
        public GroupedExpression(Expression internalExpression)
        {
            this.Expression = internalExpression;
        }

        public Expression Expression { get; }

        public override string ToString()
        {
            return $"({this.Expression.ToString()})";
        }
    }
}
