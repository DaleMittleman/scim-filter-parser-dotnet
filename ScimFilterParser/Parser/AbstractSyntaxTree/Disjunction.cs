using System;
using System.Collections.Generic;
using System.Text;

namespace ScimFilterParser.Parser.AbstractSyntaxTree
{
    public class Disjunction : Expression
    {
        public Disjunction(Term leftOperand, Expression rightOperand = null)
        {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }

        public Term LeftOperand { get; }

        public Expression RightOperand { get; }

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
