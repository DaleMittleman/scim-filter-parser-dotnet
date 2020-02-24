namespace ScimFilterParser.Parser.AbstractSyntaxTree
{
    public class Conjunction : Term
    {
        public Conjunction(Factor leftOperand, Term rightOperand = null)
        {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public Factor LeftOperand { get; }

        public Term RightOperand { get; }

        public override string ToString()
        {
            if (this.RightOperand == null)
            {
                return this.LeftOperand.ToString();
            }
            else
            {
                return $"{this.LeftOperand.ToString()} and {this.RightOperand.ToString()}";
            }
        }
    }
}
