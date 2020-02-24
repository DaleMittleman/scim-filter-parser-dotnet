namespace ScimFilterParser.Parser.AbstractSyntaxTree
{
    using System.Linq;
    using System.Text;
    using ScimFilterParser.Parser.Error;

    public class AttributePath
    {
        private AttributePath(string schema, string attributeName, string subAttributeName)
        {
            this.Schema = schema;
            this.AttributeName = attributeName;
            this.SubAttribute = subAttributeName;
        }

        public string Schema { get; }

        public string AttributeName { get; }

        public string SubAttribute { get; }

        public static AttributePath Parse(string input)
        {
            string schema = null;
            string attributeName;
            string subAttribute = null;
            string attributePathNoSchema; 

            var attributePathSplit = input.Split(':');

            // Attribute path contains a schema
            if (attributePathSplit.Length > 1)
            {
                schema = string.Join(':', attributePathSplit.Take(attributePathSplit.Length - 1));
                attributePathNoSchema = attributePathSplit[^1];
            }
            else
            {
                attributePathNoSchema = input;
            }

            var attributePathNoSchemaSplit = attributePathNoSchema.Split('.');
            
            if (attributePathNoSchemaSplit.Length > 2)
            {
                throw new AttributePathParsingException("Provided attribute path is too long.  Only one sub-attribute allowed as per RFC 7644.");
            }

            // Sub-attribute exists
            if (attributePathNoSchemaSplit.Length == 2)
            {
                attributeName = attributePathNoSchemaSplit[0];
                subAttribute = attributePathNoSchemaSplit[1];
            }
            else
            {
                attributeName = attributePathNoSchema;
            }

            return new AttributePath(schema, attributeName, subAttribute);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.Schema != null)
            {
                sb.Append(this.Schema);
                sb.Append(':');
            }

            sb.Append(this.AttributeName);

            if (this.SubAttribute != null)
            {
                sb.Append('.');
                sb.Append(this.SubAttribute);
            }

            return sb.ToString();
        }
    }
}
