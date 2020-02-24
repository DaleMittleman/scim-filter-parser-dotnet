using ScimFilterParser.Parser.AbstractSyntaxTree.Comparison;

namespace ScimFilterParser.Parser.AbstractSyntaxTree
{
    public class ComparisonExpression : Factor
    {
        public ComparisonExpression(AttributePath leftOperand, ComparisonOperator @operator)
        {
            this.LeftOperand = leftOperand;
            this.Operator = @operator;
        }

        public ComparisonExpression(AttributePath leftOperand, ComparisonOperator @operator, CompareValue rightOperand)
        {
            this.LeftOperand = leftOperand;
            this.Operator = @operator;
            this.RightOperand = rightOperand;
        }

        public AttributePath LeftOperand { get; }

        public ComparisonOperator Operator { get; }

        public CompareValue RightOperand { get; }

        public override string ToString()
        {
            if (this.Operator is Present)
            {
                return $"{this.LeftOperand.ToString()} {this.Operator.ToString()}";
            }
            else
            {
                return $"{this.LeftOperand.ToString()} {this.Operator.ToString()} {this.RightOperand.ToString()}";
            }
        }
    }
}
