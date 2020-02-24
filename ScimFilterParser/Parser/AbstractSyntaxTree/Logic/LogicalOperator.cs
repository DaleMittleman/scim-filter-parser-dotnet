namespace ScimFilterParser.Parser.AbstractSyntaxTree.Logic
{
    public abstract class LogicalOperator
    {
        public abstract string TokenValue { get; }

        public override string ToString()
        {
            return this.TokenValue;
        }
    }
}
