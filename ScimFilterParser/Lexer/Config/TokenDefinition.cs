namespace ScimFilterParser.Lexer.Config
{
    using System.Text.RegularExpressions;

    public class TokenDefinition
    {
        public TokenDefinition(string name, Regex regex)
        {
            this.Name = name;
            this.Regex = regex;
        }

        public string Name { get; protected set; }

        public Regex Regex { get; protected set; }
    }
}
