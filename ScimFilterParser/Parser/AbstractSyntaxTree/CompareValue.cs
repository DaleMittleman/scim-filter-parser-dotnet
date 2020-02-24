namespace ScimFilterParser.Parser.AbstractSyntaxTree
{
    using System;

    public class CompareValue
    {
        public CompareValue()
        {
            this.Value = null;
            this.IsNull = true;
        }

        public CompareValue(bool @bool)
        {
            this.Value = @bool;
            this.IsBool = true;
        }

        public CompareValue(string @string)
        {
            this.Value = @string;
            this.IsString = true;

            if (DateTime.TryParse(@string, out _))
            {
                this.IsValidDateTime = true;
            }
        }

        public CompareValue(double @number)
        {
            this.Value = @number;
            this.IsNumber = true;
        }

        public object Value { get; }

        public bool IsBool { get; }

        public bool IsString { get; }

        public bool IsValidDateTime { get; }

        public bool IsNumber { get; }

        public bool IsNull { get; }

        public override string ToString()
        {
            if (this.IsBool)
            {
                return (bool)this.Value ? "true" : "false";
            }
            else if (this.IsNumber)
            {
                return ((double)this.Value).ToString();
            }
            else if (this.IsString)
            {
                return $"\"{(string)this.Value}\"";
            }
            else
            {
                return "null";
            }
        }
    }
}
