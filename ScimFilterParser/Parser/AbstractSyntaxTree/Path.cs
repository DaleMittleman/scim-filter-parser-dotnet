namespace ScimFilterParser.Parser.AbstractSyntaxTree
{
    using System.Text;

    public class Path : Root
    {
        public Path(AttributePath attributePath)
        {
            this.AttributePath = attributePath;
        }

        public Path(ValuePath valuePath, string subAttribute = null)
        {
            this.AttributePath = valuePath.AttributePath;
            this.ValueFilter = valuePath.ValueFilter;
            this.SubAttribute = subAttribute;
        }

        public AttributePath AttributePath { get; }

        public Expression ValueFilter { get; }

        public string SubAttribute { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.AttributePath);
            
            if (this.ValueFilter != null)
            {
                sb.Append('[');
                sb.Append(this.ValueFilter.ToString());
                sb.Append('[');

                if (this.SubAttribute != null)
                {
                    sb.Append('.');
                    sb.Append(this.SubAttribute);
                }
            }

            return sb.ToString();
        }
    }
}
